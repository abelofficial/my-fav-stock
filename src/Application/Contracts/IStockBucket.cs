using Domain.Entity;

namespace Application.Contracts;

public interface IStockBucket
{
    Task AddStockEntry(Stock request);
}