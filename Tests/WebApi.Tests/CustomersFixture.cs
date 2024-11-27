using System;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using RocketStoreApi.Controllers;

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

        private HttpClient Client
        {
            get;
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="CustomersFixture"/> class.
        /// </summary>
        public CustomersFixture()
        {
            IConfigurationBuilder configurationBuilder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .AddEnvironmentVariables();

            IConfigurationRoot configuration = configurationBuilder.Build();

            IWebHostBuilder webHostBuilder = new WebHostBuilder()
                .UseEnvironment("Development")
                .UseConfiguration(configuration)
                .UseStartup<Startup>();
            
            this.Server = new TestServer(webHostBuilder);

            this.Client = this.Server.CreateClient();
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
