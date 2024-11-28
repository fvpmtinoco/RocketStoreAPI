using AutoFixture;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using RestSharp;
using RocketStoreApi.Controllers;
using RocketStoreApi.Managers;
using RocketStoreApi.Models;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace RocketStoreApi.Tests
{
    /// <summary>
    /// Provides integration tests for the <see cref="CustomersController"/> type.
    /// </summary>
    [Collection("CustomersAPI")]
    public partial class CustomersControllerTests(CustomersFixture fixture) : TestsBase, IClassFixture<CustomersFixture>
    {
        // Ignore Spelling: api


        #region Fields

        private readonly CustomersFixture fixture = fixture;

        #endregion

        #region Test Methods

        #region GetCustomersEndpoint

        //[Fact]
        //public async Task GetCustomersAsync()
        //{
        //    // Arrange

        //    IDictionary<string, string[]> expectedErrors = new Dictionary<string, string[]>
        //    {
        //        { "Name", new string[] { "The Name field is required." } },
        //        { "EmailAddress", new string[] { "The Email field is required." } }
        //    };

        //    Customer customer = new Customer();

        //    // Act

        //    HttpResponseMessage httpResponse = await this.fixture.GetAsync("api/customers");

        //    // Assert

        //    httpResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        //    ValidationProblemDetails error = await this.GetResponseContentAsync<ValidationProblemDetails>(httpResponse);
        //    error.Should().NotBeNull();
        //    error.Errors.Should().BeEquivalentTo(expectedErrors);
        //}

        #endregion

        /// <summary>
        /// Tests the <see cref="CustomersController.CreateCustomerAsync(Customer)"/> method
        /// to ensure that it requires name and email.
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/> that represents the asynchronous operation.
        /// </returns>
        [Theory]
        [MemberData(nameof(CreateRequiresNameAndEmailData))]

        public async Task CreateRequiresNameAndEmailAsync(ValidationErrorData validationErrorData)
        {
            // Arrange
            RestRequest request = new RestRequest("api/customers", Method.Post);
            request.AddJsonBody(validationErrorData.Json);

            // Act
            var sut = await this.fixture.RestClient.ExecutePostAsync(request);

            // Assert
            sut.StatusCode.Should().Be(HttpStatusCode.BadRequest);

            ValidationProblemDetails error = await this.GetResponseContentAsync<ValidationProblemDetails>(sut);
            error.Should().NotBeNull();
            error.Errors.Should().BeEquivalentTo(validationErrorData.ExpectedErrors);
        }

        public static IEnumerable<object[]> CreateRequiresNameAndEmailData()
        {
            Fixture fixture = new();

            yield return new object[] { new ValidationErrorData()
                {
                    Json = $@"{{ ""{nameof(Customer.Name)}"" : null, ""{nameof(Customer.EmailAddress)}"": null }}",
                    ExpectedErrors = new Dictionary<string, string[]>
                    {
                        { "Name", new string[] { "The Name field is required." } },
                        { "EmailAddress", new string[] { "The Email field is required." } }
                    }
                }
            };

            yield return new object[] { new ValidationErrorData()
                {
                    Json = @"{ }",
                    ExpectedErrors = new Dictionary<string, string[]>
                    {
                        { "Name", new string[] { "The Name field is required." } },
                        { "EmailAddress", new string[] { "The Email field is required." } }
                    }
                }
            };

            yield return new object[] {
                new ValidationErrorData()
                {
                    Json = $@"{{ ""{nameof(Customer.Name)}"" : ""{fixture.Create<string>()}"", ""{nameof(Customer.EmailAddress)}"": null }}",
                    ExpectedErrors = new Dictionary<string, string[]>
                    {
                        { nameof(Customer.EmailAddress), new[] { "The Email field is required." } }
                    }
                }
            };

            yield return new object[] {
                new ValidationErrorData()
                {
                    Json =  $@"{{ ""{nameof(Customer.Name)}"" : ""{fixture.Create<string>()}"", ""{nameof(Customer.EmailAddress)}"": ""{fixture.Create<string>()}"" }}",
                    ExpectedErrors = new Dictionary<string, string[]>
                    {
                        { nameof(Customer.EmailAddress), ["The Email field is not a valid e-mail address."] }
                    }
                }
            };

            yield return new object[] {
                new ValidationErrorData()
                {
                    Json =  $@"{{ ""{nameof(Customer.Name)}"" : ""{fixture.Create<string>()}"", ""{nameof(Customer.EmailAddress)}"" : ""valid@example.com"", ""{nameof(Customer.VatNumber)}"": ""{fixture.Create<int>()}""}}",
                    ExpectedErrors = new Dictionary<string, string[]>
                    {
                        { nameof(Customer.VatNumber), ["The field VAT Number must match the regular expression '^[0-9]{9}$'."] }
                    }
                }
            };
        }

        /// <summary>
        /// Tests the <see cref="CustomersController.CreateCustomerAsync(Customer)"/> method
        /// to ensure that it requires a unique email address.
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/> that represents the asynchronous operation.
        /// </returns>
        [Fact]
        public async Task CreateRequiresUniqueEmailAsync()
        {
            // Arrange
            Customer customer1 = new Customer()
            {
                Name = "A customer",
                EmailAddress = "customer@server.pt"
            };

            Customer customer2 = new Customer()
            {
                Name = "Another customer",
                EmailAddress = "customer@server.pt"
            };

            RestRequest request = new RestRequest("api/customers", Method.Post);
            request.AddJsonBody(customer1);
            _ = await fixture.RestClient.PostAsync<Guid>(request);

            // Act
            request = new RestRequest("api/customers", Method.Post);
            request.AddJsonBody(customer2);
            var sut = await fixture.RestClient.ExecutePostAsync<Customer>(request);

            // Assert
            ProblemDetails error = await this.GetResponseContentAsync<ProblemDetails>(sut);
            error.Should().NotBeNull();
            error.Title.Should().Be(ErrorCodes.CustomerAlreadyExists);
        }

        /// <summary>
        /// Tests the <see cref="CustomersController.CreateCustomerAsync(Customer)"/> method
        /// to ensure that it requires a valid VAT number.
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/> that represents the asynchronous operation.
        /// </returns>
        [Fact]
        public async Task CreateSucceedsAsync()
        {
            // Arrange

            Customer customer = new Customer()
            {
                Name = "My customer",
                EmailAddress = "mycustomer@server.pt",
                VatNumber = "123456789"
            };
            RestRequest restRequest = new RestRequest("api/customers", Method.Post);
            restRequest.AddJsonBody(customer);

            // Act
            var sut = await this.fixture.RestClient.ExecutePostAsync(restRequest);

            // Assert
            sut.StatusCode.Should().Be(HttpStatusCode.Created);

            Guid? id = await this.GetResponseContentAsync<Guid?>(sut);
            id.Should().NotBeNull();
            sut.Headers.Should().Contain(h => h.Name == "Location" && h.Value != null);
        }

        #endregion
    }

    public class ValidationErrorData : IXunitSerializable
    {
        public string Json { get; set; } = default!;
        public Dictionary<string, string[]> ExpectedErrors { get; set; } = default!;

        public void Deserialize(IXunitSerializationInfo info)
        {
            Json = info.GetValue<string>("Json");

            // Needed for complex types
            var requestJson = info.GetValue<string>(nameof(ExpectedErrors));
            ExpectedErrors = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, string[]>>(requestJson)!;
        }

        public void Serialize(IXunitSerializationInfo info)
        {
            info.AddValue("Json", Json);

            // Needed for complex types
            var requestJson = System.Text.Json.JsonSerializer.Serialize(ExpectedErrors);
            info.AddValue(nameof(ExpectedErrors), requestJson);
        }
    }
}
