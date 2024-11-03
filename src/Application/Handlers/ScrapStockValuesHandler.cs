using Application.Contracts;
using Application.Queries;
using Application.Results;
using Application.Services;
using Domain.Models;
using MediatR;

namespace Application.Handlers;

public class ScrapStockValuesHandler : IRequestHandler<ScrapStockValuesRequest, ScrapStockValuesResponse>
{
    private readonly ICmsService _cms;
    private readonly IYahooFinanceScraper _stockScraper;
    private readonly IStockApi _stockApi;
    private readonly ICache _cache;
    public ScrapStockValuesHandler(ICmsService cms, IYahooFinanceScraper stockScraper, ICache cache, IStockApi stockApi)
    {
        _cms = cms;
        _stockScraper = stockScraper;
        _cache = cache;
        _stockApi = stockApi;
    }

    public async Task<ScrapStockValuesResponse> Handle(ScrapStockValuesRequest request, CancellationToken cancellationToken)
    {
        var scrapItems = await GetScrapItems();
        var response = new ScrapStockValuesResponse();

        var result = await GetScrapeNewsArticles(new FetchStockDataRequest()
        {
            ScrapItems = scrapItems,
            Symbol = request.Symbol,
            From = DateTime.Now.AddHours(-48),
            To = DateTime.Now.AddHours(-24),
        });
        result.Name = result.Name;
        result.StockType = StockType.Option;
        response.Result.Add(result.MapToStockData());

        return response;
    }

    private async Task<List<ScrapItem>> GetScrapItems()
    {
        var key = "ScrapItem";
        var cacheRepository = await _cache.Get<List<ScrapItem>>(key);

        if (cacheRepository != null)
            return cacheRepository;

        return await _cache.Add<List<ScrapItem>>(key, await _cms.GetScrapItems(), 1440);
    }

    private async Task<ScrapedData> GetScrapeNewsArticles(FetchStockDataRequest request)
    {
        var key = $"ScrapedData-{request.Symbol}";
        var cacheRepository = await _cache.Get<ScrapedData>(key);

        if (cacheRepository != null)
            return cacheRepository;

        return await _cache.Add<ScrapedData>(key, await _stockApi.FetchStockDataAsync(request), 1440);
    }
}