using System.Threading.Tasks;
using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using Application.Queries;
using Application.Results;

namespace LambdaFunctions.Functions;

public class StockScraperFunc : BaseFunctions
{

    public StockScraperFunc() : base()
    {

    }

    public async Task<Response<ScrapIndexStocksResponse>> ScrapIndexStocks(APIGatewayHttpApiV2ProxyRequest request, ILambdaContext context)
    {
        return await HandleResponse<ScrapIndexStocksRequest, ScrapIndexStocksResponse>(request, context, async (req) =>
        {
            return await _mediator.Send<ScrapIndexStocksResponse>(new ScrapIndexStocksRequest { Symbol = req });
        });
    }

    public async Task<Response<ScrapStockValuesResponse>> ScrapStockValues(APIGatewayHttpApiV2ProxyRequest request, ILambdaContext context)
    {
        return await HandleResponse<ScrapStockValuesRequest, ScrapStockValuesResponse>(request, context, async (req) =>
        {
            return await _mediator.Send<ScrapStockValuesResponse>(new ScrapStockValuesRequest { Symbol = req });
        });
    }
}

