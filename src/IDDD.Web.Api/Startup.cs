using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using IDDD.Core.Cqs;
using Microsoft.AspNetCore.Cors.Infrastructure;
using System;
using IDDD.App.Cqs;
using IDDD.Infrastructure;
using IDDD.Logging.Mongo;
using Microsoft.AspNetCore.Http;
using IDDD.Core;
using IDDD.Infrastructure.Mongo;
using IDDD.Web.Api.Infrastructure;
using IDDD.App;

namespace IDDD.Web.Api
{
    public class Startup
    {

        private const string defaultName = "default";

        private readonly IHostingEnvironment _env;

        private readonly IConfigurationRoot _configuration;
        private IServiceProvider _serviceProvider { get; set; }
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();

            builder.AddEnvironmentVariables();

            _configuration = builder.Build();
            _env = env;
        }



        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddTransient<IConfiguration>(_ =>
            {
                return _configuration;
            });

            
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            
            services.AddSharedKernel();
            
            services.AddMongoDBContext(_configuration);
            
            services.AddMongoDomainPersistance(_configuration);

            services.AddIdentityWithMongoStores();

            services.AddAuthentication();

            services.AddMvc(option =>
            {
                option.Filters.Add(new GlobalExceptionFilterFactory());

            });

            services.AddDistributedMemoryCache();

            services.AddSession(options => {
                options.IdleTimeout = TimeSpan.FromMinutes(30);
            });

            services.AddCors(ConfigureCors);
            
            services.AddCqs();

            services.AddMailing(_configuration);

            services.AddCommandHandlers();

            services.AddQueryHandlers();

            //at last build service container
            _serviceProvider = services.BuildServiceProvider();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole((text, logLevel) => logLevel >= LogLevel.Warning);

            loggerFactory.AddMongoLogging<ErrorLog>(_serviceProvider, (text, logLevel) => logLevel >= LogLevel.Warning);
            
            app.ConfigureAuthentication(_configuration);

            app.UseSession();

            app.UseCors(defaultName);

            app.UseMvc();
        }

        private static void ConfigureCors(CorsOptions options)
        {
            options.AddPolicy(defaultName, policy =>
                policy.AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowAnyOrigin()
                    .AllowCredentials());
        }
    }
}
