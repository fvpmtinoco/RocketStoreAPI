﻿using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using RocketStoreApi.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace RocketStoreApi.Customers.GetCustomers
{
    public record GetCustomersByIdResponse(Customer Customer);

    public static class GetCustomersByIdEndpoint
    {
        public static void MapGetCustomersById(this WebApplication app)
        {
            app.MapGet("api/customers/{id}", async (ISender sender, [Required] Guid id) =>
            {
                var query = new GetCustomerByIdQuery(id);
                var customers = await sender.Send(query);

                if (customers == null)
                {
                    return Results.NoContent();
                }

                return Results.Ok(customers);
            })
            .WithName("GetCustomersById")
            .Produces<List<Customer>>(StatusCodes.Status200OK)
            .Produces<List<Customer>>(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status500InternalServerError)
            .WithOpenApi(op =>
            {
                op.Summary = "Get customer by id";
                op.Description = "Return a customer";
                return op;
            })
            .WithTags("Customers");
        }
    }
}