using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RocketStoreApi.SharedModels;
using System.Collections.Generic;
using System.ComponentModel;

namespace RocketStoreApi.Features.GetCustomers
{
    /// <summary>
    /// Response object for GetCustomers with pagination support
    /// </summary>
    /// <param name="Customers">List of customer data</param>
    /// <param name="TotalCount">Total number of customers (unpaginated)</param>
    /// <param name="PageNumber">Current page number</param>
    /// <param name="PageSize">Number of customers per page</param>
    public record GetCustomersResponse(List<CustomerDTO> Customers, int TotalCount, int PageNumber, int PageSize);

    public static class GetCustomersEndpoint
    {
        public static void MapGetCustomers(this WebApplication app)
        {
            app.MapGet("api/customers",
                async (ISender sender,
                [FromQuery][Description("The name of the customer to filter by")] string? name,
                [FromQuery][Description("The email of the customer to filter by")] string? email,
                [FromQuery][Description("The page number for pagination.")] int pageNumber = 1,
                [FromQuery][Description("The number of customers per page")] int pageSize = 50) =>
            {
                var query = new GetCustomersQuery(name, email, pageNumber, pageSize);
                var result = await sender.Send(query);

                return Results.Ok(new GetCustomersResponse(result.Customers, result.TotalCount, pageNumber, pageSize));
            })
            .WithName("GetCustomers")
            .Produces<List<CustomerDTO>>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status500InternalServerError)
            .WithOpenApi(op =>
            {
                op.Summary = "Get a list of customers with pagination.";
                return op;
            })
            .WithTags("Customers");
        }
    }
}
