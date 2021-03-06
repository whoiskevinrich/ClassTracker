﻿using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using KadGen.ClassTracker.Service;
using KadGen.ClassTracker.Repository;

namespace KadGen.ClassTracker.WebApi
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Add framework services.
            services.AddMvc();
            var connString = Configuration["Data:DefaultConnection:ClassTrackerConnectionString"];
            var dbContext = new ClassTrackerDbContext(connString);
            //services.AddScoped<ClassTrackerDbContext>
            //    (serviceProvider => new ClassTrackerDbContext
            //        (Configuration["Data:DefaultConnection:ClassTrackerConnectionString"]));
            services.AddScoped(serviceProvider => new OrganizationService(dbContext));

            //var connection = @"Server=.\\..\\Database\\mssqllocaldb;Database=ClassTracker;Trusted_Connection=True;MultipleActiveResultSets=true";

            //services.AddScoped<ClassTrackerDbContext>(_ => new ClassTrackerDbContext(Configuration.GetConnectionString("DefaultConnection")));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            app.UseMvc();
        }
    }
}
