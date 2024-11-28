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