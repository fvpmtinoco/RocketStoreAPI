using Microsoft.EntityFrameworkCore;
using RocketStoreApi.CQRS;
using RocketStoreApi.Database.Entities;
using RocketStoreApi.Storage;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace RocketStoreApi.Features.GetCustomers
{
    public record GetCustomersResult(Customer Customer);

    public class GetCustomersQuery(string? name, string? email) : IQuery<IEnumerable<GetCustomersResult>>
    {
        public readonly string? Name = name;
        public readonly string? Email = email;
    }

    public class GetCustomersQueryHandler(ApplicationDbContext context) : IQueryHandler<GetCustomersQuery, IEnumerable<GetCustomersResult>>
    {
        public async Task<IEnumerable<GetCustomersResult>> Handle(GetCustomersQuery request, CancellationToken cancellationToken)
        {
            var query = context.Customers.AsQueryable();

            // Apply filtering based on Name if provided
            if (!string.IsNullOrWhiteSpace(request.Name))
                query = query.Where(c => c.Name.Contains(request.Name.ToLowerInvariant()));

            // Apply filtering based on Email if provided
            if (!string.IsNullOrEmpty(request.Email))
                query = query.Where(c => c.Email.Contains(request.Email.ToLowerInvariant()));

            // Execute the query and return the result
            var result = await query.ToListAsync(cancellationToken);
            return result.Select(c => new GetCustomersResult(c));
        }
    }
}
