using System.Globalization;
using Application.Services;
using Domain.Models;

public static class ScrapParser
{
    public static PriceData Parse(Dictionary<string, string> priceItems, List<ScrapField> targetFields)
    {

        var priceData = new PriceData();
        foreach (var price in priceItems)
        {
            var priceField = targetFields.FirstOrDefault(x => x.Name == price.Key);

            if (priceField == null || string.IsNullOrEmpty(price.Key) || string.IsNullOrEmpty(price.Value))
                continue;

            switch (priceField.FieldType)
            {
                case FieldType.Decimal:
                    var isParsed = decimal.TryParse(price.Value, NumberStyles.AllowThousands | NumberStyles.AllowDecimalPoint, CultureInfo.CreateSpecificCulture("en-GB"), out var decimalValue);
                    priceData.Decimals.Add(new DecimalScrapField()
                    {
                        Name = price.Key,
                        TextValue = price.Value,
                        Value = isParsed ? decimalValue : new decimal(0),
                    });
                    break;
                case FieldType.Number:
                    isParsed = long.TryParse(price.Value, NumberStyles.AllowThousands | NumberStyles.AllowDecimalPoint, CultureInfo.CreateSpecificCulture("en-GB"), out var intValue);
                    priceData.Numbers.Add(new NumberScrapField()
                    {
                        Name = price.Key,
                        TextValue = price.Value,
                        Value = isParsed ? intValue : 0,
                    });
                    break;
                case FieldType.Range:
                    priceData.Ranges.Add(new RangeScrapField()
                    {
                        Name = price.Key,
                        TextValue = price.Value
                    });
                    break;
                case FieldType.Date:
                    priceData.Dates.Add(new DateScrapField()
                    {
                        Name = price.Key,
                        TextValue = price.Value
                    });
                    break;
                case FieldType.DateRange:
                    priceData.DateRanges.Add(new DateRangeScrapField()
                    {
                        Name = price.Key,
                        TextValue = price.Value
                    });
                    break;
                default:
                    priceData.Notes.Add(new TextScrapField()
                    {
                        Name = price.Key,
                        TextValue = price.Value
                    });
                    break;
            }
        }

        return priceData;
    }
}