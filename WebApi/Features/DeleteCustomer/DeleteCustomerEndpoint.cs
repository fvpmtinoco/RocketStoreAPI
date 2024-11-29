using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Net;

namespace RocketStoreApi.Features.DeleteCustomer
{
    public static class DeleteCustomerEndpoint
    {
        public static void MapDeleteCustomer(this WebApplication app)
        {
            app.MapDelete("api/customers/{id}", async (ISender sender, [Required] Guid id) =>
            {
                DeleteCustomerCommand command = new DeleteCustomerCommand(id);
                var result = await sender.Send(command);
                if (result.IsSuccess)
                    return Results.NoContent();

                // Error handling - return detailed error responses based on the result's error code
                var problemDetails = new ProblemDetails
                {
                    Title = result.ErrorCode.ToString(),
                    Detail = result.ErrorDescription,
                };

                if (result.ErrorCode == DeleteCustomerErrorCodes.InvalidCustomer)
                {
                    // NotFound (404) when the customer does not exist
                    problemDetails.Status = (int)HttpStatusCode.Conflict;
                    return Results.NotFound(problemDetails);
                }

                // Generic Bad Request (400) for other errors
                problemDetails.Status = (int)HttpStatusCode.BadRequest;
                return Results.BadRequest(problemDetails);
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
        [Description("The customer doesn't exist")]
        InvalidCustomer
    }
}