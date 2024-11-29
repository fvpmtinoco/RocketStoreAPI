using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using RocketStoreApi.SharedModels;
using System.Collections.Generic;

namespace RocketStoreApi.Features.GetCustomers
{
    /// <summary>
    /// Response object for GetCustomers
    /// </summary>
    /// <param name="Customers"></param>
    public record GetCustomersResponse(List<Customer> Customers);

    public static class GetCustomersEndpoint
    {
        public static void MapGetCustomers(this WebApplication app)
        {
            app.MapGet("api/customers", async (ISender sender, string? name, string? email) =>
            {
                var query = new GetCustomersQuery(name, email);
                var customers = await sender.Send(query);
                return Results.Ok(customers);
            })
            .WithName("GetCustomers")
            .Produces<List<Customer>>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status500InternalServerError)
            .WithOpenApi(op =>
            {
                op.Summary = "Get all customers";
                op.Description = "Return all customers";
                return op;
            })
            .WithTags("Customers");
        }
    }
}
