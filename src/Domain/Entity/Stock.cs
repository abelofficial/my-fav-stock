using System.Globalization;

namespace Domain.Entity;

public class Stock
{
    public string Id
    {
        get => this.Name.Replace(" ", "-").Replace("(", "").Replace(")", "").ToLower() + "-" + CreatedAt.ToString("hh:mm:ss.ff", CultureInfo.InvariantCulture);
    }
    public string Name
    {
        get; set;
    }
    public string Description
    {
        get; set;
    }
    public DateTime CreatedAt
    {
        get; set;
    }

    public OptionStockDetail OptionDetail
    {
        get; set;
    }
    public IndexStockDetail IndexDetail
    {
        get; set;
    }
}

public class OptionStockDetail
{
    public long Volume
    {
        get; set;
    }
    public long AvgVolume
    {
        get; set;
    }
    public decimal PreviousClose
    {
        get; set;
    }
    public decimal Open
    {
        get; set;
    }
    public decimal Beta
    {
        get; set;
    }
    public decimal PE
    {
        get; set;
    }
    public decimal EPS
    {
        get; set;
    }

    public decimal YearTarget
    {
        get; set;
    }

    public Domain.Models.Range Bid
    {
        get; set;
    }

    public Domain.Models.Range Ask
    {
        get; set;
    }

    public Domain.Models.Range DayRange
    {
        get; set;
    }

    public Domain.Models.Range EarningsDate
    {
        get; set;
    }

    public DateTime ExDividendDate
    {
        get; set;
    }

    public string MarketCap
    {
        get; set;
    }

    public Domain.Models.Range RangeWeek52
    {
        get; set;
    }
}

public class IndexStockDetail
{
    public long Volume
    {
        get; set;
    }
    public long AvgVolume
    {
        get; set;
    }
    public decimal PreviousClose
    {
        get; set;
    }
    public decimal Open
    {
        get; set;
    }
    public Domain.Models.Range DayRange
    {
        get; set;
    }

    public Domain.Models.Range RangeWeek52
    {
        get; set;
    }
}