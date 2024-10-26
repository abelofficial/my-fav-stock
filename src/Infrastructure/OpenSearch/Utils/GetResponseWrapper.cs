using OpenSearch.Client;

namespace Infrastructure.OpenSearch;

public class GetResponseWrapper<TDocument> : IGetResponseWrapper<TDocument> where TDocument : class
{
    private GetResponse<TDocument> _response;

    public GetResponseWrapper(GetResponse<TDocument> response)
    {
        _response = response;
    }

    public FieldValues Fields => _response.Fields;
    public bool Found => _response.Found;
    public string Id => _response.Id;
    public string Index => _response.Index;
    public long? PrimaryTerm => _response.PrimaryTerm;
    public string Routing => _response.Routing;
    public long? SequenceNumber => _response.SequenceNumber;
    public TDocument Source => _response.Source;
    public string Type => _response.Type;
    public long Version => _response.Version;
}