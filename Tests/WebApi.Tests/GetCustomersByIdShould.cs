using AutoFixture;
using FluentAssertions;
using RestSharp;
using RocketStoreApi.Features.GetCustomers;
using RocketStoreApi.Tests.Configurations;
using System.Net;
using System.Threading.Tasks;
using Xunit;

namespace RocketStoreApi.Tests
{
    [Collection("CustomersAPI")]
    public partial class GetCustomersByIdShould(CustomersFixture fixture) : TestsBase(fixture), IClassFixture<CustomersFixture>
    {
        private readonly CustomersFixture fixture = fixture;

        #region Test Methods

        [Fact]
        public async Task GetByIdSucceedsAsync()
        {
            // Arrange
            Fixture specimenBuilders = new();
            string customerName = specimenBuilders.Create<string>();
            string customerEmail = $"{specimenBuilders.Create<string>()}@random.pt";
            var createCustomer = await CreateCustomerAsync(name: customerName, email: customerEmail);

            RestRequest restRequest = new($"api/customers/{createCustomer.Id}", Method.Get);

            // Act
            var sut = await fixture.RestClient.ExecuteGetAsync<GetCustomersByIdResponse>(restRequest);

            // Assert
            sut.StatusCode.Should().Be(HttpStatusCode.OK);
            sut.Data!.Customer.Name.Should().Be(customerName);
            sut.Data!.Customer.EmailAddress.Should().Be(customerEmail);
        }

        [Fact]
        public async Task GetAddressSucceedsAsync()
        {
            // Arrange
            Fixture specimenBuilders = new();
            var createCustomer = await CreateCustomerAsync(address: "Rua Principal 51, 3040-650 Assafarge, Portugal");

            RestRequest restRequest = new($"api/customers/{createCustomer.Id}", Method.Get);

            // Act
            var sut = await fixture.RestClient.ExecuteGetAsync<GetCustomersByIdResponse>(restRequest);

            // Assert
            sut.StatusCode.Should().Be(HttpStatusCode.OK);
            sut.Data.Should().NotBeNull();
            sut.Data!.Customer.Latitude.Should().NotBeNull();
            sut.Data!.Customer.Latitude.Should().Be(40.15895);

            sut.Data!.Customer.Longitude.Should().NotBeNull();
            sut.Data!.Customer.Longitude.Should().Be(-8.43167);
        }

        #endregion
    }
}
