namespace RocketStoreApi.Configurations.Caching
{
    public interface ICacheable
    {
        string CacheKey { get; }
        int SlidingExpirationInMinutes { get; }
    }
}
