using System.Globalization;
using Domain.models;
using Domain.Models;
using Newtonsoft.Json;
using Serilog;

namespace Application.Services;

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

public interface IStockApi
{
    Task<ScrapedData> FetchStockDataAsync(FetchStockDataRequest request);
}

public class StockApi : IStockApi
{
    private readonly HttpClient _httpClient;
    private const string ApiKey = "HcS_n902Hy9OKLIyLYbl6gLP9czOvw03"; // Replace with your actual API key
    private const string StockDataUrl = "https://api.polygon.io/v2/aggs/ticker/{0}/range/{1}/{2}/{3}/{4}?adjusted=true&sort=asc&apiKey={5}";
    private const string NewsUrl = "https://api.polygon.io/v2/reference/news?ticker={0}&apiKey={1}";

    public StockApi()
    {
        _httpClient = new HttpClient();
    }

    public async Task<ScrapedData> FetchStockDataAsync(FetchStockDataRequest request)
    {
        try
        {
            if (string.IsNullOrEmpty(request.Symbol))
            {
                throw new ArgumentException("Symbol must be provided.");
            }

            Log.Information("Fetching price data from stock api for request: {request}", request);

            // Format the date range for the API request
            string from = request.From.ToString("yyyy-MM-dd");
            string to = request.To.ToString("yyyy-MM-dd");

            // Construct the API request URL for stock data
            var stockDataUrl = string.Format(StockDataUrl, request.Symbol, request.Multiplier, request.Interval, from, to, ApiKey);

            // Send the request to the API
            var stockResponse = await _httpClient.GetAsync(stockDataUrl);
            stockResponse.EnsureSuccessStatusCode();

            // Parse the JSON response
            var stockJsonResponse = await stockResponse.Content.ReadAsStringAsync();
            var stockApiResponse = JsonConvert.DeserializeObject<PolygonStockApiResponse>(stockJsonResponse);

            // Map the API response to your PriceData structure
            var priceData = MapToPriceData(stockApiResponse, request.ScrapItems);

            // Fetch news articles
            var articles = await FetchNewsArticlesAsync(request.Symbol);

            return new ScrapedData
            {
                Name = stockApiResponse.Ticker,
                Articles = articles,
                PriceData = priceData
            };
        }
        catch (Exception ex)
        {
            Log.Error($"An error occurred while fetching stock data: {ex.Message}");
            return null;
        }
    }

    private async Task<List<NewsArticle>> FetchNewsArticlesAsync(string symbol)
    {
        try
        {
            Log.Information("Fetching articles from stock api for symbol: {symbol}", symbol);

            // Construct the API request URL for news articles
            var newsUrl = string.Format(NewsUrl, symbol, ApiKey);

            // Send the request to the API
            var newsResponse = await _httpClient.GetAsync(newsUrl);
            newsResponse.EnsureSuccessStatusCode();

            // Parse the JSON response
            var newsJsonResponse = await newsResponse.Content.ReadAsStringAsync();
            var newsApiResponse = JsonConvert.DeserializeObject<PolygonNewsApiResponse>(newsJsonResponse);

            // Map the API response to your NewsArticle structure
            var articles = newsApiResponse.Results.Select(article => new NewsArticle
            {
                Title = article.Title,
                Link = article.ArticleUrl,
                Source = article.Author,
                PublishedTime = article.PublishedUtc.ToString("yyyy-MM-dd HH:mm:ss")
            }).ToList();

            return articles;
        }
        catch (Exception ex)
        {
            Log.Error($"An error occurred while fetching news articles: {ex.Message}");
            return new List<NewsArticle>();
        }
    }

    private PriceData MapToPriceData(PolygonStockApiResponse apiResponse, List<ScrapItem> scrapItems)
    {
        var priceData = new PriceData();
        var fields = scrapItems.SelectMany(x => x.ScrapFields).ToList();

        foreach (var result in apiResponse.Results)
        {
            foreach (var field in fields)
            {
                // Mapping based on the field type and available data
                switch (field.FieldType)
                {
                    case FieldType.Decimal:
                        priceData.Decimals.Add(new DecimalScrapField
                        {
                            Name = field.Name,
                            TextValue = result.Close.ToString(CultureInfo.InvariantCulture),
                            Value = result.Close
                        });
                        break;
                    case FieldType.Number:
                        priceData.Numbers.Add(new NumberScrapField
                        {
                            Name = field.Name,
                            TextValue = result.Volume.ToString(),
                            Value = result.Volume
                        });
                        break;
                    case FieldType.Range:
                        priceData.Ranges.Add(new RangeScrapField
                        {
                            Name = field.Name,
                            TextValue = $"Low: {result.Low.ToString(CultureInfo.InvariantCulture)}, High: {result.High.ToString(CultureInfo.InvariantCulture)}"
                        });
                        break;
                    case FieldType.Date:
                        priceData.Dates.Add(new DateScrapField
                        {
                            Name = field.Name,
                            TextValue = DateTimeOffset.FromUnixTimeMilliseconds(result.Timestamp).UtcDateTime.ToString("yyyy-MM-dd HH:mm:ss")
                        });
                        break;
                    case FieldType.DateRange:
                        // Add logic for DateRange if applicable, potentially using from and to date spans
                        break;
                    default:
                        priceData.Notes.Add(new TextScrapField
                        {
                            Name = field.Name,
                            TextValue = $"Unmapped field: {field.Name}"
                        });
                        break;
                }
            }
        }

        return priceData;
    }
}
