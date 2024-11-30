using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace RocketStoreApi.Features.DeleteCustomer
{
    public static class DeleteCustomerEndpoint
    {
        public static void MapDeleteCustomer(this WebApplication app)
        {
            app.MapDelete("api/customers/{id}", async (ISender sender, [FromRoute, Required][Description("The costumer's identifier")] Guid id) =>
            {
                DeleteCustomerCommand command = new DeleteCustomerCommand(id);
                var result = await sender.Send(command);
                if (result.IsSuccess)
                    return Results.NoContent();

                // If the customer doesn't exist (InvalidCustomer), return NotFound (404)
                return Results.NotFound(new ProblemDetails
                {
                    Title = result.ErrorCode.ToString(),
                    Detail = result.ErrorDescription
                });

            }).WithName("DeleteCustomer")
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status500InternalServerError)
            .WithOpenApi(op =>
            {
                op.Summary = "Delete a customer";
                return op;
            })
            .WithTags("Customers");
        }
    }

    public enum DeleteCustomerErrorCodes
    {
        [Description("The customer is invalid")]
        InvalidCustomer
    }
}