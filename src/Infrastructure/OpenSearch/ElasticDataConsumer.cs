
using System.Globalization;
using Application.Contracts;
using Application.Services;
using Domain.Entity;
using Infrastructure.OpenSearch;
using OpenSearch.Client;
using OpenSearch.Net;
using Serilog;

public class ElasticDataConsumer : IElasticDataConsumer
{
    private readonly IElasticClientWrapper _client;
    private readonly IElasticIndexManager _indexManager;

    public ElasticDataConsumer(
        IElasticClientWrapper clientWrapper,
        IElasticIndexManager indexManager
    )
    {
        _client = clientWrapper;
        _indexManager = indexManager;
    }

    public async Task ConsumeData(Stock data)
    {
        var index = _indexManager.EntryIndex();
        Log.Information("Consume data: {data}", data);
        await _indexManager.CreateIndices();

        var indexResponse = await IndexItem(index, data, GetKey(data));
        if (!(indexResponse?.IsValid ?? false))
        {
            Log.Error(
                $"Failed to write update to Order {GetKey(data)} in index {index} with result {indexResponse?.Result}.\nDebugInformation: {indexResponse?.DebugInformation}");
        }
    }

    private static string GetKey(Stock data) => data.Name + "-" + data.CreatedAt.ToString("hh:mm:ss.F", CultureInfo.InvariantCulture);

    /// <summary>
    /// Update bulk records
    /// </summary>
    /// <param name="lstStockEntry"></param>
    /// <param name="orderId"></param>
    /// <returns></returns>
    public async Task UpdateBulkStockEntry(List<Stock> dataList)
    {
        var index = _indexManager.EntryIndex();
        await _indexManager.CreateIndices();

        if (dataList != null && dataList.Count > 0)
        {
            var bulkDescriptor = new BulkDescriptor();
            foreach (var data in dataList)
            {
                bulkDescriptor.Index<Stock>(op => op
                    .Index(index)
                    .Document(data)
                    .Id(GetKey(data)));
            }

            var stockIndexResponse = await _client.BulkAsync(bulkDescriptor);
            if (!(stockIndexResponse?.IsValid ?? false))
            {
                Log.Error(
                    $"Failed to bulk update: {stockIndexResponse?.DebugInformation}");
            }
        }
        else
        {
            Log.Debug($"No entry passed.");
        }
    }

    public async Task DeleteFromAllIndices(string metaOrderNumber)
    {
        var indices = (await _client.Indices.GetAsync(Indices.AllIndices)).Indices.Keys
            .Where(k => k.Name.StartsWith("my-fav-stocks")).ToList();
        var tasks = new List<Task>();
        foreach (var index in indices)
        {
            tasks.Add(_client.DeleteAsync(new DeleteRequest(index, metaOrderNumber) { Refresh = Refresh.True }));
        }

        await Task.WhenAll(tasks);
    }

    private async Task<IIndexResponseWrapper> IndexItem<T>(string index, T item, string id, int retries = 10)
        where T : class
    {
        int attempt = 0;
        IIndexResponseWrapper indexResponse = null;
        while (++attempt <= retries)
        {
            var itemDetails = await GetCurrentDocumentDetails<T>(index, id);

            indexResponse = await _client.IndexAsync(item,
                descriptor => AddOptimisticLocking(itemDetails.Primary, itemDetails.Sequence,
                    descriptor.Index(index).Refresh(Refresh.True)));
            if (indexResponse.Result == Result.Created || indexResponse.Result == Result.Updated)
            {
                return indexResponse;
            }
        }

        Log.Error(
            $"Failed to write {item} to index {index} with result {indexResponse?.Result}.\nDebugInformation:{indexResponse?.DebugInformation}");
        return null;
    }

    private async Task<UpdateDetails> GetCurrentDocumentDetails<T>(string index, string id) where T : class
    {
        var response = await _client.GetAsync(new DocumentPath<T>(new Id(id)), selector => selector.Index(index));
        if (response.Found)
        {
            return new UpdateDetails { Primary = response.PrimaryTerm, Sequence = response.SequenceNumber };
        }

        return new UpdateDetails();
    }

    private IndexDescriptor<T> AddOptimisticLocking<T>(long? primary, long? sequence, IndexDescriptor<T> descriptor)
        where T : class
    {
        if (primary != null && sequence != null)
        {
            return descriptor.IfPrimaryTerm(primary).IfSequenceNumber(sequence);
        }

        return descriptor;
    }

    //utility for multivalued return. tuples are ugly.
    private class UpdateDetails
    {
        public long? Primary
        {
            get; set;
        }
        public long? Sequence
        {
            get; set;
        }
    }
}