using OpenSearch.Client;

namespace Infrastructure.OpenSearch;

public interface IGetResponseWrapper<TDocument>
{
    FieldValues Fields
    {
        get;
    }
    bool Found
    {
        get;
    }
    string Id
    {
        get;
    }
    string Index
    {
        get;
    }
    long? PrimaryTerm
    {
        get;
    }
    string Routing
    {
        get;
    }
    long? SequenceNumber
    {
        get;
    }
    TDocument Source
    {
        get;
    }
    string Type
    {
        get;
    }
    long Version
    {
        get;
    }
}

public interface IIndexResponseWrapper
{
    Result Result
    {
        get;
    }
    bool IsValid
    {
        get;
    }
    string DebugInformation
    {
        get;
    }
}

public interface IOpenSearchClientProvider
{
    IOpenSearchClient CreateClient();
}



public interface IIndicesNamespaceWrapper<TIndices>
{
    Task<GetIndexResponse> GetAsync(TIndices indices);
}