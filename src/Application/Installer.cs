using System.Reflection;
using Application.models;
using Application.Services;
using Application.Services.StockApi;
using Domain;
using Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Application;
public class Installer : IExtensionsInstaller
{
    public void InstallServices(IServiceCollection services, IConfiguration config)
    {
        services.AddScoped<IStockApi, StockApiService>();
        services.AddScoped<ISlickChartsScraper, SlickChartsScraper>();
        services.AddScoped<IYahooFinanceScraper, YahooFinanceScraper>();
        services.AddConfigurations(config.GetSection(nameof(StockScraperOptions)), new StockScraperOptions());
        services.AddConfigurations(config.GetSection(nameof(StockApiOptions)), new StockApiOptions());
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));
    }
}
