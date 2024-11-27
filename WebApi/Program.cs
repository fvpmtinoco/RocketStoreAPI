using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace RocketStoreApi
{
    /// <summary>
    /// Defines the entry point of the application.
    /// </summary>
    public static partial class Program
    {
        #region Public Methods

        /// <summary>
        /// Defines the entry point of the application.
        /// </summary>
        /// <param name="args">The arguments.</param>
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        #endregion

        #region Private Methods

        private static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });

        #endregion
    }
}
