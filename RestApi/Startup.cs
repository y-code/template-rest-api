using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Versioning.Conventions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace RestApi
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();

            services.AddApiVersioning(options =>
            {
                options.Conventions.Add(new VersionByNamespaceConvention());
                options.ReportApiVersions = true;
            });

            services.AddVersionedApiExplorer(options =>
            {
                options.GroupNameFormat = "'ver. 'V";
                options.SubstituteApiVersionInUrl = true;
            });

            services.AddOpenApiDocument(settings =>
            {
                settings.DocumentName = "v1";
                settings.Version = "1.0.0";
                settings.GenerateExamples = false;
                settings.UseRouteNameAsOperationId = true;
                settings.Title = "SWAGGER-TITLE";
                settings.ApiGroupNames = new[] { "ver. 1" };
            });
            services.AddOpenApiDocument(settings =>
            {
                settings.DocumentName = "v2";
                settings.Version = "2.0.0";
                settings.Title = "SWAGGER-TITLE";
                settings.ApiGroupNames = new[] { "ver. 2" };
            });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseExceptionHandler("/api/error/dev");
            }
            else
            {
                app.UseExceptionHandler("/api/error");
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller}/{action=Index}/{id?}");
            });

            app.UseApiVersioning();
            app.UseOpenApi();
            app.UseSwaggerUi3();
        }
    }
}
