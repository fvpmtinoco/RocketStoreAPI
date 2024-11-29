using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RocketStoreApi.Configurations;
using RocketStoreApi.CQRS;
using RocketStoreApi.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace RocketStoreApi.Features.GetCustomers
{
    public record GetCustomerByIdResult(CustomerAddressDetail? Customer);

    public class GetCustomerByIdQuery(Guid id) : IQuery<GetCustomerByIdResult>
    {
        public readonly Guid Id = id;
    }

    public class GetCustomerByIdQueryHandler(ApplicationDbContext context, IHttpClientFactory httpClientFactory, IOptions<AppSettings> appSettings) : IQueryHandler<GetCustomerByIdQuery, GetCustomerByIdResult>
    {
        public async Task<GetCustomerByIdResult> Handle(GetCustomerByIdQuery request, CancellationToken cancellationToken)
        {
            var httpClient = httpClientFactory.CreateClient();
            var query = context.Customers.AsQueryable();

            query = query.Where(c => c.Id == request.Id);

            var customer = await query.SingleOrDefaultAsync(cancellationToken);

            if (customer is null)
                throw new Exception("To implement error handling in a graceful manner");

            PositionStackResponse? positionStackResponse = null;

            // Check if the customer has an address before making the API call
            if (!string.IsNullOrWhiteSpace(customer.Address))
            {
                var apiUrl = appSettings.Value.PositionStack.Url;
                var accessKey = appSettings.Value.PositionStack.AccessKey;
                var queryParams = $"?access_key={accessKey}&query={Uri.EscapeDataString(customer.Address)}&fields=results.latitude,results.longitude";

                // Call the PositionStack API
                var response = await httpClient.GetStringAsync(apiUrl + queryParams);

                // Deserialize the response as dynamic as the format is not known to be  to get the desired data
                positionStackResponse = JsonConvert.DeserializeObject<PositionStackResponse>(response);
            }

            return new GetCustomerByIdResult(new CustomerAddressDetail
            {
                Address = customer.Address,
                EmailAddress = customer.Email,
                Name = customer.Name,
                Latitude = positionStackResponse?.Data?[0].Latitude,
                Longitude = positionStackResponse?.Data?[0].Longitude
            });
        }
    }

    public partial class CustomerAddressDetail : SharedModels.Customer
    {
        public double? Latitude { get; set; } = default!;
        public double? Longitude { get; set; } = default!;
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
