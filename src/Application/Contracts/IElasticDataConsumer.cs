using Application.Services;
using Domain.Entity;

namespace Application.Contracts;
public interface IElasticDataConsumer
{

    Task ConsumeData(Stock data);
    Task DeleteFromAllIndices(string key);
    Task UpdateBulkStockEntry(List<Stock> dataList);
}