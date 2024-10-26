using OpenSearch.Client;
using OpenSearch.Client.Specification.IndicesApi;

namespace Infrastructure.OpenSearch;
public class IndicesNamespaceWrapper : IIndicesNamespaceWrapper
{
    private readonly IndicesNamespace _clientIndices;

    public IndicesNamespaceWrapper(IndicesNamespace indices)
    {
        _clientIndices = indices;
    }

    public async Task<GetIndexResponse> GetAsync(Indices indices)
    {
        return await _clientIndices.GetAsync(indices);
    }
}

public interface IIndicesNamespaceWrapper
{
    Task<GetIndexResponse> GetAsync(Indices indices);
}