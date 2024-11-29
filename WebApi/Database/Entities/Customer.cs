using System;

namespace RocketStoreApi.Database.Entities
{
    /// <summary>
    /// Defines a customer.
    /// </summary>
    public partial class Customer
    {
        #region Public Properties

        /// <summary>
        /// Gets or sets the customer identifier.
        /// </summary>
        public Guid Id { get; set; } = default!;

        /// <summary>
        /// Gets or sets the customer name.
        /// </summary>
        public string Name { get; set; } = default!;

        /// <summary>
        /// Gets or sets the customer email address.
        /// </summary>
        public string Email { get; set; } = default!;

        /// <summary>
        /// Gets or sets the customer VAT number.
        /// </summary>
        public string? VatNumber { get; set; }

        /// <summary>
        /// Gets or sets the customer's address.
        /// </summary>
        public string? Address { get; set; }

        #endregion
    }
}
