using Application.CQRS;
using Application.Services;

namespace Application.Results;

public class ScrapStockValuesResponse : IResponse
{
    public List<ScrapedData> Result
    {
        get;
        set;
    } = new();
}
