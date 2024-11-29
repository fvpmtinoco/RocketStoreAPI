using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RocketStoreApi.SharedModels;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Net;

namespace RocketStoreApi.Features.CreateCustomer
{
    /// <summary>
    /// Represents a request to create a new customer.
    /// </summary>
    /// <param name="Customer"></param>
    public record CreateCustomerRequest(Customer Customer);

    /// <summary>
    /// Represents a response to a successful creation of a new customer.
    /// </summary>
    /// <param name="Id"></param>
    public record CreateCustomerResponse(Guid Id);

    public static class CreateCustomerEndpoint
    {
        public static void MapCreateCustomer(this WebApplication app)
        {
            app.MapPost("api/customers", async (IValidator<CreateCustomerRequest> validator, ISender sender, [FromBody, Required] CreateCustomerRequest request) =>
            {
                var validationResult = await validator.ValidateAsync(request);

                if (!validationResult.IsValid)
                {
                    return Results.ValidationProblem(validationResult.ToDictionary());
                }

                var command = new CreateCustomerCommand(request.Customer.Name, request.Customer.EmailAddress, request.Customer.VatNumber, request.Customer.Address);
                var result = await sender.Send(command);

                if (result.IsSuccess)
                    return Results.Created($"api/customers/{result.Value.Id}", new CreateCustomerResponse(result.Value.Id));
                else
                {
                    // Error handling - return detailed error responses based on the result's error code
                    var problemDetails = new ProblemDetails
                    {
                        Title = result.ErrorCode.ToString(),
                        Detail = result.ErrorDescription,
                    };

                    if (result.ErrorCode == CreateCustomerErrorCodes.CustomerAlreadyExists)
                    {
                        // Conflict (409) when the customer already exists
                        problemDetails.Status = (int)HttpStatusCode.Conflict;
                        return Results.Conflict(problemDetails);
                    }

                    // Generic Bad Request (400) for other errors
                    problemDetails.Status = (int)HttpStatusCode.BadRequest;
                    return Results.BadRequest(problemDetails);
                }
            })
            .WithName("CreateCustomer")
            .Produces<CreateCustomerResponse>(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status409Conflict)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status500InternalServerError)
            .WithOpenApi(op =>
            {
                op.Summary = "Create a new customer";
                return op;
            })
            .WithTags("Customers");
        }
    }

    public enum CreateCustomerErrorCodes
    {
        [Description("The customer already exists")]
        CustomerAlreadyExists
    }

    /// <summary>
    /// Validates the <see cref="CreateCustomerRequest"/> model.
    /// This validator ensures that the properties of the request are valid, 
    /// including delegating validation of the nested <see cref="Customer"/> object 
    /// to the <see cref="CustomerValidator"/>.
    /// </summary>
    public class CreateCustomerRequestValidator : AbstractValidator<CreateCustomerRequest>
    {
        public CreateCustomerRequestValidator()
        {
            RuleFor(x => x.Customer).SetValidator(new CustomerValidator());
        }
    }
}
