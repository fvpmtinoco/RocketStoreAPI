using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using RestSharp;
using System;
using System.IO;
using System.Net.Http;
using Xunit;

namespace RocketStoreApi.Tests
{
    /// <summary>
    /// Defines a test fixture used to test the <see cref="CustomersController"/>.
    /// </summary>
    /// <seealso cref="IDisposable" />
    [CollectionDefinition("CustomersAPI")]
    public sealed partial class CustomersFixture : ICollectionFixture<CustomersFixture>
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

        public RestClient RestClient { get; private set; }
        private HttpClient client { get; set; }


        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="CustomersFixture"/> class.
        /// </summary>
        public CustomersFixture()
        {
            // Create a WebApplicationFactory for the minimal API
            WebApplicationFactory<Program> factory = new WebApplicationFactory<Program>()
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
            client = factory.CreateClient();

            RestClient = new RestClient(client);
        }

        #endregion

        #region Public Methods

        /// <inheritdoc />
        public void Dispose()
        {
            this.Dispose(disposing: true);
        }

        #endregion

        #region Private Methods

        private void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    if (this.client != null)
                    {
                        this.client.Dispose();
                    }

                    if (this.Server != null)
                    {
                        this.Server.Dispose();
                    }

                    if (this.RestClient != null)
                    {
                        this.RestClient = null;
                    }
                }

                this.disposed = true;
            }
        }

        #endregion
    }
}
