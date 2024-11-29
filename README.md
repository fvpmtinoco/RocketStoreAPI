The following steps were taken to implement key architectural patterns and improve the organization of the codebase. The migration to .NET 8 included moving from Startup.cs to Program.cs for application configuration. 
- The CQRS and Mediator patterns were implemented to separate concerns and manage command/query logic. 
- Additionally, the Vertical Slice Architecture (VSA) pattern was applied because the API is presumably expected to be a microservice and will not grow much larger, avoiding the boilerplate code typically present in Clean Architecture. While Clean Architecture is a great choice for more complex APIs, VSA offers a simpler, more focused approach for smaller, feature-specific services, without neglecting the separation of concerns. These changes help improve the structure, clarity, and maintainability of the application.
- RestSharp was integrated in test library for easier handling of HTTP requests, as it abstracts much of the HTTP request handling, eliminating the need for custom Get and Post methods.
- FluentValidation (https://docs.fluentvalidation.net/en/latest/aspnet.html#minimal-apis) was used to handle model validation explicitly, as minimal APIs don't provide automatic validation like traditional controllers
    
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





## Database Choice: Relational (PostgreSQL) vs Non-Relational (MongoDB)

### Requirements:
- **Fetch by ID**  
- **Partial Search** by `name` and/or `email` (contains-style, not exact match)  
- **Email Uniqueness** (cannot insert customers with the same email)  
- **Delete** operation to remove customers by ID

| **Requirement**                            | **Relational Database (PostgreSQL)**                           | **Non-Relational Database (MongoDB)**                            |
|--------------------------------------------|----------------------------------------------------------------|------------------------------------------------------------------|
| **Fetch by ID**                            | **Excellent** — Optimized for **exact lookups** using primary key (ID). | **Excellent** — Optimized for **exact lookups** using `_id` or custom indexes. |
| **Partial Search by Name**                 | **Good** — Supports **partial match** with `ILIKE` (e.g., `name ILIKE '%john%'`). | **Good** — Supports **partial match** with regex (`$regex`), but may be slower with large datasets. |
| **Partial Search by Email**                | **Good** — Same as Name, supports `ILIKE` (e.g., `email ILIKE '%john%'`). | **Good** — Same as Name, supports regex (`$regex`), but performance may degrade with large datasets. |
| **Email Uniqueness**                       | **Excellent** — Enforced directly with `UNIQUE` constraint at the database level. | **Less straightforward** — Email uniqueness must be enforced at the **application level** or with additional **unique index**. |
| **ACID Transactions**                      | **Full ACID compliance** — Guarantees strong consistency and transactional integrity. | **Limited ACID compliance** — MongoDB supports transactions, but full ACID guarantees may require replica sets and advanced configuration. |
| **Scalability**                            | **Vertical scaling** (more resources on a single server), **horizontal scaling** is possible with sharding but requires advanced configuration. | **Horizontal scaling** — MongoDB is designed for sharding and scaling across multiple nodes easily. |
| **Data Integrity**                         | **Strong integrity** — Supports foreign keys, constraints, and triggers. | **Weak integrity** — No foreign key support, so data consistency must be managed at the application level. |
| **Delete Operation**                       | **Excellent** — Simple `DELETE` query by `ID` (`DELETE FROM customers WHERE id = ?`). | **Excellent** — Simple `DELETE` operation by `_id` (`db.customers.deleteOne({ _id: id })`). |

### Recommendation:
For the given requirements (ID fetch, partial search, email uniqueness, and delete operation), **PostgreSQL** is the recommended choice due to:
- Native support for partial text search with `ILIKE`.
- Strong **email uniqueness** enforcement at the database level.
- **ACID compliance** ensuring data consistency and integrity.
- Simple and efficient **DELETE** operations with direct support for relational integrity.

While **MongoDB** could be a viable option, it is better suited for cases requiring schema flexibility or horizontal scaling, and requires more work to ensure email uniqueness and optimized partial text search performance.

While **MongoDB** could be a viable option, it is better suited for cases requiring schema flexibility or horizontal scaling, and requires more work to ensure email uniqueness and optimized partial text search performance.
