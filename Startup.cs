using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json.Serialization;
using SysDomain;
using SysUtility.Config;
using SysUtility.Config.Models;
using SysUtility.Helpers;
using WeghingSystemCore.Extensions;

namespace WeghingSystemCore
{
    public class Startup
    {
        public IHostEnvironment HostEnvironment { get; }
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration, IHostEnvironment hostEnvironment)
        {
            Configuration = configuration;
            HostEnvironment = hostEnvironment;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AllowCORS();

            services.ConfigureWritable<AppConfig>(Configuration.GetSection("ApplicationSettings"), defaultValues: AppConfig.GetDefault());

            services.AddDbContextService(Configuration);

            services.AddRepositoryService();

            services.DisableAutoValidate();

            /**
             * HAS ERROR
             */
            //services.ConfigureSessions();

            //services.AddMvc(setupAction =>
            //{
            //    setupAction.EnableEndpointRouting = false;
            //}).AddJsonOptions(jsonOptions =>
            //{
            //    jsonOptions.JsonSerializerOptions.PropertyNamingPolicy = null;
            //})
            //.SetCompatibilityVersion(CompatibilityVersion.Version_3_0);


            services.AddControllers(setupAction =>
            {
                setupAction.EnableEndpointRouting = false;
            }).AddNewtonsoftJson(options =>
            {
                options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
                options.SerializerSettings.ContractResolver = new DefaultContractResolver();
            })
            .SetCompatibilityVersion(CompatibilityVersion.Version_3_0);
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            CustomCultureHelpers.SetCustomCulture();

            app.UseCors();

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }

    }
}
