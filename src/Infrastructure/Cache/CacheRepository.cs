
using System.Net;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using Application.Contracts;
using Newtonsoft.Json;

namespace Infrastructure.Cache;

public class CacheRepository : ICache
{
    private readonly IAmazonDynamoDB _db;

    public CacheRepository(IAmazonDynamoDB db)
    {
        _db = db;
    }

    public async Task<T> Add<T>(string key, T value, int expireAfterInMin = 60, string tag = "range_key")
    {
        var item = new Dictionary<string, AttributeValue>
        {
            ["CacheKey"] = new AttributeValue { S = key },
            ["ExpireAt"] = new AttributeValue { S = DateTime.UtcNow.AddMinutes(expireAfterInMin).ToString() },
            ["RangeKey"] = new AttributeValue { S = tag },
            ["JsonValue"] = new AttributeValue { S = JsonConvert.SerializeObject(value) },
        };

        var request = new PutItemRequest
        {
            TableName = "my-fav-stocks-cache",
            Item = item
        };

        var temp = await _db.PutItemAsync(request);

        if (temp.HttpStatusCode != HttpStatusCode.OK)
            return default;

        return value;
    }

    public async Task<T> Get<T>(string key)
    {
        var keyAttribute = new Dictionary<string, AttributeValue>
        {
            ["CacheKey"] = new AttributeValue { S = key },
        };

        var request = new GetItemRequest
        {
            Key = keyAttribute,
            TableName = "my-fav-stocks-cache",
        };

        var response = await _db.GetItemAsync(request);

        if (response.Item.Count() == 0)
            return default;
        var settings = new JsonSerializerSettings { Error = (se, ev) => { ev.ErrorContext.Handled = true; } };
        return JsonConvert.DeserializeObject<T>(response.Item["JsonValue"].S, settings);
    }
}