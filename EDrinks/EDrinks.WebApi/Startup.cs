using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using EDrinks.Common;
using EDrinks.Events;
using EDrinks.EventSourceSql;
using EDrinks.QueryHandlers;
using EDrinks.QueryHandlers.Model;
using EDrinks.WebApi.Services;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace EDrinks.WebApi
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;

            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(Configuration)
                .CreateLogger();
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddScoped<IStreamResolver, StreamResolver>();
            services.AddScoped<IEventSourceFacade, EventSourceFacade>();
            services.AddScoped<IReadModel, ReadModel>();
            services.AddScoped<IDataContext, DataContext>();
            services.AddSingleton<IEventLookup, EventLookup>();
            services.AddSingleton<IDatabaseLookup, DatabaseLookup>();

            var assemblies = (new[] {"EDrinks.QueryHandlers", "EDrinks.CommandHandlers"})
                .Select(assemblyName => Assembly.Load(assemblyName));
            services.AddMediatR(assemblies);

            ConfigureAuth(services);

            services.Configure<RequestLocalizationOptions>(options =>
            {
                options.DefaultRequestCulture = new RequestCulture("en-US");
            });

            services.AddCors();
            services.AddControllers();
            services.AddHttpContextAccessor();
        }

        protected virtual void ConfigureAuth(IServiceCollection services)
        {
            string domain = $"https://{Configuration["Auth0:Domain"]}/";
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
            {
                options.Authority = domain;
                options.Audience = Configuration["Auth0:ApiIdentifier"];
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            var origins = new List<string>();
            Configuration.GetSection("AppSettings").GetSection("AllowedOrigins").Bind(origins);
            app.UseCors(builder => builder.WithOrigins(origins.ToArray()).AllowAnyHeader().AllowAnyMethod());

            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
            app.UseRequestLocalization();
        }
    }
}