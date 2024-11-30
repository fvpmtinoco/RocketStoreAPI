using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace RocketStoreApi.Features.GetCustomers
{
    public record GetCustomersByIdResponse(CustomerAddressDetail Customer);

    public static class GetCustomersByIdEndpoint
    {
        public static void MapGetCustomersById(this WebApplication app)
        {
            app.MapGet("api/customers/{id}", async (ISender sender, [Required] Guid id) =>
            {
                var query = new GetCustomerByIdQuery(id);
                var result = await sender.Send(query);

                if (result.IsSuccess)
                    return Results.Ok(result.Value);

                // Error handling - return detailed error responses based on the result's error code
                if (result.ErrorCode == GetCustomersByIdErrorCodes.InvalidCustomer)
                {
                    // NotFound (404) when the customer does not exist
                    return Results.NotFound(new ProblemDetails { Title = result.ErrorCode.ToString(), Detail = result.ErrorDescription });
                }

                // Generic Bad Request (400) for other errors
                return Results.BadRequest();
            })
            .WithName("GetCustomerById")
            .Produces<CustomerAddressDetail>(StatusCodes.Status200OK)
            .Produces<CustomerAddressDetail>(StatusCodes.Status204NoContent)
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
}
