using System.Globalization;
using Application.models;
using Application.Models;
using Domain.Entity;
using Domain.models;
using Domain.Models;
using Domain.Utils;

public class ScrapedData
{
    public string Id
    {
        get => Name + "-" + UpdatedAt.ToString("hh:mm:ss.F", CultureInfo.InvariantCulture);
    }
    public string Name
    {
        get; set;
    }
    public StockType StockType
    {
        get; set;
    }
    public List<NewsArticle> Articles
    {
        get; set;
    }
    public PriceData PriceData
    {
        get; set;
    }

    public DateTime UpdatedAt { get; set; } = DateTime.Now;

    public Stock MapToStockData()
    {
        var stock = new Stock();

        stock.Name = Name;
        stock.CreatedAt = UpdatedAt;
        stock.Articles = Articles;
        if (StockType == StockType.Index)
        {
            stock.IndexDetail = new();
        }
        else
        {
            stock.OptionDetail = new();
        }
        foreach (var item in this.PriceData.Decimals)
        {
            switch (item.Name)
            {
                case Constants.PreviousClose:
                    if (StockType == StockType.Index)
                    {
                        stock.IndexDetail.PreviousClose = item.Value;
                    }
                    else
                    {
                        stock.OptionDetail.PreviousClose = item.Value;
                    }
                    break;
                case Constants.Open:
                    if (StockType == StockType.Index)
                    {
                        stock.IndexDetail.Open = item.Value;
                    }
                    else
                    {
                        stock.OptionDetail.Open = item.Value;
                    }
                    break;
                case Constants.Beta:
                    stock.OptionDetail.Beta = item.Value;
                    break;
                case Constants.PE:
                    stock.OptionDetail.PE = item.Value;
                    break;
                case Constants.EPS:
                    stock.OptionDetail.EPS = item.Value;
                    break;
                case Constants.YearTarget:
                    stock.OptionDetail.YearTarget = item.Value;
                    break;
                default:
                    break;
            }
        }

        foreach (var item in this.PriceData.Numbers)
        {
            switch (item.Name)
            {
                case Constants.Volume:
                    if (StockType == StockType.Index)
                    {
                        stock.IndexDetail.Volume = item.Value;
                    }
                    else
                    {
                        stock.OptionDetail.Volume = item.Value;
                    }
                    break;
                case Constants.AvgVolume:
                    if (StockType == StockType.Index)
                    {
                        stock.IndexDetail.AvgVolume = item.Value;
                    }
                    else
                    {
                        stock.OptionDetail.AvgVolume = item.Value;
                    }
                    break;
                default:
                    break;
            }
        }

        foreach (var item in this.PriceData.Ranges)
        {
            switch (item.Name)
            {
                case Constants.Bid:
                    stock.OptionDetail.Bid = item.Value;
                    break;
                case Constants.Ask:
                    stock.OptionDetail.Ask = item.Value;
                    break;
                case Constants.EarningsDate:
                    stock.OptionDetail.EarningsDate = item.Value;
                    break;
                case Constants.DayRange:
                    if (StockType == StockType.Index)
                    {
                        stock.IndexDetail.DayRange = item.Value;
                    }
                    else
                    {
                        stock.OptionDetail.DayRange = item.Value;
                    }
                    break;
                case Constants.RangeWeek52:
                    if (StockType == StockType.Index)
                    {
                        stock.IndexDetail.RangeWeek52 = item.Value;
                        break;
                    }
                    else
                    {
                        stock.OptionDetail.RangeWeek52 = item.Value;
                    }
                    break;
                default:
                    break;
            }
        }

        foreach (var item in this.PriceData.Dates)
        {
            switch (item.Name)
            {
                case Constants.ExDividendDate:
                    stock.OptionDetail.ExDividendDate = item.Value;
                    break;
                default:
                    break;
            }
        }

        foreach (var item in this.PriceData.Notes)
        {
            switch (item.Name)
            {
                case Constants.MarketCap:
                    stock.OptionDetail.MarketCap = item.Value;
                    break;
                default:
                    break;
            }
        }
        return stock;
    }
}

public class PriceData
{
    public List<NumberScrapField> Numbers
    {
        get; set;
    } = new();
    public List<DecimalScrapField> Decimals
    {
        get; set;
    } = new();
    public List<RangeScrapField> Ranges
    {
        get; set;
    } = new();
    public List<DateScrapField> Dates
    {
        get; set;
    } = new();
    public List<DateRangeScrapField> DateRanges
    {
        get; set;
    } = new();
    public List<TextScrapField> Notes
    {
        get; set;
    } = new();
}

public class ScrapItem
{
    public string Title
    {
        get; set;
    }

    public StockEntryType StockType
    {
        get; set;
    }
    public List<ScrapField> ScrapFields
    {
        get; set;
    }


}

public class StockEntryType
{
    public StockType Value
    {
        get => (StockType)Enum.Parse(typeof(StockType), this.TypeValue);
    }

    public string TypeValue
    {
        get; set;
    }
}

public abstract class BaseScrapField
{

    public string TextValue
    {
        get; set;
    }
    public string Name
    {
        get; set;
    }
}

public class TextScrapField : BaseScrapField
{
    public string Value
    {
        get => base.TextValue;
    }
}

public class NumberScrapField : BaseScrapField
{
    public long Value
    {
        get; set;
    }
}

public class DecimalScrapField : BaseScrapField
{
    public decimal Value
    {
        get; set;
    }
}

public class DateScrapField : BaseScrapField
{
    public DateTime Value
    {
        get => ParseValue();
    }

    public DateTime ParseValue()
    {
        DateTime.TryParse(base.TextValue.Replace(" x ", " - ").Split(" - ").Last(), out var date);
        return date;
    }
}
public class RangeScrapField : BaseScrapField
{
    public Domain.Models.Range Value
    {
        get => ParseValue();
    }

    public Domain.Models.Range ParseValue()
    {
        decimal.TryParse(base.TextValue.Replace(" x ", " - ").Split(" - ").Last(), NumberStyles.AllowThousands | NumberStyles.AllowDecimalPoint, CultureInfo.CreateSpecificCulture("en-GB"), out var high);
        decimal.TryParse(base.TextValue.Replace(" x ", " - ").Split(" - ").First(), NumberStyles.AllowThousands | NumberStyles.AllowDecimalPoint, CultureInfo.CreateSpecificCulture("en-GB"), out var low);
        return new Domain.Models.Range()
        {
            High = high,
            Low = low
        };
    }
}

public class DateRange
{
    public DateTime High
    {
        get; set;
    }
    public DateTime Low
    {
        get; set;
    }
}

public class DateRangeScrapField : BaseScrapField
{
    public DateRange Value
    {
        get => ParseValue();
    }

    public DateRange ParseValue()
    {
        DateTime.TryParse(base.TextValue.Replace(" x ", " - ").Split(" - ").Last(), out var high);
        DateTime.TryParse(base.TextValue.Replace(" x ", " - ").Split(" - ").First(), out var low);
        return new DateRange()
        {
            High = high,
            Low = low
        };
    }
}

public class ScrapField
{
    public FieldType FieldType
    {
        get; set;
    }
    public string Name
    {
        get; set;
    }
}

public enum FieldType
{
    Decimal,
    Number,
    Range,
    Date,
    DateRange,
    Text
}

public class CompanyData
{
    public int Rank
    {
        get; set;
    }
    public string CompanyName
    {
        get; set;
    }
    public string Symbol
    {
        get; set;
    }
    public decimal Weight
    {
        get; set;
    }
    public decimal Price
    {
        get; set;
    }
    public decimal Change
    {
        get; set;
    }
    public decimal PercentageChange
    {
        get; set;
    }
    public DateTime UpdatedAt
    {
        get; set;
    }
}