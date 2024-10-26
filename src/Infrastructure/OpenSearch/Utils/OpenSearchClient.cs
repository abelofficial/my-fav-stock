using OpenSearch.Client;
using OpenSearch.Client.Specification.IndicesApi;
using OpenSearch.Net;

namespace Infrastructure.OpenSearch;

public class ElasticClientWrapper : IElasticClientWrapper
{
    private readonly IOpenSearchClient _client;
    private readonly IIndicesNamespaceWrapper _indicesWrapper;

    public ElasticClientWrapper(IOpenSearchClientProvider clientProvider)
    {
        _client = clientProvider.CreateClient();
        _indicesWrapper = new IndicesNamespaceWrapper(_client.Indices);
    }

    public IOpenSearchLowLevelClient LowLevel => _client.LowLevel;
    public IIndicesNamespaceWrapper Indices => _indicesWrapper;

    public async Task<IGetResponseWrapper<T>> GetAsync<T>(DocumentPath<T> documentPath,
        Func<GetDescriptor<T>, IGetRequest> func) where T : class
    {
        return new GetResponseWrapper<T>(await _client.GetAsync(documentPath, func));
    }

    public async Task<IIndexResponseWrapper> IndexAsync<T>(T item, Func<IndexDescriptor<T>, IIndexRequest<T>> func,
        CancellationToken ct = default(CancellationToken)) where T : class
    {
        return new IndexResponseWrapper(await _client.IndexAsync(item, func, ct));
    }

    public async Task<DeleteResponse> DeleteAsync(IDeleteRequest deleteRequest)
    {
        return await _client.DeleteAsync(deleteRequest);
    }

    public async Task<BulkResponse> BulkAsync(BulkDescriptor bulkDescriptor)
    {
        return await _client.BulkAsync(bulkDescriptor);
    }
}

public interface IElasticClientWrapper
{
    IOpenSearchLowLevelClient LowLevel
    {
        get;
    }
    IIndicesNamespaceWrapper Indices
    {
        get;
    }
    Task<BulkResponse> BulkAsync(BulkDescriptor bulkDescriptor);
    Task<DeleteResponse> DeleteAsync(IDeleteRequest deleteRequest);

    Task<IIndexResponseWrapper> IndexAsync<T>(T item, Func<IndexDescriptor<T>, IIndexRequest<T>> func,
        CancellationToken ct = default(CancellationToken)) where T : class;

    Task<IGetResponseWrapper<T>> GetAsync<T>(DocumentPath<T> documentPath,
        Func<GetDescriptor<T>, IGetRequest> func) where T : class;
}

