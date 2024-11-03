using Application.Contracts;
using Application.models;
using Domain.models;
using Domain.Models;
using Domain.Utils;
using HtmlAgilityPack;
using Serilog;

namespace Application.Services;

public interface IYahooFinanceScraper
{
    public string BaseUrl
    {
        get;
        set;
    }

    Task<Dictionary<string, string>> ScrapeQuoteStatisticsAsync();
    Task<ScrapedData> ScrapeNewsArticlesAsync(StockType value, List<ScrapItem> scrapItems);
}
public class YahooFinanceScraper : IYahooFinanceScraper
{
    private readonly HttpClient _httpClient;
    private readonly ICmsService _cmsService;
    private readonly StockScraperOptions _stockScraperOptions;
    public string BaseUrl
    {
        get; set;
    }


    public YahooFinanceScraper(StockScraperOptions stockScraperOptions, ICmsService cmsService)
    {
        _cmsService = cmsService;
        _httpClient = new HttpClient(new HttpClientHandler
        {
            AutomaticDecompression = System.Net.DecompressionMethods.All
        });
        _stockScraperOptions = stockScraperOptions;
    }

    public async Task<ScrapedData> ScrapeNewsArticlesAsync(StockType stockType, List<ScrapItem> scrapItems)
    {

        try
        {
            var fields = scrapItems.Where(x => x.StockType.Value == stockType).SelectMany(x => x.ScrapFields);
            var html = await _httpClient.GetStringAsync(BaseUrl);
            var htmlDocument = new HtmlDocument();
            htmlDocument.LoadHtml(html);
            var scrapedPriceData = ScrapForStockData(htmlDocument);
            var priceData = ScrapParser.Parse(scrapedPriceData, fields.ToList());

            return new ScrapedData()
            {
                Articles = ScrapNewArticles(htmlDocument),
                PriceData = priceData
            };
        }
        catch (Exception ex)
        {
            Log.Error($"An error occurred while scraping news articles: {ex.Message}");
            return null;
        }
    }

    public async Task<Dictionary<string, string>> ScrapeQuoteStatisticsAsync()
    {
        try
        {
            Log.Information($"Scraping data for url: {_stockScraperOptions.Url}");
            var html = await _httpClient.GetStringAsync(_stockScraperOptions.Url);
            var htmlDocument = new HtmlDocument();
            htmlDocument.LoadHtml(html);

            return ScrapForStockData(htmlDocument);
        }
        catch (Exception ex)
        {
            Log.Error($"An error occurred while scraping quote statistics: {ex.Message}");
            return null;
        }
    }

    private List<NewsArticle> ScrapNewArticles(HtmlDocument htmlDocument)
    {
        var articles = new List<NewsArticle>();
        var newsSectionNode = htmlDocument.DocumentNode.SelectSingleNode(Constants.ArticleScrapFields.Layer1);

        if (newsSectionNode != null)
        {
            var streamItems = newsSectionNode.SelectNodes(Constants.ArticleScrapFields.Layer2);

            if (streamItems != null)
            {
                var exitCounter = 0;
                foreach (var item in streamItems)
                {
                    if (exitCounter++ == 4)
                        break;

                    var articleNode = item.SelectSingleNode(Constants.ArticleScrapFields.Layer3);

                    if (articleNode != null)
                    {

                        var titleNode = articleNode.SelectSingleNode(Constants.ArticleScrapFields.Layer4);
                        var linkNode = articleNode.SelectSingleNode(Constants.ArticleScrapFields.Layer5);
                        var footerNode = articleNode.SelectSingleNode(Constants.ArticleScrapFields.Layer6);
                        var sourceNode = footerNode.SelectSingleNode(Constants.ArticleScrapFields.Layer7);

                        var newsArticle = new NewsArticle
                        {
                            Title = titleNode?.InnerText.Trim(),
                            Link = linkNode?.GetAttributeValue("href", ""),
                            Source = sourceNode?.InnerText.Trim(),
                            PublishedTime = sourceNode?.InnerText.Trim().Split('•').Last().Trim()
                        };

                        articles.Add(newsArticle);
                    }
                }
            }
        }

        return articles;
    }

    private Dictionary<string, string> ScrapForStockData(HtmlDocument htmlDocument)
    {

        var quoteStatistics = new Dictionary<string, string>();
        var statNodes = htmlDocument.DocumentNode.SelectNodes(Constants.PriceDataScrapFields.Layer1);

        if (statNodes != null)
        {
            foreach (var node in statNodes)
            {
                var labelNode = node.SelectSingleNode(Constants.PriceDataScrapFields.Layer2);
                var valueNode = node.SelectSingleNode(Constants.PriceDataScrapFields.Layer3);

                if (labelNode != null && valueNode != null)
                {
                    var label = labelNode.InnerText.Trim();
                    var value = valueNode.SelectSingleNode(Constants.PriceDataScrapFields.Layer4) != null
                        ? valueNode.SelectSingleNode(Constants.PriceDataScrapFields.Layer5).InnerText.Trim()
                        : valueNode.InnerText.Trim();

                    quoteStatistics[label] = value;
                }
            }
        }

        return quoteStatistics;
    }
}
