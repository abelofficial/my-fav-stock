namespace Domain.Utils;

public static class Constants
{
    public const string Volume = "Volume";
    public const string AvgVolume = "Avg. Volume";
    public const string Beta = "Beta";
    public const string PE = "PE Ratio (TTM)";
    public const string EPS = "EPS (TTM)";
    public const string YearTarget = "1y Target Est";
    public const string Bid = "Bid";
    public const string Ask = "Ask";
    public const string EarningsDate = "Earnings Date";
    public const string ExDividendDate = "Ex-Dividend Date";
    public const string MarketCap = "Market Cap (intraday)";
    public const string PreviousClose = "Previous Close";
    public const string RangeWeek52 = "52 Week Range";
    public const string Open = "Open";
    public const string DayRange = "Day's Range";

    public static class ArticleScrapFields
    {
        public const string Layer1 = "//section[contains(@class, 'stream-items')]";
        public const string Layer2 = ".//div[contains(@class, 'stream-item')]";
        public const string Layer3 = ".//section[contains(@class, 'container')]";
        public const string Layer4 = ".//h3";
        public const string Layer5 = ".//a[contains(@class, 'subtle-link fin-size-small thumb')]";
        public const string Layer6 = ".//div[contains(@class, 'footer')]";
        public const string Layer7 = ".//div[contains(@class, 'publishing')]";
    }

    public static class PriceDataScrapFields
    {
        public const string Layer1 = "//div[@data-testid='quote-statistics']//li";
        public const string Layer2 = ".//span[1]";
        public const string Layer3 = ".//span[2]";
        public const string Layer4 = ".//fin-streamer";
        public const string Layer5 = ".//fin-streamer";
    }
}














