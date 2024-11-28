using Microsoft.EntityFrameworkCore;
using RocketStoreApi.CQRS;
using RocketStoreApi.Entities;
using RocketStoreApi.Storage;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace RocketStoreApi.Customers.GetCustomers
{
    public class GetCustomersQuery(string? name, string? email) : IQuery<IEnumerable<Customer>>
    {
        public readonly string? Name = name;
        public readonly string? Email = email;
    }

    public class GetCustomersQueryHandler(ApplicationDbContext context) : IQueryHandler<GetCustomersQuery, IEnumerable<Customer>>
    {
        private readonly ApplicationDbContext context = context;
        public async Task<IEnumerable<Customer>> Handle(GetCustomersQuery request, CancellationToken cancellationToken)
        {
            var query = context.Customers.AsQueryable();

            // Apply filtering based on Name if provided
            if (!string.IsNullOrWhiteSpace(request.Name))
                query = query.Where(c => c.Name.Contains(request.Name.ToLowerInvariant()));

            // Apply filtering based on Email if provided
            if (!string.IsNullOrEmpty(request.Email))
                query = query.Where(c => c.Email.Contains(request.Email.ToLowerInvariant()));

            // Execute the query and return the result
            return await query.ToListAsync(cancellationToken);
        }
    }
}
