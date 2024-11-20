using Newtonsoft.Json;

namespace Application.Services.StockApi;

public class StockApiOptions
{
    public string ApiKey
    {
        get; set;
    }

    public string StockDataUrl
    {
        get; set;
    }

    public string NewsUrl
    {
        get; set;
    }
}

public class PolygonApiResponse
{
    [JsonProperty("ticker")]
    public string Ticker
    {
        get; set;
    }

    [JsonProperty("results")]
    public List<PolygonApiResult> Results
    {
        get; set;
    }
}
public class PolygonApiResult
{
    [JsonProperty("c")]
    public decimal Close
    {
        get; set;
    }

    [JsonProperty("h")]
    public decimal High
    {
        get; set;
    }

    [JsonProperty("l")]
    public decimal Low
    {
        get; set;
    }

    [JsonProperty("o")]
    public decimal Open
    {
        get; set;
    }

    [JsonProperty("v")]
    public long Volume
    {
        get; set;
    }

    [JsonProperty("t")]
    public long Timestamp
    {
        get; set;
    }
}

public class PolygonStockApiResponse
{
    [JsonProperty("ticker")]
    public string Ticker
    {
        get; set;
    }

    [JsonProperty("results")]
    public List<PolygonStockResult> Results
    {
        get; set;
    }
}

public class PolygonStockResult
{
    [JsonProperty("c")]
    public decimal Close
    {
        get; set;
    }

    [JsonProperty("h")]
    public decimal High
    {
        get; set;
    }

    [JsonProperty("l")]
    public decimal Low
    {
        get; set;
    }

    [JsonProperty("o")]
    public decimal Open
    {
        get; set;
    }

    [JsonProperty("v")]
    public long Volume
    {
        get; set;
    }

    [JsonProperty("t")]
    public long Timestamp
    {
        get; set;
    }
}

public class PolygonNewsApiResponse
{
    [JsonProperty("results")]
    public List<PolygonNewsArticle> Results
    {
        get; set;
    }
}

public class PolygonNewsArticle
{
    [JsonProperty("title")]
    public string Title
    {
        get; set;
    }

    [JsonProperty("author")]
    public string Author
    {
        get; set;
    }

    [JsonProperty("published_utc")]
    public DateTime PublishedUtc
    {
        get; set;
    }

    [JsonProperty("article_url")]
    public string ArticleUrl
    {
        get; set;
    }
}

public class FetchStockDataRequest
{
    public string Symbol
    {
        get; set;
    } // Ticker symbol for the API request
    public List<ScrapItem> ScrapItems
    {
        get; set;
    }
    public string Interval { get; set; } = "hour"; // Default interval
    public int Multiplier { get; set; } = 1; // Default multiplier
    public DateTime From { get; set; } = DateTime.UtcNow.AddHours(-24); // Default to 24 hours ago
    public DateTime To { get; set; } = DateTime.UtcNow; // Default to current time
}
