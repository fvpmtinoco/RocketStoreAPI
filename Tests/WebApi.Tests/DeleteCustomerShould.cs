using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using RestSharp;
using RocketStoreApi.Features.DeleteCustomer;
using RocketStoreApi.Tests.Configurations;
using System;
using System.Net;
using System.Threading.Tasks;
using Xunit;

namespace RocketStoreApi.Tests
{
    [Collection("CustomersAPI")]
    public partial class DeleteCustomerShould(CustomersFixture costumersFixture) : TestsBase(costumersFixture), IClassFixture<CustomersFixture>
    {
        private readonly CustomersFixture costumersFixture = costumersFixture;

        #region Test Methods

        /// <summary>
        /// Test to ensure that the Delete operation for a customer works successfully.
        /// A customer is created via a POST request, and then the customer is deleted using a DELETE request.
        /// The test verifies that the response status code is NoContent (204), indicating successful deletion.
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task DeleteSucceedsAsync()
        {
            // Arrange
            var createResponse = await CreateCustomerAsync();
            Guid idToDelete = createResponse.Id;

            RestRequest restRequest = new($"api/customers/{idToDelete}", Method.Delete);

            // Act
            var sut = await costumersFixture.RestClient.ExecuteDeleteAsync(restRequest);

            // Assert
            sut.StatusCode.Should().Be(HttpStatusCode.NoContent);
        }

        /// <summary>
        /// Tests that a DELETE request to delete a customer returns a 404 Not Found status code
        /// and a specific error message when the customer does not exist.
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task ReturnConflitOnInvalidCustomer()
        {
            // Arrange
            Guid randomCustomer = Guid.NewGuid();
            RestRequest restRequest = new($"api/customers/{randomCustomer}", Method.Delete);

            // Act
            var sut = await costumersFixture.RestClient.ExecuteDeleteAsync(restRequest);

            // Assert
            sut.StatusCode.Should().Be(HttpStatusCode.NotFound);
            ProblemDetails error = await GetResponseContentAsync<ProblemDetails>(sut);
            error.Should().NotBeNull();
            error.Title.Should().Be(DeleteCustomerErrorCodes.InvalidCustomer.ToString());
        }

        #endregion
    }
}
