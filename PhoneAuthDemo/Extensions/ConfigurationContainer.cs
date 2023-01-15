using Microsoft.OpenApi.Models;
using PhoneAuthDemo.Repositories;
using PhoneAuthDemo.Services;
using System.Reflection;

namespace PhoneAuthDemo.Extensions
{
    /// <summary>
    /// To add the configurations
    /// </summary>
    public static class ConfigurationContainer
    {
        /// <summary>
        /// Add swagger in startup
        /// </summary>
        /// <param name="serviceCollection">Collection of service</param>
        public static void AddCustomSwaggerOpenAPI(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddSwaggerGen(setupAction =>
            {

                setupAction.SwaggerDoc(
                    "OpenAPISpecification",
                    new OpenApiInfo()
                    {
                        Title = "WebAPI",
                        Version = "1",
                        Description = "Through this API you can access all API details"
                    });

                var xmlCommentsFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlCommentsFullPath = Path.Combine(AppContext.BaseDirectory, xmlCommentsFile);
                setupAction.IncludeXmlComments(xmlCommentsFullPath);

            });
        }

        /// <summary>
        /// To add the swageger in middleware
        /// </summary>
        /// <param name="app">Application builder</param>
        public static void UseCustomSwagger(this IApplicationBuilder app)
        {
            app.UseSwagger();

            app.UseSwaggerUI(setupAction =>
            {
                setupAction.SwaggerEndpoint("/swagger/OpenAPISpecification/swagger.json", "API");
                setupAction.RoutePrefix = "OpenAPI";
            });
        }

        /// <summary>
        /// Add scoped services
        /// </summary>
        /// <param name="serviceCollection">Collection of service</param>
        public static void AddScopedServices(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddScoped<IUserService, UserRepository>();
        }

    }
}
