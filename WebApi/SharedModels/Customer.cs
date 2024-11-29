using FluentValidation;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace RocketStoreApi.SharedModels
{
    /// <summary>
    /// Defines a customer.
    /// </summary>
    public partial class Customer
    {
        #region Public Properties

        /// <summary>
        /// Gets or sets the customer name.
        /// </summary>
        [Required(AllowEmptyStrings = false)]
        [DisplayName("Name")]
        [JsonPropertyName("name")]
        public string Name { get; set; } = default!;

        /// <summary>
        /// Gets or sets the customer email address.
        /// </summary>
        [Required(AllowEmptyStrings = false)]
        [EmailAddress]
        [DisplayName("Email")]
        [JsonPropertyName("emailAddress")]
        public string EmailAddress { get; set; } = default!;

        /// <summary>
        /// Gets or sets the customer VAT number.
        /// </summary>
        [RegularExpression("^[0-9]{9}$")]
        [DisplayName("VAT Number")]
        [JsonPropertyName("vatNumber")]
        public string? VatNumber { get; set; }

        /// <summary>
        /// Gets or sets the customer address.
        /// </summary>
        [DisplayName("Address")]
        [JsonPropertyName("address")]
        public string? Address { get; set; }

        #endregion
    }

    /// <summary>
    /// Validates the <see cref="Customer"/> model.
    /// This validator ensures that the <see cref="Customer"/> properties are valid
    /// </summary>
    public class CustomerValidator : AbstractValidator<Customer>
    {
        public CustomerValidator()
        {
            RuleFor(x => x.Name).NotNull().NotEmpty();
            RuleFor(x => x.EmailAddress).NotNull().NotEmpty();
            RuleFor(x => x.EmailAddress).EmailAddress();
            RuleFor(x => x.VatNumber).Matches(@"^[0-9]{9}$");
        }
    }
}
