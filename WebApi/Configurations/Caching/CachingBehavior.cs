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

            var cacheOptions = new MemoryCacheEntryOptions
            {
                SlidingExpiration = TimeSpan.FromMinutes(request.SlidingExpirationInMinutes)
            };

            // Cache the response for future use
            memoryCache.Set(request.CacheKey, response, cacheOptions);

            return response;
        }
    }
}