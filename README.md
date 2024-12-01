## Assumptions
1. The API will not grow much more in terms of functionalities (maybe an endpoint for customer update)

## Changelog
The following steps were taken to implement key architectural patterns and improve the organization of the codebase. The migration to .NET 8 included moving from Startup.cs to Program.cs for application configuration. 
- The CQRS and Mediator patterns were implemented to separate concerns and manage command/query logic. 
- Additionally, the Vertical Slice Architecture (VSA) pattern was applied assumption nº 1, avoiding the boilerplate code typically present in Clean Architecture. While Clean Architecture is a great choice for more complex APIs, VSA offers a simpler, more focused approach for smaller services, without neglecting the separation of concerns. These changes help improve the structure and clarity of the application.
- RestSharp was integrated in test project for easier handling of HTTP requests, as it abstracts much of the HTTP request handling, eliminating the need for custom Get and Post methods.
- FluentValidation (https://docs.fluentvalidation.net/en/latest/aspnet.html#minimal-apis) was used to handle model validation explicitly, as minimal APIs don't provide automatic validation like traditional controllers
- Simple implementation of caching with IMemoryCache to reduce external API calls and avoid hitting rate limits. Injected into the MediatR pipeline behavior for efficient caching management. Invalidation occurs on deletion of the customer

## Improvements not addressed due to lack of time
1. Distributed cache was not considered due to time limitations. Cache concurrency issues are not fully addressed in the current implementation. Using libraries like LazyCache or similar could offer a more resilient solution for handling concurrent cache access and updates. 
2. Concurrency issues related to create/delete operations are not addressed in this implementation. Synchronization mechanisms like locking or optimistic concurrency should be considered in a real world scenario
3. The usage of higher-level abstraction to simplify the process of declaring endpoints and organizing them in a more modular way. Extension methods for WebApplication were used, but recurring to Carter modules (https://github.com/CarterCommunity/Carter) would have avoided some boilerplate code in Program.cs

## Detailed changelog
### Steps 27/11/2024:
1. Fixed compilation errors.
2. Fixed integration tests.
3. Resolved issues with DI regarding singletons that used scoped services.
4. Fixed Automapper issue, lacking mapping from model to entity (EmailAddress -> Email).
5. Upgraded solution projects to .NET 8.
6. Upgraded NuGet packages accordingly.

### Steps 28/11/2024:
1. Implemented CQRS and Mediator patterns using Mediatr.
2. Implemented US02 & US03 using the minimal API pattern.
3. Applied VSA (Vertical Slice Architecture) pattern (https://www.milanjovanovic.tech/blog/vertical-slice-architecture).
4. Droppped the Startup.cs file, making use of simplified hosting model in the `Program.cs` file to configure services and middleware, available in later .NET versions.
5. Refactor tests to use RestSharp.
6. Added IXunitSerializer to enable Theory test to list all test outcomes in test explorer tree

### Steps 29/11/2024:
1. Refactored US01 and corresponding integration tests.
2. Used FluentValidation to replicate the validations that existed in the Customer model.
3. Refactored project folder structure
4. Added Docker support
5. Implemented US04 & US05
6. Mapped appsettings to records in order to inject them and read external API settings

### Steps 29/11/2024:
1. Completed missing tests
2. Implemented pagination in US02
3. Implemented caching in US03 using Mediatr pipelines

### Steps 30/11/2024:
1. Cachingbehavior tests
2. Caching only successful responses
