using System.Linq;
using System.Reflection;
using EDrinks.Common.Config;
using EDrinks.EventSource;
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
            services.AddSingleton<IEventSourceFacade, EventSourceFacade>();
            
            var assemblies = Assembly.GetExecutingAssembly()
                .GetReferencedAssemblies()
                .Select(assemblyName => Assembly.Load(assemblyName))
                .ToList();
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