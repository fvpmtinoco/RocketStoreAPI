using AutoFixture;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using RestSharp;
using RocketStoreApi.Features.CreateCustomer;
using RocketStoreApi.SharedModels;
using RocketStoreApi.Tests.Configurations;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace RocketStoreApi.Tests
{
    [Collection("CustomersAPI")]
    public partial class CreateCustomerShould(CustomersFixture costumersFixture) : TestsBase(costumersFixture), IClassFixture<CustomersFixture>
    {
        #region Test Methods

        private readonly CustomersFixture costumersFixture = costumersFixture;

        /// <summary>
        /// Tests the CreateCustomer feature to ensure endpoint parameters are valid
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/> that represents the asynchronous operation.
        /// </returns>
        [Theory]
        [MemberData(nameof(EnsureContractIsRespectedData))]

        public async Task EnsureContractIsRespected(CreateCustomerValidationErrorData validationErrorData)
        {
            // Arrange
            RestRequest request = new RestRequest("api/customers", Method.Post);
            request.AddJsonBody(validationErrorData.Json);

            // Act
            var sut = await costumersFixture.RestClient.ExecutePostAsync(request);

            // Assert
            sut.StatusCode.Should().Be(HttpStatusCode.BadRequest);

            ValidationProblemDetails error = await this.GetResponseContentAsync<ValidationProblemDetails>(sut);
            error.Should().NotBeNull();
            error.Errors.Keys.Should().BeEquivalentTo(validationErrorData.ExpectedErrors);
        }

        public static IEnumerable<object[]> EnsureContractIsRespectedData()
        {
            Fixture fixture = new();

            yield return new object[] {
                new CreateCustomerValidationErrorData()
                {
                    Json = $@"{{ ""customer"": {{ ""{nameof(CustomerDTO.Name)}"" : null, ""{nameof(CustomerDTO.EmailAddress)}"": null }} }}",
                    ExpectedErrors = ["Customer.Name" ,"Customer.EmailAddress"],
                    TestDescription = "Name and Email are null"
                }
            };

            yield return new object[] {
                new CreateCustomerValidationErrorData()
                {
                    Json = @"{ ""customer"": { } }",
                    ExpectedErrors = ["Customer.Name" ,"Customer.EmailAddress"],
                    TestDescription = "Request is empty"
                }
            };

            yield return new object[] {
                new CreateCustomerValidationErrorData()
                {
                    Json = $@"{{ ""customer"": {{ ""{nameof(CustomerDTO.Name)}"" : ""{fixture.Create<string>()}"", ""{nameof(CustomerDTO.EmailAddress)}"": null }} }}",
                    ExpectedErrors = ["Customer.EmailAddress"],
                    TestDescription = "Email is null"
                }
            };

            yield return new object[] {
                new CreateCustomerValidationErrorData()
                {
                    Json =  $@"{{ ""customer"": {{ ""{nameof(CustomerDTO.Name)}"" : ""{fixture.Create<string>()}"", ""{nameof(CustomerDTO.EmailAddress)}"": ""{fixture.Create<string>()}"" }} }}",
                    ExpectedErrors = ["Customer.EmailAddress"],
                    TestDescription = "Email is invalid"
                }
            };

            yield return new object[] {
                new CreateCustomerValidationErrorData()
                {
                    Json =  $@"{{ ""customer"": {{ ""{nameof(CustomerDTO.Name)}"" : ""{fixture.Create<string>()}"", ""{nameof(CustomerDTO.EmailAddress)}"" : ""valid@example.com"", ""{nameof(CustomerDTO.VatNumber)}"": ""{fixture.Create<int>()}"" }} }}",
                    ExpectedErrors = ["Customer.VatNumber"],
                    TestDescription = "VAT number is invalid"
                }
            };
        }

        /// <summary>
        /// Tests the <see cref="CustomersController.CreateCustomerAsync(CustomerDTO)"/> method
        /// to ensure that it requires a unique email address.
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/> that represents the asynchronous operation.
        /// </returns>
        [Fact]
        public async Task CreateRequiresUniqueEmailAsync()
        {
            // Arrange
            CustomerDTO customer1 = new CustomerDTO()
            {
                Name = "A customer",
                EmailAddress = "customer@server.pt"
            };

            CustomerDTO customer2 = new CustomerDTO()
            {
                Name = "Another customer",
                EmailAddress = "customer@server.pt"
            };

            RestRequest request = new RestRequest("api/customers", Method.Post);
            request.AddJsonBody(new CreateCustomerRequest(customer1));
            _ = await costumersFixture.RestClient.PostAsync<CreateCustomerResponse>(request);

            // Act
            request = new RestRequest("api/customers", Method.Post);
            request.AddJsonBody(new CreateCustomerRequest(customer2));
            var sut = await costumersFixture.RestClient.ExecutePostAsync<CreateCustomerResponse>(request);

            // Assert
            ProblemDetails error = await this.GetResponseContentAsync<ProblemDetails>(sut);
            error.Should().NotBeNull();
            error.Title.Should().Be(CreateCustomerErrorCodes.CustomerAlreadyExists.ToString());
        }

        /// <summary>
        /// Tests the <see cref="CustomersController.CreateCustomerAsync(CustomerDTO)"/> method
        /// to ensure that it requires a valid VAT number.
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/> that represents the asynchronous operation.
        /// </returns>
        [Fact]
        public async Task CreateSucceedsAsync()
        {
            // Arrange

            CustomerDTO customer = new CustomerDTO()
            {
                Name = "My customer",
                EmailAddress = "mycustomer@server.pt",
                VatNumber = "123456789"
            };
            RestRequest restRequest = new RestRequest("api/customers", Method.Post);
            restRequest.AddJsonBody(new CreateCustomerRequest(customer));

            // Act
            var sut = await costumersFixture.RestClient.ExecutePostAsync<CreateCustomerResponse>(restRequest);

            // Assert
            sut.StatusCode.Should().Be(HttpStatusCode.Created);

            var result = await this.GetResponseContentAsync<CreateCustomerResponse>(sut);
            result.Should().NotBeNull();
            sut.Headers.Should().Contain(h => h.Name == "Location" && h.Value != null);
        }

        #endregion
    }

    /// <summary>
    /// IXunitSerializable implementation on ValidationErrorData to decompose CreateRequiresNameAndEmailAsync theory tests
    /// </summary>
    public class CreateCustomerValidationErrorData : TestCaseBase, IXunitSerializable
    {
        public string Json { get; set; } = default!;
        public string[] ExpectedErrors { get; set; } = [];

        public void Deserialize(IXunitSerializationInfo info)
        {
            Json = info.GetValue<string>(nameof(Json));
            TestDescription = info.GetValue<string>(nameof(TestDescription));

            // Needed for complex types
            var requestJson = info.GetValue<string>(nameof(ExpectedErrors));
            ExpectedErrors = System.Text.Json.JsonSerializer.Deserialize<string[]>(requestJson)!;
        }

        public void Serialize(IXunitSerializationInfo info)
        {
            info.AddValue(nameof(Json), Json);
            info.AddValue(nameof(TestDescription), TestDescription);

            // Needed for complex types
            var requestJson = System.Text.Json.JsonSerializer.Serialize(ExpectedErrors);
            info.AddValue(nameof(ExpectedErrors), requestJson);
        }

        // Override ToString to return the description
        //public override string ToString() => TestDescription;
    }
}
