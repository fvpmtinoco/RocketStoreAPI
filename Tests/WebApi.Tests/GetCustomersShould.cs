using AutoFixture;
using FluentAssertions;
using RestSharp;
using RocketStoreApi.Features.GetCustomers;
using RocketStoreApi.Tests.Configurations;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Xunit;

namespace RocketStoreApi.Tests
{
    [Collection("CustomersAPI")]
    public partial class GetCustomersShould(CustomersFixture customersFixture) : TestsBase(customersFixture), IClassFixture<CustomersFixture>
    {
        #region Test Methods

        private readonly CustomersFixture customersFixture = customersFixture;

        [Fact]
        public async Task GetSucceedsAsync()
        {
            // Arrange
            Fixture specimenBuilders = new();
            string customerName1 = specimenBuilders.Create<string>();
            string customerName2 = specimenBuilders.Create<string>();
            var createCustomer1 = await CreateCustomerAsync(customerName1);
            var createCustomer2 = await CreateCustomerAsync(customerName2);

            RestRequest restRequest = new($"api/customers/", Method.Get);

            // Act
            var sut = await customersFixture.RestClient.ExecuteGetAsync<GetCustomersResponse>(restRequest);

            // Assert
            sut.StatusCode.Should().Be(HttpStatusCode.OK);
            sut.Data!.Customers.Select(c => c.Name).Should().Contain([customerName1, customerName2]);
        }

        [Fact]
        public async Task GetFilterByNameSucceedsAsync()
        {
            // Arrange
            Fixture specimenBuilders = new();
            string customerName1 = specimenBuilders.Create<string>();
            string customerName2 = specimenBuilders.Create<string>();
            var createCustomer1 = await CreateCustomerAsync(customerName1);
            var createCustomer2 = await CreateCustomerAsync(customerName2);

            RestRequest restRequest = new($"api/customers/", Method.Get);
            restRequest.AddQueryParameter("name", customerName1[..(customerName1.Length / 2)]);

            // Act
            var sut = await customersFixture.RestClient.ExecuteGetAsync<GetCustomersResponse>(restRequest);

            // Assert
            sut.StatusCode.Should().Be(HttpStatusCode.OK);
            // Should only contain the first customer
            sut.Data!.Customers.Select(c => c.Name).Should().BeEquivalentTo([customerName1]);
        }

        [Fact]
        public async Task GetFilterByEmailSucceedsAsync()
        {
            // Arrange
            Fixture specimenBuilders = new();
            string customerEmail = "fernando@random.pt";
            var createCustomer1 = await CreateCustomerAsync(email: customerEmail);
            var createCustomer2 = await CreateCustomerAsync();

            RestRequest restRequest = new($"api/customers/", Method.Get);
            restRequest.AddQueryParameter("email", customerEmail[..(customerEmail.Length / 2)]);

            // Act
            var sut = await customersFixture.RestClient.ExecuteGetAsync<GetCustomersResponse>(restRequest);

            // Assert
            sut.StatusCode.Should().Be(HttpStatusCode.OK);
            // Should only contain the first customer
            sut.Data!.Customers.Select(c => c.EmailAddress).Should().BeEquivalentTo([customerEmail]);
        }

        #endregion
    }
}
