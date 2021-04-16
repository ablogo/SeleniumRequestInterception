using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SeleniumRequestInterception.Dtos;
using SeleniumRequestInterception.Interfaces;
using SeleniumRequestInterception.Services;
using System;
using System.IO;

namespace SeleniumRequestInterception
{
    public class Program
    {
        static void Main(string[] args)
        {
            var services = ConfigureServices();
            var serviceProvider = services.BuildServiceProvider();

            Console.WriteLine("Enter a valid Url");

            string url = Console.ReadLine();

            serviceProvider.GetService<IChrome>().GoToPage(url).Wait();

            Console.WriteLine("Press any key to exit");
            Console.ReadKey();
        }

        private static IServiceCollection ConfigureServices()
        {
            IServiceCollection services = new ServiceCollection();
            try
            {
                var config = LoadConfiguration();
                services.AddSingleton(config);

                services.Configure<ChromeDriverConfiguration>(config.GetSection("ChromeDriver"));
                services.AddScoped<IChrome, ChromeService>();
            }
            catch (Exception ex) { }
            return services;
        }

        private static IConfiguration LoadConfiguration()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

            return builder.Build();
        }
    }
}
