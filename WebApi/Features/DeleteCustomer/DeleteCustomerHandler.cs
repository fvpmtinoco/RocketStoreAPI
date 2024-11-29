using MediatR;
using Microsoft.Extensions.Logging;
using RocketStoreApi.Configurations;
using RocketStoreApi.CQRS;
using RocketStoreApi.Storage;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace RocketStoreApi.Features.DeleteCustomer
{
    public record DeleteCustomerCommand(Guid id) : ICommand<Result<Unit, DeleteCustomerErrorCodes>>;

    public class DeleteCustomerHandler(ApplicationDbContext context, ILogger<DeleteCustomerHandler> logger) : ICommandHandler<DeleteCustomerCommand, Result<Unit, DeleteCustomerErrorCodes>>
    {
        public async Task<Result<Unit, DeleteCustomerErrorCodes>> Handle(DeleteCustomerCommand request, CancellationToken cancellationToken)
        {
            var entity = (await context.Customers.FindAsync(request.id));

            if (entity is null)
            {
                logger.LogWarning($"Customer with id '{request.id}' not found.");
                return Result<Unit, DeleteCustomerErrorCodes>.Failure(DeleteCustomerErrorCodes.InvalidCustomer, $"Customer with id '{request.id}' not found.");
            }

            context.Customers.Remove(entity);
            await context.SaveChangesAsync(cancellationToken);

            return Result<Unit, DeleteCustomerErrorCodes>.Success(Unit.Value);
        }
    }
}
