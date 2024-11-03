using System.Globalization;
using Application.Contracts;
using Application.models;
using HtmlAgilityPack;
using Serilog;

namespace Application.Services;

public interface ISlickChartsScraper
{
    Task<List<CompanyData>> ParseCompanyTableAsync(string symbol);
}
public class SlickChartsScraper : ISlickChartsScraper
{

    private readonly HttpClient _httpClient;
    private readonly ICmsService _cmsService;
    private readonly StockScraperOptions _stockScraperOptions;
    public string BaseUrl
    {
        get; set;
    } = "https://www.slickcharts.com/%SYMBOL%";


    public SlickChartsScraper(StockScraperOptions stockScraperOptions, ICmsService cmsService)
    {
        _cmsService = cmsService;
        _httpClient = new HttpClient();
        _stockScraperOptions = stockScraperOptions;
    }

    public async Task<List<CompanyData>> ParseCompanyTableAsync(string symbol)
    {
        try
        {
            BaseUrl = BaseUrl.Replace("%SYMBOL%", symbol);

            // Configure HttpClient with necessary headers
            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.BaseAddress = new Uri(BaseUrl);
            _httpClient.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/58.0.3029.110 Safari/537.3");
            _httpClient.DefaultRequestHeaders.Add("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8");
            _httpClient.DefaultRequestHeaders.Add("Connection", "keep-alive");

            var html = await _httpClient.GetStringAsync(BaseUrl);
            var htmlDocument = new HtmlDocument();
            htmlDocument.LoadHtml(html);

            var companyTable = htmlDocument.DocumentNode.SelectSingleNode("//table[contains(@class, 'table-hover')]");
            var rows = companyTable.SelectNodes(".//tbody/tr");
            var companyDataList = new List<CompanyData>();

            foreach (var row in rows)
            {
                var cells = row.SelectNodes("td");
                if (cells == null || cells.Count < 7)
                    continue;

                var companyData = new CompanyData();

                if (int.TryParse(cells[0].InnerText.Trim(), out int rank))
                {
                    companyData.Rank = rank;
                }

                companyData.CompanyName = cells[1].SelectSingleNode(".//a").InnerText.Trim();
                companyData.Symbol = cells[2].SelectSingleNode(".//a").InnerText.Trim();

                if (decimal.TryParse(cells[3].InnerText.TrimEnd('%'), NumberStyles.Any, CultureInfo.InvariantCulture, out decimal weight))
                {
                    companyData.Weight = weight / 100; // Convert percentage string to decimal
                }

                var priceText = cells[4].InnerText.Replace("&nbsp;", "").Trim();
                if (decimal.TryParse(priceText, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal price))
                {
                    companyData.Price = price;
                }

                var changeText = cells[5].InnerText.Trim();
                if (decimal.TryParse(changeText, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal change))
                {
                    companyData.Change = change;
                }

                var percentageChangeText = cells[6].InnerText.Trim('(', ')', '%');
                if (decimal.TryParse(percentageChangeText, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal percentageChange))
                {
                    companyData.PercentageChange = percentageChange;
                }

                companyDataList.Add(companyData);
            }

            return companyDataList;
        }
        catch (Exception ex)
        {
            Log.Error($"An error occurred while parsing the company table: {ex.Message}");
            return null;
        }
    }

}


