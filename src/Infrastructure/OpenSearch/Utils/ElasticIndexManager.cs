using System.Collections.Concurrent;
using System.Runtime.Serialization;
using Application.Services;
using Domain.Utils;
using Infrastructure.OpenSearch;
using OpenSearch.Client;
using Serilog;

public interface IElasticIndexManager
{
    string EntryIndex();
    Task CreateIndices();
    Task DropIndices();
}

public class ElasticIndexManager : IElasticIndexManager
{
    private static readonly ConcurrentDictionary<string, bool> _existingIndices =
        new ConcurrentDictionary<string, bool>();

    private static readonly SemaphoreLocker _lock = new SemaphoreLocker();
    private readonly IOpenSearchClient _client;
    private readonly OpenSearchOptions _elasticOptions;

    public ElasticIndexManager(
        IOpenSearchClientProvider clientProvider,
        OpenSearchOptions elasticOptions)
    {
        _client = clientProvider.CreateClient();
        _elasticOptions = elasticOptions;
    }

    public string EntryIndex()
    {
        return $"my-fav-stock".ToLowerInvariant();
    }

    public async Task CreateIndices()
    {
        await CreateIndices<ScrapedData>(EntryIndex);
    }

    public async Task DropIndices()
    {
        await DropIndices<ScrapedData>(EntryIndex);
    }

    private async Task CreateIndices<T>(Func<string> indexNameGenerator) where T : class
    {
        var indexName = indexNameGenerator().ToLowerInvariant();
        Log.Debug($"Checking if index {indexName} exists");

        if (_existingIndices.ContainsKey(indexName))
        {
            return;
        }

        var exists = _client.Indices.Exists(indexName);
        if (exists.Exists)
        {
            _existingIndices.AddOrUpdate(indexName, true, (string s, bool b) => true);
            return;
        }

        Log.Debug($"Creating index {indexName}");

        var created = await _client.Indices
            .CreateAsync(indexName, d => d.Map<T>(se => se.AutoMap())
                .Settings(s => s.NumberOfShards(5)
                    .Setting("index.mapping.total_fields.limit", 200)
                    .NumberOfReplicas(5)
                    .Analysis(a =>
                        a.Analyzers(aa =>
                            aa.Standard(
                                "english",
                                descriptor => descriptor)))));

        if (created.ApiCall.Success)
        {
            Log.Debug($"Index {indexName} created successfully");
            _existingIndices.AddOrUpdate(indexName, true, (string s, bool b) => true);
        }
        else
        {
            throw new IndexNotCreatedException(indexName, created.ApiCall.HttpStatusCode ?? 500,
                created.ApiCall.DebugInformation);
        }
    }

    private async Task DropIndices<T>(Func<string> indexNameGenerator) where T : class
    {
        var indexName = indexNameGenerator().ToLowerInvariant();

        var deleted = await _client.Indices.DeleteAsync(indexName);

        if (deleted.ApiCall.Success)
        {
            _existingIndices.TryRemove(indexName, out bool dummy);
        }
        else
        {
            throw new IndexNotCreatedException(indexName, deleted.ApiCall.HttpStatusCode ?? 500,
                deleted.ApiCall.DebugInformation);
        }
    }
}

[Serializable]
internal class IndexNotCreatedException : Exception
{
    private string indexName;
    private object value;
    private object debugInformation;

    public IndexNotCreatedException()
    {
    }

    public IndexNotCreatedException(string message) : base(message)
    {
    }

    public IndexNotCreatedException(string message, Exception innerException) : base(message, innerException)
    {
    }

    public IndexNotCreatedException(string indexName, object value, object debugInformation)
    {
        this.indexName = indexName;
        this.value = value;
        this.debugInformation = debugInformation;
    }

    protected IndexNotCreatedException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}