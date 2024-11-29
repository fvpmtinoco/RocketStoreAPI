using AutoMapper;
using FluentValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RocketStoreApi.Customers.CreateCustomer;
using RocketStoreApi.Customers.GetCustomers;
using RocketStoreApi.Managers;
using RocketStoreApi.Storage;

var builder = WebApplication.CreateBuilder(args);

// Add services to the API

builder.Services.AddMediatR(config =>
{
    // Register MediatR services from the current assembly
    config.RegisterServicesFromAssemblies(typeof(Program).Assembly);
});

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseInMemoryDatabase("RocketStoreApiDb"));

builder.Services.AddOpenApiDocument(
    (options) =>
    {
        options.DocumentName = "Version 1";
        options.Title = "RocketStore API";
        options.Description = "REST API for the RocketStore Web Application";
    });

// Register AutoMapper and scan the current assembly for profiles
builder.Services.AddAutoMapper(typeof(Program));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    // Group endpoints under the "Customers" category in Swagger UI
    options.TagActionsBy(api =>
    {
        if (api.RelativePath.Contains("customers", System.StringComparison.OrdinalIgnoreCase))
        {
            return new[] { "Customers" };
        }
        return new[] { "Default" };
    });
});

builder.Services.AddScoped<Profile, MappingProfile>();
builder.Services.AddControllers();
builder.Services.AddValidatorsFromAssemblyContaining<Program>();

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

// Enable OpenAPI (Swagger) for API documentation
app.UseOpenApi();
app.UseSwaggerUi();
app.UseSwagger();

// Redirect HTTP requests to HTTPS
app.UseHttpsRedirection();

// Set up routing (necessary for minimal API and controllers)
app.UseRouting();

app.MapControllers();

//Minimal API 
app.MapGetCustomers();
app.MapGetCustomersById();
app.MapCreateCustomer();


// Add authorization middleware
app.UseAuthorization();

app.Run();

// For Web API integration tests
public partial class Program { }
