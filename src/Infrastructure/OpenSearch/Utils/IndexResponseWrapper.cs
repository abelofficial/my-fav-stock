using OpenSearch.Client;

namespace Infrastructure.OpenSearch;

public class IndexResponseWrapper : IIndexResponseWrapper
{
    private readonly IndexResponse _response;

    public IndexResponseWrapper(IndexResponse response)
    {
        _response = response;
    }

    public Result Result => _response.Result;
    public bool IsValid => _response.IsValid;
    public string DebugInformation => _response.DebugInformation;
}