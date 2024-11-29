using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using RocketStoreApi.CQRS;
using RocketStoreApi.Managers;
using RocketStoreApi.Storage;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace RocketStoreApi.Customers.CreateCustomer
{
    public record CreateCustomerCommand(string Name, string EmailAddress, string? VatNumber) : ICommand<Result<CreateCustomerResult, CreateCustomerErrorCodes>>;
    public record CreateCustomerResult(Guid Id);

    public class CreateCustomerHandler(ApplicationDbContext context, ILogger<CreateCustomerHandler> logger) : ICommandHandler<CreateCustomerCommand, Result<CreateCustomerResult, CreateCustomerErrorCodes>>
    {
        private readonly ILogger<CreateCustomerHandler> logger = logger;

        public async Task<Result<CreateCustomerResult, CreateCustomerErrorCodes>> Handle(CreateCustomerCommand command, CancellationToken cancellationToken)
        {
            Entities.Customer entity = new Entities.Customer
            {
                Name = command.Name,
                Email = command.EmailAddress,
                VatNumber = command.VatNumber
            };

            // Check if the customer already exists
            if (await context.Customers.AnyAsync(i => i.Email == entity.Email, cancellationToken))
            {
                logger.LogWarning($"A customer with email '{entity.Email}' already exists.");
                return Result<CreateCustomerResult, CreateCustomerErrorCodes>.Failure(CreateCustomerErrorCodes.CustomerAlreadyExists, $"A customer with email '{entity.Email}' already exists.");
            }

            context.Customers.Add(entity);

            await context.SaveChangesAsync(cancellationToken);

            logger.LogInformation($"Customer '{entity.Name}' created successfully.");

            return Result<CreateCustomerResult, CreateCustomerErrorCodes>.Success(new CreateCustomerResult(entity.Id));
        }
    }
}
