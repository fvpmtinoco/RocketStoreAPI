using AutoFixture;
using RestSharp;
using RocketStoreApi.Features.CreateCustomer;
using RocketStoreApi.Features.GetCustomers;
using RocketStoreApi.SharedModels;
using System;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace RocketStoreApi.Tests.Configurations
{
    /// <summary>
    /// Defines the base class for test classes.
    /// </summary>
    public abstract partial class TestsBase(CustomersFixture fixture)
    {
        #region Public Methods

        /// <summary>
        /// Gets the content from the specified response.
        /// </summary>
        /// <typeparam name="T">The content response.</typeparam>
        /// <param name="response">The response.</param>
        /// <returns>
        /// The <see cref="Task{TResult}"/> that represents the asynchronous operation.
        /// The <typeparamref name="T"/> instance.
        /// </returns>
        public virtual async Task<T> GetResponseContentAsync<T>(RestResponse response)
        {
            // Check if the response is null
            if (response == null) return default;

            // Convert the response content to a stream
            using var stream = new MemoryStream(Encoding.UTF8.GetBytes(response.Content));
            {
                // Use the async DeserializeAsync method to deserialize the JSON content
                return await JsonSerializer.DeserializeAsync<T>(stream).ConfigureAwait(false);
            }
        }

        #endregion

        public async Task<CreateCustomerResponse> CreateCustomerAsync(string? name = null, string? email = null, string? address = null)
        {
            Fixture specimenBuilders = new();
            CustomerDTO customer = new CustomerDTO()
            {
                Name = name ?? specimenBuilders.Create<string>(),
                EmailAddress = email ?? $"{specimenBuilders.Create<string>()}@server.pt",
                VatNumber = "123456789",
                Address = address ?? null
            };

            // Send POST request to create the customer
            RestRequest restRequest = new RestRequest("api/customers", Method.Post);
            restRequest.AddJsonBody(new CreateCustomerRequest(customer));

            var response = await fixture.RestClient.ExecutePostAsync<CreateCustomerResponse>(restRequest);
            return response.Data!;
        }

        public async Task<GetCustomersByIdResponse> GetCustomerByIdAsync(Guid customerId)
        {
            // Send GET request to create the customer
            RestRequest restRequest = new RestRequest($"api/customers/{customerId}", Method.Get);

            var response = await fixture.RestClient.ExecuteGetAsync<GetCustomersByIdResponse>(restRequest);
            return response.Data!;
        }
    }
}
