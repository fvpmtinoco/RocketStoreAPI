using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace RocketStoreApi.Configurations
{
    public static class ModelValidator
    {
        // Recursively validate the object and its nested properties
        public static (List<ValidationResult> Results, bool IsValid) DataAnnotationsValidate(this object model)
        {
            var results = new List<ValidationResult>();
            var context = new ValidationContext(model);

            // Validate the top-level object
            var isValid = Validator.TryValidateObject(model, context, results, validateAllProperties: true);

            // Validate nested objects
            foreach (var property in model.GetType().GetProperties())
            {
                var propertyValue = property.GetValue(model);
                if (propertyValue != null)
                {
                    // If property is a complex object, validate it recursively
                    if (property.PropertyType.IsClass && property.PropertyType != typeof(string))
                    {
                        var nestedResults = propertyValue.DataAnnotationsValidate();
                        results.AddRange(nestedResults.Results);
                    }

                    // If property is a collection, validate each item inside the collection
                    if (typeof(System.Collections.IEnumerable).IsAssignableFrom(property.PropertyType) &&
                        property.PropertyType != typeof(string))
                    {
                        var collection = propertyValue as System.Collections.IEnumerable;
                        if (collection != null)
                        {
                            foreach (var item in collection)
                            {
                                var nestedResults = item.DataAnnotationsValidate();
                                results.AddRange(nestedResults.Results);
                            }
                        }
                    }
                }
            }

            return (results, isValid && !results.Any(r => r != ValidationResult.Success));
        }
    }

    public static class CustomRouteHandlerBuilder
    {
        public static RouteHandlerBuilder Validate<T>(this RouteHandlerBuilder builder, bool firstErrorOnly = true)
        {
            builder.AddEndpointFilter(async (invocationContext, next) =>
            {
                var argument = invocationContext.Arguments.OfType<T>().FirstOrDefault();
                var response = argument.DataAnnotationsValidate();

                if (!response.IsValid)
                {
                    string errorMessage = firstErrorOnly
                        ? response.Results.FirstOrDefault()?.ErrorMessage
                        : string.Join("|", response.Results.Select(x => x.ErrorMessage));

                    return Results.Problem(errorMessage, statusCode: 400);
                }

                return await next(invocationContext);
            });

            return builder;
        }
    }
}
