using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RocketStoreApi.Configurations;
using RocketStoreApi.Configurations.Caching;
using RocketStoreApi.Features.GetCustomersById;
using RocketStoreApi.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace RocketStoreApi.Features.GetCustomers
{
    public record GetCustomerByIdResult(CustomerDetail? Customer);

    public class GetCustomerByIdQuery(Guid id) : IRequest<Result<GetCustomerByIdResult, GetCustomersByIdErrorCodes>>, ICacheable
    {
        public readonly Guid Id = id;

        public string CacheKey => $"Customer_{Id}";

        public int SlidingExpirationInMinutes => 30;
    }

    internal class GetCustomerByIdQueryHandler(ApplicationDbContext context, IHttpClientFactory httpClientFactory, IOptions<AppSettings> appSettings, IPositionStackService positionStackService, ILogger<GetCustomerByIdQueryHandler> logger) : IRequestHandler<GetCustomerByIdQuery, Result<GetCustomerByIdResult, GetCustomersByIdErrorCodes>>
    {
        public async Task<Result<GetCustomerByIdResult, GetCustomersByIdErrorCodes>> Handle(GetCustomerByIdQuery request, CancellationToken cancellationToken)
        {
            var query = context.Customers.AsQueryable();
            query = query.Where(c => c.Id == request.Id);

            var customer = await query.SingleOrDefaultAsync(cancellationToken);

            if (customer is null)
                return Result<GetCustomerByIdResult, GetCustomersByIdErrorCodes>.Failure(GetCustomersByIdErrorCodes.InvalidCustomer, $"The customer with id {request.Id} is invalid");

            PositionStackResponse? positionStackResponse = null;

            // Check if the customer has an address before making the API call
            if (!string.IsNullOrWhiteSpace(customer.Address))
            {
                var httpClient = httpClientFactory.CreateClient();

                var apiUrl = appSettings.Value.PositionStack.Url;
                var accessKey = appSettings.Value.PositionStack.AccessKey;
                var queryParams = $"?access_key={accessKey}&query={Uri.EscapeDataString(customer.Address)}&fields=results.latitude,results.longitude";

                var apiResult = await positionStackService.GetCoordinatesAsync(customer.Address, cancellationToken);

                if (!apiResult.IsSuccess)
                {
                    logger.LogWarning($"Error retrieving geolocalization for customer '{customer.Id}' with address '{customer.Address}'");
                    return Result<GetCustomerByIdResult, GetCustomersByIdErrorCodes>.Failure(
                        GetCustomersByIdErrorCodes.ApiError,
                        apiResult.ErrorDescription
                    );
                }

                positionStackResponse = apiResult.Value;
            }

            return Result<GetCustomerByIdResult, GetCustomersByIdErrorCodes>.Success(new GetCustomerByIdResult(new CustomerDetail
            {
                Address = customer.Address,
                EmailAddress = customer.Email,
                Name = customer.Name,
                Latitude = positionStackResponse?.Data?[0].Latitude,
                Longitude = positionStackResponse?.Data?[0].Longitude
            }));
        }
    }

    #region Position stack object structure

    public record PositionStackResponse
    {
        [JsonProperty("data")]
        public List<Data>? Data { get; set; }
    }

    public record Data
    {
        [JsonProperty("latitude")]
        public double? Latitude { get; set; }

        [JsonProperty("longitude")]
        public double? Longitude { get; set; }

        [JsonProperty("type")]
        public string? Type { get; set; }

        [JsonProperty("name")]
        public string? Name { get; set; }

        [JsonProperty("number")]
        public string? Number { get; set; }

        [JsonProperty("postal_code")]
        public string? PostalCode { get; set; }

        [JsonProperty("street")]
        public string? Street { get; set; }

        [JsonProperty("confidence")]
        public double? Confidence { get; set; }

        [JsonProperty("region")]
        public string? Region { get; set; }

        [JsonProperty("region_code")]
        public string? RegionCode { get; set; }

        [JsonProperty("county")]
        public string? County { get; set; }

        [JsonProperty("locality")]
        public string? Locality { get; set; }

        [JsonProperty("administrative_area")]
        public string? AdministrativeArea { get; set; }

        [JsonProperty("neighbourhood")]
        public string? Neighbourhood { get; set; }

        [JsonProperty("country")]
        public string? Country { get; set; }

        [JsonProperty("country_code")]
        public string? CountryCode { get; set; }

        [JsonProperty("continent")]
        public string? Continent { get; set; }

        [JsonProperty("label")]
        public string? Label { get; set; }

        [JsonProperty("map_url")]
        public string? MapUrl { get; set; }
    }

    #endregion
}
