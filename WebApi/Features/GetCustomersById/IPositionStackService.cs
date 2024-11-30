using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RocketStoreApi.Configurations;
using RocketStoreApi.Features.GetCustomers;
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace RocketStoreApi.Features.GetCustomersById
{
    public interface IPositionStackService
    {
        /// <summary>
        /// Get the coordinates (latitude and longitude) of an address using the PositionStack API.
        /// </summary>
        /// <param name="address"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<Result<PositionStackResponse, GetCustomersByIdErrorCodes>> GetCoordinatesAsync(string address, CancellationToken cancellationToken);
    }

    public class PositionStackService : IPositionStackService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly AppSettings _appSettings;

        public PositionStackService(IHttpClientFactory httpClientFactory, IOptions<AppSettings> appSettings)
        {
            _httpClientFactory = httpClientFactory;
            _appSettings = appSettings.Value;
        }

        // Virtual so it can be mocked
        public virtual async Task<Result<PositionStackResponse, GetCustomersByIdErrorCodes>> GetCoordinatesAsync(string address, CancellationToken cancellationToken)
        {
            var httpClient = _httpClientFactory.CreateClient();
            var apiUrl = _appSettings.PositionStack.Url;
            var accessKey = _appSettings.PositionStack.AccessKey;
            var queryParams = $"?access_key={accessKey}&query={Uri.EscapeDataString(address)}&fields=results.latitude,results.longitude";

            try
            {
                var response = await httpClient.GetStringAsync(apiUrl + queryParams, cancellationToken);
                var positionStackResponse = JsonConvert.DeserializeObject<PositionStackResponse>(response);

                if (positionStackResponse == null || positionStackResponse.Data == null || positionStackResponse.Data.Count == 0)
                {
                    return Result<PositionStackResponse, GetCustomersByIdErrorCodes>.Failure(GetCustomersByIdErrorCodes.ApiError, "Invalid response from PositionStack API.");
                }

                return Result<PositionStackResponse, GetCustomersByIdErrorCodes>.Success(positionStackResponse);
            }
            catch (HttpRequestException ex)
            {
                return Result<PositionStackResponse, GetCustomersByIdErrorCodes>.Failure(GetCustomersByIdErrorCodes.ApiError, $"Error calling PositionStack API: {ex.Message}");
            }
            catch (TaskCanceledException) when (!cancellationToken.IsCancellationRequested)
            {
                return Result<PositionStackResponse, GetCustomersByIdErrorCodes>.Failure(GetCustomersByIdErrorCodes.ApiError, "The request to PositionStack API timed out.");
            }
            catch (JsonException ex)
            {
                return Result<PositionStackResponse, GetCustomersByIdErrorCodes>.Failure(GetCustomersByIdErrorCodes.ApiError, $"Error deserializing PositionStack response: {ex.Message}");
            }
        }
    }
}
