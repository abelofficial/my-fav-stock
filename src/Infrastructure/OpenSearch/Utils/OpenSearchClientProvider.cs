using OpenSearch.Client;
using OpenSearch.Net;

namespace Infrastructure.OpenSearch;

public class OpenSearchClientProvider : IOpenSearchClientProvider
{
    public OpenSearchClientProvider()
    {
    }
    public IOpenSearchClient CreateClient()
    {
        var endpoint = "";
        if (!endpoint.ToLowerInvariant().StartsWith("http"))
        {
            endpoint = $"ENDPOINT";
        }

        var nodes = new Uri[] { new Uri(endpoint) };
        var pool = new StaticConnectionPool(nodes);
        var settings = new ConnectionSettings(pool).BasicAuthentication("USERNAME", "PASSWORD");
        settings.IncludeServerStackTraceOnError();
        settings.DisableDirectStreaming();
        settings.DefaultIndex("INDEX_NAME");
        var client = new OpenSearchClient(settings);
        return client;
    }
}

public class OpenSearchOptions
{
    public string UserName
    {
        get;
        internal set;
    }

    public string Password
    {
        get;
        internal set;
    }
    public string Endpoint
    {
        get;
        internal set;
    }
    public object MappingFieldSize
    {
        get;
        internal set;
    } = 200;
    public int? Replicas
    {
        get;
        internal set;
    } = 1;
    public int? Shards
    {
        get;
        internal set;
    } = 5;
}