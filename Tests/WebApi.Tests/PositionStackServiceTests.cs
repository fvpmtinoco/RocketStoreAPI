using AutoFixture;
using FluentAssertions;
using Moq;
using RocketStoreApi.Configurations;
using RocketStoreApi.Features.GetCustomers;
using RocketStoreApi.Features.GetCustomersById;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace RocketStoreApi.Tests
{
    [Collection("CustomersAPI")]
    public class GetCustomerByIdQueryHandlerTests
    {
        private readonly Mock<IPositionStackService> positionStackServiceMock;

        public GetCustomerByIdQueryHandlerTests()
        {
            positionStackServiceMock = new Mock<IPositionStackService>();
        }

        [Fact]
        public async Task GetCoordinatesAsyncShouldReturnSuccessWhenAPIRespondsCorrectly()
        {
            // Arrange
            var specimenBuilders = new Fixture();
            var customerAddress = specimenBuilders.Create<string>();
            var expectedLatitude = specimenBuilders.Create<double>();
            var expectedLongitude = specimenBuilders.Create<double>();

            // Create a mock PositionStackResponse
            var mockResponse = new PositionStackResponse
            {
                Data = [new Data { Latitude = expectedLatitude, Longitude = expectedLongitude }]
            };

            // Mock the service method to return a successful response
            positionStackServiceMock.Setup(s => s.GetCoordinatesAsync(customerAddress, It.IsAny<CancellationToken>()))
                .ReturnsAsync(Result<PositionStackResponse, GetCustomersByIdErrorCodes>.Success(mockResponse));

            // Act
            var result = await positionStackServiceMock.Object.GetCoordinatesAsync(customerAddress, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.Data![0].Latitude.Should().Be(expectedLatitude);
            result.Value.Data![0].Longitude.Should().Be(expectedLongitude);
        }

        [Fact]
        public async Task GetCoordinatesAsyncShouldReturnFailureWhenAPIRespondsInvalidData()
        {
            // Arrange
            var customerAddress = "Invalid address";

            // Mock the service method to return a failure due to invalid data from API
            positionStackServiceMock.Setup(s => s.GetCoordinatesAsync(customerAddress, It.IsAny<CancellationToken>()))
                .ReturnsAsync(Result<PositionStackResponse, GetCustomersByIdErrorCodes>.Failure(GetCustomersByIdErrorCodes.ApiError, "Invalid response from PositionStack API."));

            // Act
            var result = await positionStackServiceMock.Object.GetCoordinatesAsync(customerAddress, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.ErrorCode.Should().Be(GetCustomersByIdErrorCodes.ApiError);
            result.ErrorDescription.Should().Be("Invalid response from PositionStack API.");
        }
    }
}
