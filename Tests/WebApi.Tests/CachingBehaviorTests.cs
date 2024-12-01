using AutoFixture;
using MediatR;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Moq;
using RocketStoreApi.Configurations;
using RocketStoreApi.Configurations.Caching;
using RocketStoreApi.Features.GetCustomers;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace RocketStoreApi.Tests
{
    [Collection("CustomersAPI")]
    public class CachingBehaviorTests
    {
        private readonly Mock<ILogger<CachingBehavior<GetCustomerByIdQuery, Result<GetCustomerByIdResult, GetCustomersByIdErrorCodes>>>> mockLogger;
        private readonly Fixture specimenBuilders;

        public CachingBehaviorTests()
        {
            mockLogger = new Mock<ILogger<CachingBehavior<GetCustomerByIdQuery, Result<GetCustomerByIdResult, GetCustomersByIdErrorCodes>>>>();
            specimenBuilders = new();
        }

        [Fact]
        public async Task ReturnsCachedResponseIfInCache()
        {
            // Arrange
            Guid customerId = specimenBuilders.Create<Guid>();
            var cacheKey = $"Customer_{customerId}";

            var cachedResponse = Result<GetCustomerByIdResult, GetCustomersByIdErrorCodes>.Success(new GetCustomerByIdResult(specimenBuilders.Create<CustomerDetail>()));
            using var memoryCache = CreateMemoryCacheWithEntry(cacheKey, cachedResponse);
            var request = new GetCustomerByIdQuery(customerId);

            var behavior = new CachingBehavior<GetCustomerByIdQuery, Result<GetCustomerByIdResult, GetCustomersByIdErrorCodes>>(mockLogger.Object, memoryCache);
            var nextDelegate = new Mock<RequestHandlerDelegate<Result<GetCustomerByIdResult, GetCustomersByIdErrorCodes>>>().Object;

            // Act
            var response = await behavior.Handle(request, nextDelegate, CancellationToken.None);

            // Assert
            Assert.Equal(cachedResponse, response);
        }

        [Fact]
        public async Task CallsNextAndCachesResponseIfNotCached()
        {
            // Arrange
            var request = new GetCustomerByIdQuery(It.IsAny<Guid>());
            var generatedResponse = Result<GetCustomerByIdResult, GetCustomersByIdErrorCodes>.Success(new GetCustomerByIdResult(specimenBuilders.Create<CustomerDetail>()));
            // Empty cache
            using var memoryCache = new MemoryCache(new MemoryCacheOptions());

            var behavior = new CachingBehavior<GetCustomerByIdQuery, Result<GetCustomerByIdResult, GetCustomersByIdErrorCodes>>(mockLogger.Object, memoryCache);

            // Need a real delegate as Moq doesn't support constructor arguments passed for delegate mocks methods
            Task<Result<GetCustomerByIdResult, GetCustomersByIdErrorCodes>> nextDelegateAsync() => Task.FromResult(generatedResponse);

            // Act
            var response = await behavior.Handle(request, nextDelegateAsync, CancellationToken.None);

            // Assert
            Assert.Equal(generatedResponse, response);
        }

        private static MemoryCache CreateMemoryCacheWithEntry<T>(string key, T value)
        {
            var memoryCache = new MemoryCache(new MemoryCacheOptions());
            memoryCache.Set(key, value);
            return memoryCache;
        }
    }
}
