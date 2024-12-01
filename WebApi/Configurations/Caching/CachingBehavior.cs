using MediatR;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace RocketStoreApi.Configurations.Caching
{
    public class CachingBehavior<TRequest, TResponse>(ILogger<CachingBehavior<TRequest, TResponse>> logger, IMemoryCache memoryCache)
        : IPipelineBehavior<TRequest, TResponse> where TRequest : ICacheable
    {
        private readonly IMemoryCache memoryCache = memoryCache;
        private readonly ILogger logger = logger;

        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            // Check if the response is already cached
            if (memoryCache.TryGetValue(request.CacheKey, out TResponse? cachedResponse))
            {
                logger.LogInformation("Fetched from cache with key: {CacheKey}", request.CacheKey);
                return cachedResponse!;
            }

            // Call the next behavior/handler in the pipeline
            var response = await next();

            // Cache only if the response is successful IResult
            if (response is IResult result && result.IsSuccess)
            {
                var cacheOptions = new MemoryCacheEntryOptions
                {
                    SlidingExpiration = TimeSpan.FromMinutes(request.SlidingExpirationInMinutes)
                };

                memoryCache.Set(request.CacheKey, response, cacheOptions);
                logger.LogInformation("Cached response with key: {CacheKey}", request.CacheKey);
            }
            else
            {
                logger.LogWarning("Response with key {CacheKey} not cached as the response was not successful.", request.CacheKey);
            }

            return response;
        }
    }
}