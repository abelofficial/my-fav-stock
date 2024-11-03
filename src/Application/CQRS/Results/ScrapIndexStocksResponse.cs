using Application.CQRS;
using Application.Services;

namespace Application.Results;

public class ScrapIndexStocksResponse : IResponse
{
    public List<IndexItem> Result
    {
        get;
        set;
    } = new();
}


public class IndexItem
{
    public string Name
    {
        get; set;
    }
    public List<CompanyData> Results
    {
        get;
        set;
    } = new();
}
