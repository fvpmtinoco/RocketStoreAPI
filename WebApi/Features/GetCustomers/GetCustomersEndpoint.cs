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
    public record GetCustomersResponse(List<CustomerDTO> Customers);

    public static class GetCustomersEndpoint
    {
        public static void MapGetCustomers(this WebApplication app)
        {
            app.MapGet("api/customers", async (ISender sender, string? name, string? email) =>
            {
                var query = new GetCustomersQuery(name, email);
                var result = await sender.Send(query);
                return Results.Ok(new GetCustomersResponse(result.Customers));
            })
            .WithName("GetCustomers")
            .Produces<List<CustomerDTO>>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status500InternalServerError)
            .WithOpenApi(op =>
            {
                op.Summary = "Get all customers";
                return op;
            })
            .WithTags("Customers");
        }
    }
}
