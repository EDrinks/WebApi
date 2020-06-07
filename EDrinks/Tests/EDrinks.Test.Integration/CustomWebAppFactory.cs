using System.IO;
using System.Linq;
using EDrinks.Common;
using EDrinks.WebApi;
using EDrinks.WebApi.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace EDrinks.Test.Integration
{
    public class CustomWebAppFactory : WebApplicationFactory<Startup>
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                var descriptor = services.SingleOrDefault(
                    d => d.ServiceType ==
                         typeof(StreamResolver));

                if (descriptor != null)
                {
                    services.Remove(descriptor);
                }

                services.AddScoped<IStreamResolver, TestStreamResolver>();
            });

            base.ConfigureWebHost(builder);
        }

        protected override IHostBuilder CreateHostBuilder()
        {
            var builder = Host.CreateDefaultBuilder()
                .ConfigureAppConfiguration(x =>
                {
                    x.AddJsonFile(Path.Join(Directory.GetCurrentDirectory(), "appsettings.json"));
                })
                .ConfigureWebHostDefaults(hostBuilder =>
                {
                    hostBuilder.UseStartup<TestStartup>();
                    // .UseTestServer();
                });

            return builder;
        }
    }
}