using Application.CQRS;
using Application.Services;
using Domain.Entity;

namespace Application.Results;

public class ScrapStockValuesResponse : IResponse
{
    public List<Stock> Result
    {
        get;
        set;
    } = new();
}
