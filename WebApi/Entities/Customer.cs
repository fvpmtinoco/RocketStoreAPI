using System;

namespace RocketStoreApi.Entities
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
        public Guid Id
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the customer name.
        /// </summary>
        public string Name
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the customer email address.
        /// </summary>
        public string Email
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the customer VAT number.
        /// </summary>
        public string VatNumber
        {
            get;
            set;
        }

        #endregion
    }
}
