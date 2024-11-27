using AutoMapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RocketStoreApi.Managers;
using RocketStoreApi.Storage;

namespace RocketStoreApi
{
    /// <summary>
    /// Defines the startup of the application.
    /// </summary>
    public partial class Startup
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="Startup"/> class.
        /// </summary>
        public Startup()
        {
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Configures the services required by the application.
        /// </summary>
        /// <param name="services">The service collection.</param>
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseInMemoryDatabase("RocketStoreApiDb"));

            services.AddControllers();

            services.AddOpenApiDocument(
                (options) =>
                {
                    options.DocumentName = "Version 1";
                    options.Title = "RocketStore API";
                    options.Description = "REST API for the RocketStore Web Application";
                });

            // Register AutoMapper and scan the current assembly for profiles
            services.AddAutoMapper(typeof(Startup));

            services.AddScoped<ICustomersManager, CustomersManager>();

            services.AddScoped<Profile, MappingProfile>();
        }

        /// <summary>
        /// Configures the application.
        /// </summary>
        /// <param name="app">The application.</param>
        /// <param name="env">The environment.</param>
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseOpenApi();

            app.UseSwaggerUi();

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }

        #endregion
    }
}
