using Microsoft.EntityFrameworkCore;
using RocketStoreApi.CQRS;
using RocketStoreApi.Database.Entities;
using RocketStoreApi.Storage;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace RocketStoreApi.Features.GetCustomers
{
    public record GetCustomerByIdResult(Customer? Customer);

    public class GetCustomerByIdQuery(Guid id) : IQuery<GetCustomerByIdResult>
    {
        public readonly Guid Id = id;
    }

    public class GetCustomerByIdQueryHandler(ApplicationDbContext context) : IQueryHandler<GetCustomerByIdQuery, GetCustomerByIdResult>
    {
        public async Task<GetCustomerByIdResult> Handle(GetCustomerByIdQuery request, CancellationToken cancellationToken)
        {
            var query = context.Customers.AsQueryable();

            query = query.Where(c => c.Id == request.Id);

            var customer = await query.SingleOrDefaultAsync(cancellationToken);

            return new GetCustomerByIdResult(customer);
        }
    }
}
