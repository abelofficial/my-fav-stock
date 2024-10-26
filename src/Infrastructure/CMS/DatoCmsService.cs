using Newtonsoft.Json;
using Serilog;
using Application.Contracts;
using Domain.Models;
using Infrastructure.CMS.Models;
using System.Text;
using Application.Models;

namespace Infrastructure.CMS;

public class DatoCmsService : ICmsService
{
    private readonly HttpClient _httpClient;
    private readonly CMSSettings _settings;
    public DatoCmsService(CMSSettings settings, IHttpClientFactory httpClientFactory)
    {
        _settings = settings;
        _httpClient = httpClientFactory.CreateClient();
    }

    public async Task<List<StockEntryOptions>> GetStockEntries()
    {
        var requestBody = new
        {
            query = @"
                {
                    allStockEntries {
                        id
                        name
                        url
                        stock {
                            name
                            symbol
                        }
                        stockType{
                            typeValue
                        }
                    }
                }"
        };

        var request = new HttpRequestMessage(HttpMethod.Post, _settings.Url);
        request.Content = new StringContent(JsonConvert.SerializeObject(requestBody), Encoding.UTF8, "application/json");
        request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _settings.Token);

        Log.Information("Dato request: {@Request}", request);
        var response = await _httpClient.SendAsync(request);

        if (response.IsSuccessStatusCode)
        {
            var result = await response.Content.ReadAsStringAsync();
            dynamic data = JsonConvert.DeserializeObject<dynamic>(result);
            var stockEntries = data.data.allStockEntries.ToObject<List<StockEntryOptions>>();

            return stockEntries;
        }
        else
        {
            Log.Error("Dato request error: {@Error}", response.Content.ReadAsStringAsync());
            throw new Exception($"Failed to execute query: {response.StatusCode}");
        }
    }

    public async Task<List<ScrapItem>> GetScrapItems()
    {
        var requestBody = new
        {
            query = @"
                {
                    allScrapItems {
                        title
                        stockType {
                            typeValue
                        }
                        scrapFields{
                            fieldType
                            name
                        }
                    }
                }"
        };

        var request = new HttpRequestMessage(HttpMethod.Post, _settings.Url);
        request.Content = new StringContent(JsonConvert.SerializeObject(requestBody), Encoding.UTF8, "application/json");
        request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _settings.Token);

        Log.Information("Dato request: {@Request}", request);
        var response = await _httpClient.SendAsync(request);

        if (response.IsSuccessStatusCode)
        {
            var result = await response.Content.ReadAsStringAsync();
            dynamic data = JsonConvert.DeserializeObject<dynamic>(result);
            var scrapItems = data.data.allScrapItems.ToObject<List<ScrapItem>>();

            return scrapItems;
        }
        else
        {
            Log.Error("Dato request error: {@Error}", response.Content.ReadAsStringAsync());
            throw new Exception($"Failed to execute query: {response.StatusCode}");
        }
    }
}