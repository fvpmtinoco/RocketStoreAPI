using Microsoft.EntityFrameworkCore;
using RocketStoreApi.CQRS;
using RocketStoreApi.Entities;
using RocketStoreApi.Storage;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace RocketStoreApi.Customers.GetCustomers
{
    public class GetCustomerByIdQuery(Guid id) : IQuery<Customer?>
    {
        public readonly Guid Id = id;
    }

    public class GetCustomerByIdQueryHandler(ApplicationDbContext context) : IQueryHandler<GetCustomerByIdQuery, Customer?>
    {
        private readonly ApplicationDbContext context = context;
        public async Task<Customer?> Handle(GetCustomerByIdQuery request, CancellationToken cancellationToken)
        {
            var query = context.Customers.AsQueryable();

            query = query.Where(c => c.Id == request.Id);

            var customer = await query.SingleOrDefaultAsync(cancellationToken);

            return customer;
        }
    }
}
