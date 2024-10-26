using Application.Models;
using Domain.Models;

namespace Infrastructure.CMS.Models;
public class GetStockEntriesResponse
{
    public IEnumerable<StockEntryOptions> AllStockEntries
    {
        get; set;
    }
}