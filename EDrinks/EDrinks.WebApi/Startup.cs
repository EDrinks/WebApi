using System.Linq;
using System.Net;
using System.Reflection;
using EDrinks.Common;
using EDrinks.Common.Config;
using EDrinks.Events;
using EDrinks.EventSource;
using EDrinks.QueryHandlers;
using EDrinks.WebApi.Services;
using EventStore.ClientAPI;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
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
            services.Configure<EventStoreConfig>(options => Configuration.GetSection("EventStore").Bind(options));

            services.AddScoped<IStreamResolver, StreamResolver>();
            services.AddSingleton<IEventStoreConnection>(serviceProvider =>
            {
                var settings = ConnectionSettings.Create();
                var connection = EventStoreConnection.Create(settings, new IPEndPoint(
                    IPAddress.Parse(Configuration.GetValue<string>("EventStore:IPAddress")),
                    Configuration.GetValue<int>("EventStore:Port")));
                connection.ConnectAsync().Wait();

                return connection;
            });
            services.AddScoped<IEventSourceFacade, EventSourceFacade>();
            services.AddScoped<IReadModel, ReadModel>();
            services.AddSingleton<IEventLookup, EventLookup>();

            var assemblies = (new[] {"EDrinks.QueryHandlers", "EDrinks.CommandHandlers"})
                .Select(assemblyName => Assembly.Load(assemblyName));
            services.AddMediatR(assemblies);

            services.AddMvc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseMvc();
        }
    }
}