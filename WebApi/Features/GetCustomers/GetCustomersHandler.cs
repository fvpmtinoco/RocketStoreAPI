using MediatR;
using Microsoft.EntityFrameworkCore;
using RocketStoreApi.Configurations;
using RocketStoreApi.SharedModels;
using RocketStoreApi.Storage;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace RocketStoreApi.Features.GetCustomers
{
    public record GetCustomersQueryResult(List<CustomerDTO> Customers, int TotalCount);

    public class GetCustomersQuery(string? name, string? email, int pageNumber = 1, int pageSize = 10) : IRequest<Result<GetCustomersQueryResult, GetCustomersByIdErrorCodes>>
    {
        public readonly string? Name = name;
        public readonly string? Email = email;
        public readonly int pageNumber = pageNumber;
        public readonly int pageSize = pageSize;
    }

    internal class GetCustomersQueryHandler(ApplicationDbContext context) : IRequestHandler<GetCustomersQuery, Result<GetCustomersQueryResult, GetCustomersByIdErrorCodes>>
    {
        public async Task<Result<GetCustomersQueryResult, GetCustomersByIdErrorCodes>> Handle(GetCustomersQuery request, CancellationToken cancellationToken)
        {
            // Pagination calculation: Skip the first (PageNumber - 1) * PageSize customers and take PageSize customers
            // Example 1: If PageNumber = 1 and PageSize = 10, skip = (1 - 1) * 10 = 0, 
            // meaning no records are skipped, and the first 10 customers are returned.
            // Example 2: If PageNumber = 3 and PageSize = 10, skip = (3 - 1) * 10 = 20, 
            // meaning the first 20 customers are skipped, and customers 21-30 are returned.
            var skip = (request.pageNumber - 1) * request.pageSize;

            var query = context.Customers.AsQueryable();

            // Apply filtering based on Name if provided
            if (!string.IsNullOrWhiteSpace(request.Name))
                query = query.Where(c => c.Name.ToLowerInvariant().Contains(request.Name.ToLowerInvariant()));

            // Apply filtering based on Email if provided
            if (!string.IsNullOrEmpty(request.Email))
                query = query.Where(c => c.Email.Contains(request.Email.ToLowerInvariant()));

            query = query.OrderBy(c => c.Id);

            // Get the total number of customers (without pagination)
            var totalCount = await query.CountAsync(cancellationToken);

            // Execute the query and return the result
            var result = await query
                .Skip(skip)
                .Take(request.pageSize)
                .Select(x => new CustomerDTO
                {
                    Name = x.Name,
                    EmailAddress = x.Email,
                    VatNumber = x.VatNumber,
                    Address = x.Address
                }).ToListAsync(cancellationToken);

            return Result<GetCustomersQueryResult, GetCustomersByIdErrorCodes>.Success(new GetCustomersQueryResult(result, totalCount));
        }
    }
}
