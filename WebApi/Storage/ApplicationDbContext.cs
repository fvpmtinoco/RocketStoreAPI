using Microsoft.EntityFrameworkCore;
using RocketStoreApi.Entities;

namespace RocketStoreApi.Storage
{
    /// <summary>
    /// Defines the application database context.
    /// </summary>
    /// <seealso cref="Microsoft.EntityFrameworkCore.DbContext" />
    public partial class ApplicationDbContext : DbContext
    {
        #region Public Properties

        /// <summary>
        /// Gets or sets the customers database set.
        /// </summary>
        public DbSet<Customer> Customers { get; set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ApplicationDbContext"/> class.
        /// </summary>
        /// <param name="context">The database context.</param>
        public ApplicationDbContext(DbContextOptions context)
            : base(context)
        {
        }

        #endregion

        #region Protected Methods

        /// <inheritdoc/>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Customers

            modelBuilder.Entity<Customer>().Property(x => x.Id);
            modelBuilder.Entity<Customer>().Property(t => t.Name).HasMaxLength(200).IsRequired();
            modelBuilder.Entity<Customer>().Property(t => t.Email).HasMaxLength(200).IsRequired();
            modelBuilder.Entity<Customer>().Property(t => t.VatNumber).HasMaxLength(9);
            modelBuilder.Entity<Customer>().HasIndex(c => c.Email).IsUnique();
        }

        #endregion
    }
}
