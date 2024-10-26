using Application.Contracts;
using Application.Queries;
using Application.Results;
using Application.Services;
using MediatR;

namespace Application.Handlers;

public class ScrapStockValuesHandler : IRequestHandler<ScrapStockValuesRequest, ScrapStockValuesResponse>
{
    private readonly ICmsService _cms;
    private readonly IStockScraper _stockScraper;
    // private readonly IElasticDataConsumer _elasticDataConsumer;
    private readonly IStockBucket _bucket;
    public ScrapStockValuesHandler(ICmsService cms, IStockScraper stockScraper, IStockBucket bucket)
    {
        _cms = cms;
        _stockScraper = stockScraper;
        _bucket = bucket;
    }

    public async Task<ScrapStockValuesResponse> Handle(ScrapStockValuesRequest request, CancellationToken cancellationToken)
    {
        var stockEntries = await _cms.GetStockEntries();
        var scrapItems = await _cms.GetScrapItems();
        var response = new ScrapStockValuesResponse();

        foreach (var stockEntry in stockEntries)
        {
            _stockScraper.BaseUrl = stockEntry.Url.ToString();
            var result = await _stockScraper.ScrapeNewsArticlesAsync(stockEntry.StockType.Value, scrapItems);
            result.Name = stockEntry.FullName;
            result.StockType = stockEntry.StockType.Value;
            response.Result.Add(result);
            var temp = result.MapToStockData();
            await _bucket.AddStockEntry(temp);

        }

        return response;
    }
}