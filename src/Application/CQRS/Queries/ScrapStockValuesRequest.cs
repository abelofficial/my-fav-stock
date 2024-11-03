using Application.CQRS;

using ICQRSRequest = MediatR.IRequest<Application.Results.ScrapStockValuesResponse>;

namespace Application.Queries;

public class ScrapStockValuesRequest : IRequest, ICQRSRequest
{
    public string Symbol
    {
        get;
        set;
    }
}