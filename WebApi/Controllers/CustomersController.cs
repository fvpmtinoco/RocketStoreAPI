using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using RocketStoreApi.Managers;
using RocketStoreApi.Models;
using System;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Threading.Tasks;

namespace RocketStoreApi.Controllers
{
    /// <summary>
    /// Defines the customers controller.
    /// This controller provides actions on customers.
    /// </summary>
    [ApiController]
    public partial class CustomersController : ControllerBase
    {
        // Ignore Spelling: api

        #region Public Methods

        /// <summary>
        /// Creates the specified customer.
        /// </summary>
        /// <param name="customer">The customer that should be created.</param>
        /// <returns>
        /// The new customer identifier.
        /// </returns>
        [HttpPost("api/customers")]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.Conflict)]
        [ProducesResponseType(typeof(Guid), (int)HttpStatusCode.Created)]
        public async Task<IActionResult> CreateCustomerAsync([FromBody, Required] Customer customer)
        {
            if (customer != null)
            {
                var manager = this.HttpContext.RequestServices.GetRequiredService<ICustomersManager>();
                Result<Guid> result = await manager.CreateCustomerAsync(customer);

                if (result.FailedWith(ErrorCodes.CustomerAlreadyExists))
                {
                    return this.Conflict(
                        new ProblemDetails()
                        {
                            Status = (int)HttpStatusCode.Conflict,
                            Title = result.ErrorCode,
                            Detail = result.ErrorDescription
                        });
                }
                else if (result.Failed)
                {
                    return this.BadRequest(
                        new ProblemDetails()
                        {
                            Status = (int)HttpStatusCode.BadRequest,
                            Title = result.ErrorCode,
                            Detail = result.ErrorDescription
                        });
                }

                return this.Created(
                    this.GetUri("customers", result.Value),
                    result.Value);
            }

            return this.BadRequest(
                new ProblemDetails()
                {
                    Status = (int)HttpStatusCode.BadRequest,
                    Title = "Invalid customer",
                    Detail = "The customer is invalid."
                });
        }

        #endregion

        #region Private Methods

        private Uri GetUri(params object[] parameters)
        {
            string result = $"{this.Request.Scheme}://{this.Request.Host}{this.Request.PathBase}";

            foreach (object pathParam in parameters)
            {
                result = $"{result}/{pathParam}";
            }

            return new Uri(result);
        }

        #endregion
    }
}
