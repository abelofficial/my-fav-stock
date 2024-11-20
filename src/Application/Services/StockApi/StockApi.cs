using System.Globalization;
using Application.Services.StockApi;
using Domain.models;
using Domain.Models;
using Newtonsoft.Json;
using Serilog;

namespace Application.Services;

public interface IStockApi
{
    Task<ScrapedData> FetchStockDataAsync(Services.StockApi.FetchStockDataRequest request);
}

public class StockApiService : IStockApi
{
    private readonly HttpClient _httpClient;
    private StockApiOptions _options;

    public StockApiService(StockApiOptions options)
    {
        _options = options;
        _httpClient = new HttpClient();
    }

    public async Task<ScrapedData> FetchStockDataAsync(Services.StockApi.FetchStockDataRequest request)
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
            var stockDataUrl = string.Format(_options.StockDataUrl, request.Symbol, request.Multiplier, request.Interval, from, to, _options.ApiKey);

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
            var newsUrl = string.Format(_options.NewsUrl, symbol, _options.ApiKey);

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
