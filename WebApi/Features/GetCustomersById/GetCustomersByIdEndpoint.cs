using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace RocketStoreApi.Features.GetCustomers
{
    /// <summary>
    /// Represents a response to a request to get a customer by its ID.
    /// </summary>
    /// <param name="Customer"></param>
    public record GetCustomersByIdResponse(CustomerDetail Customer);

    public static class GetCustomersByIdEndpoint
    {
        public static void MapGetCustomersById(this WebApplication app)
        {
            app.MapGet("api/customers/{id}", async (ISender sender, [FromRoute, Required][Description("The costumer's identifier")] Guid id) =>
            {
                var query = new GetCustomerByIdQuery(id);
                var result = await sender.Send(query);

                if (result.IsSuccess)
                    return Results.Ok(result.Value);

                if (result.ErrorCode == GetCustomersByIdErrorCodes.InvalidCustomer)
                {
                    // NotFound (404) when the customer does not exist
                    return Results.NotFound(new ProblemDetails { Title = result.ErrorCode.ToString(), Detail = result.ErrorDescription });
                }

                // Generic Bad Request (400) for other errors
                return Results.BadRequest();
            })
            .WithName("GetCustomerById")
            .Produces<CustomerDetail>(StatusCodes.Status200OK)
            .Produces<CustomerDetail>(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status500InternalServerError)
            .WithOpenApi(op =>
            {
                op.Summary = "Get customer by id";
                return op;
            })
            .WithTags("Customers");
        }
    }

    public enum GetCustomersByIdErrorCodes
    {
        [Description("The customer is invalid")]
        InvalidCustomer,
        [Description("Error calling PositionStack API")]
        ApiError
    }

    /// <summary>
    /// Defines a detailed customer with geolocation information.
    /// </summary>
    public class CustomerDetail : SharedModels.CustomerDTO
    {
        /// <summary>
        /// Latitude of the customer's address.
        /// This value is fetched from an external geolocation service.
        /// </summary>
        public double? Latitude { get; init; } = default!;

        /// <summary>
        /// Longitude of the customer's address.
        /// This value is fetched from an external geolocation service.
        /// </summary>
        public double? Longitude { get; init; } = default!;
    }
}
