using System;
using System.Text.Json.Serialization;
using Domain.Models;

namespace Application.Models;

public class StockEntryOptions
{
    public string Id { get; set; }
    public string Name { get; set; }
    public string FullName { get => this.Stock.Name + $" ({this.Stock.Symbol})"; }
    public Uri Url { get; set; }
    public StockInfo Stock { get; set; }
    public StockEntryType StockType { get; set; }
}

public class StockEntryType
{
    public StockType Value { get => (StockType)Enum.Parse(typeof(StockType), this.TypeValue); }

    public string TypeValue { get; set; }
}