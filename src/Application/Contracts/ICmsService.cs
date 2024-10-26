using Application.Models;
using Domain.Models;

namespace Application.Contracts;
public interface ICmsService
{
    Task<List<StockEntryOptions>> GetStockEntries();
    Task<List<ScrapItem>> GetScrapItems();
}