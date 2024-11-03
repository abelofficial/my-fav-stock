using Application.CQRS;

using ICQRSRequest = MediatR.IRequest<Application.Results.ScrapIndexStocksResponse>;

namespace Application.Queries;

public class ScrapIndexStocksRequest : IRequest, ICQRSRequest
{
    public string Symbol
    {
        get;
        set;
    }
}