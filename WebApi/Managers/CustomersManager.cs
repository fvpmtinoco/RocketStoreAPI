using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using RocketStoreApi.Storage;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace RocketStoreApi.Managers
{
    /// <summary>
    /// Defines the default implementation of <see cref="ICustomersManager"/>.
    /// </summary>
    /// <seealso cref="ICustomersManager" />
    internal partial class CustomersManager : ICustomersManager
    {
        #region Private Properties

        private ApplicationDbContext Context
        {
            get;
        }

        private IMapper Mapper
        {
            get;
        }

        private ILogger Logger
        {
            get;
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="CustomersManager" /> class.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="mapper">The mapper.</param>
        /// <param name="logger">The logger.</param>
        public CustomersManager(ApplicationDbContext context, IMapper mapper, ILogger<CustomersManager> logger)
        {
            this.Context = context;
            this.Mapper = mapper;
            this.Logger = logger;
        }

        #endregion

        #region Public Methods

        /// <inheritdoc />
        public async Task<Result<Guid>> CreateCustomerAsync(Models.Customer customer, CancellationToken cancellationToken = default)
        {
            customer = customer ?? throw new ArgumentNullException(nameof(customer));

            Entities.Customer entity = this.Mapper.Map<Models.Customer, Entities.Customer>(customer);

            if (await Context.Customers.AnyAsync(i => i.Email == entity.Email, cancellationToken))
            {
                this.Logger.LogWarning($"A customer with email '{entity.Email}' already exists.");

                return Result<Guid>.Failure(
                    ErrorCodes.CustomerAlreadyExists,
                    $"A customer with email '{entity.Email}' already exists.");
            }

            this.Context.Customers.Add(entity);

            try
            {
                await this.Context.SaveChangesAsync(cancellationToken);
            }
            catch (Exception)
            {

            }

            this.Logger.LogInformation($"Customer '{customer.Name}' created successfully.");

            return Result<Guid>.Success(entity.Id);
        }

        #endregion
    }
}