using System.Threading.Tasks;
using Amazon.Lambda.Core;
using Application.Queries;
using Application.Results;

namespace LambdaFunctions.Functions;

public class StockScraperFunc : BaseFunctions
{

    public StockScraperFunc() : base()
    {

    }

    public async Task<Response<ScrapStockValuesResponse>> ScrapStockValues(ScrapStockValuesRequest request, ILambdaContext context)
    {
        return await HandleResponse(request, context, async (req) =>
        {
            return await _mediator.Send(request);
        });
    }
}

