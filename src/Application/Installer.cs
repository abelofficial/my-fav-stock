﻿using System.Reflection;
using Application.models;
using Application.Services;
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
        services.AddScoped<IStockApi, StockApi>();
        services.AddScoped<ISlickChartsScraper, SlickChartsScraper>();
        services.AddScoped<IYahooFinanceScraper, YahooFinanceScraper>();
        services.AddConfigurations(config.GetSection(nameof(StockScraperOptions)), new StockScraperOptions());
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));
    }
}
