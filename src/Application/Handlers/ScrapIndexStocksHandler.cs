using Application.Contracts;
using Application.Models;
using Application.Queries;
using Application.Results;
using Application.Services;
using Domain.Models;
using MediatR;

namespace Application.Handlers;

public class ScrapIndexStocksHandler : IRequestHandler<ScrapIndexStocksRequest, ScrapIndexStocksResponse>
{
    private readonly ICmsService _cms;
    private readonly ISlickChartsScraper _slickChartsScraper;
    private readonly IYahooFinanceScraper _stockScraper;
    // private readonly IElasticDataConsumer _elasticDataConsumer;
    private string URL = "https://www.slickcharts.com/%SYMBOL%";
    private readonly ICache _cache;
    public ScrapIndexStocksHandler(ICmsService cms, IYahooFinanceScraper stockScraper, ICache cache, ISlickChartsScraper slickChartsScraper)
    {
        _cms = cms;
        _stockScraper = stockScraper;
        _cache = cache;
        _slickChartsScraper = slickChartsScraper;
    }

    public async Task<ScrapIndexStocksResponse> Handle(ScrapIndexStocksRequest request, CancellationToken cancellationToken)
    {
        var response = new ScrapIndexStocksResponse();

        _stockScraper.BaseUrl = URL.Replace("%SYMBOL%", request.Symbol);
        var result = await GetScrapeNewsArticles(request.Symbol);

        response.Result.Add(new IndexItem
        {
            Results = result
        });

        return response;
    }

    private async Task<List<CompanyData>> GetScrapeNewsArticles(string symbol)
    {
        var key = $"CompanyTable-{symbol}";
        var cacheRepository = await _cache.Get<List<CompanyData>>(key);

        if (cacheRepository != null)
            return cacheRepository;

        return await _cache.Add<List<CompanyData>>(key, await _slickChartsScraper.ParseCompanyTableAsync(symbol), 1440);
    }
}