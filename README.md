The following steps were taken to implement key architectural patterns and improve the organization of the codebase. The migration to .NET 8 included moving from Startup.cs to Program.cs for application configuration. 
- The CQRS and Mediator patterns were implemented to separate concerns and manage command/query logic. 
- Additionally, the Vertical Slice Architecture (VSA) pattern was applied because the API is presumably expected to be a microservice and will not grow much larger, avoiding the boilerplate code typically present in Clean Architecture. While Clean Architecture is a great choice for more complex APIs, VSA offers a simpler, more focused approach for smaller, feature-specific services, without neglecting the separation of concerns. These changes help improve the structure, clarity, and maintainability of the application.
- RestSharp was integrated in test library for easier handling of HTTP requests, as it abstracts much of the HTTP request handling, eliminating the need for custom Get and Post methods.
    
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
