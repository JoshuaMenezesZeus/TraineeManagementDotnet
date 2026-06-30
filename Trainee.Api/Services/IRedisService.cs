namespace Trainee.Api.Services
{
    public interface IRedisService
    {
        Task<T?> GetAsync<T>(string key);
        Task SetAsync <T>(T t, string key);
        Task RemoveAsync (string key);

    }
}