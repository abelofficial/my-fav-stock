namespace Application.Contracts;

public interface ICache
{
    Task<T> Get<T>(string key);
    Task<T> Add<T>(string key, T value, int expireAfterInMin = 60, string tag = "range_key");
}