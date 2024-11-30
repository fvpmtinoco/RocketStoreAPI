using FluentValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RocketStoreApi.Configurations;
using RocketStoreApi.Configurations.Caching;
using RocketStoreApi.Features.CreateCustomer;
using RocketStoreApi.Features.DeleteCustomer;
using RocketStoreApi.Features.GetCustomers;
using RocketStoreApi.Features.GetCustomersById;
using RocketStoreApi.Storage;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the API

// Register IMemoryCache for use in CachingBehavior
builder.Services.AddMemoryCache();

// Register MediatR services from the current assembly
builder.Services.AddMediatR(config =>
{
    config.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());

    // Behavior pipeline for caching
    config.AddOpenBehavior(typeof(CachingBehavior<,>));
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

builder.Services.AddControllers();

// Fluent validation DI
builder.Services.AddValidatorsFromAssemblyContaining(typeof(Program));

// Register IHttpClientFactory
builder.Services.AddHttpClient();

// Register AppSettings
builder.Services.Configure<AppSettings>(builder.Configuration);

// Register services
builder.Services.AddScoped<IPositionStackService, PositionStackService>();

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

//Minimal API extension methods
app.MapGetCustomers();
app.MapGetCustomersById();
app.MapCreateCustomer();
app.MapDeleteCustomer();

// Add authorization middleware
app.UseAuthorization();

app.Run();

// For Web API integration tests
public partial class Program { }
