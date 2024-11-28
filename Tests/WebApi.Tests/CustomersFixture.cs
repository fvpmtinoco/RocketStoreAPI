using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using RocketStoreApi.Controllers;
using System;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace RocketStoreApi.Tests
{
    /// <summary>
    /// Defines a test fixture used to test the <see cref="CustomersController"/>.
    /// </summary>
    /// <seealso cref="IDisposable" />
    public sealed partial class CustomersFixture : IDisposable
    {
        // Ignore Spelling: json

        #region Fields

        private bool disposed;

        #endregion

        #region Private Properties

        private TestServer Server
        {
            get;
        }

        public HttpClient Client { get; private set; }
        public WebApplicationFactory<Program> Factory { get; private set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="CustomersFixture"/> class.
        /// </summary>
        public CustomersFixture()
        {
            // Create a WebApplicationFactory for the minimal API
            this.Factory = new WebApplicationFactory<Program>()
                .WithWebHostBuilder(builder =>
                {
                    // Configure additional services or settings needed for the test environment
                    builder.ConfigureAppConfiguration((context, config) =>
                    {
                        config.SetBasePath(Directory.GetCurrentDirectory())
                              .AddJsonFile("appsettings.json")
                              .AddEnvironmentVariables();
                    });

                    // Optionally, override services or add mocks for testing, e.g., replace DB context or MediatR services
                    builder.ConfigureServices(services =>
                    {
                        // Mock any dependencies if necessary
                    });
                });

            // Create the test client from the factory
            this.Client = this.Factory.CreateClient();
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Send a post request.
        /// </summary>
        /// <typeparam name="T">The model type.</typeparam>
        /// <param name="endpointPath">The endpoint path.</param>
        /// <param name="model">The model instance.</param>
        /// <returns>
        /// The <see cref="Task{TResult}"/> that represents the asynchronous operation.
        /// The <see cref="HttpResponseMessage"/> instance.
        /// </returns>
        public async Task<HttpResponseMessage> PostAsync<T>(string endpointPath, T model)
        {
            string json = JsonSerializer.Serialize(model);

            using StringContent content = new StringContent(
                json,
                Encoding.UTF8,
                "application/json");

            return await this.Client.PostAsync(
                new Uri($"{this.Server.BaseAddress}{endpointPath}"),
                content)
                .ConfigureAwait(false);
        }

        /// <inheritdoc />
        public void Dispose()
        {
            this.Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        #endregion

        #region Private Methods

        private void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    if (this.Client != null)
                    {
                        this.Client.Dispose();
                    }

                    if (this.Server != null)
                    {
                        this.Server.Dispose();
                    }
                }

                this.disposed = true;
            }
        }

        #endregion
    }
}
