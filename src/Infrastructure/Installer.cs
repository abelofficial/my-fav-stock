
using Amazon.DynamoDBv2;
using Amazon.S3;
using Application.Contracts;
using Domain;
using Domain.Interfaces;
using Infrastructure.Cache;
using Infrastructure.CMS;
using Infrastructure.CMS.Models;
using Infrastructure.OpenSearch;
using Infrastructure.S3Bucket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure
{
    public class Installer : IExtensionsInstaller
    {
        private IServiceCollection _services;
        private IConfiguration _config;
        public void InstallServices(IServiceCollection services, IConfiguration config)
        {
            _services = services;
            _config = config;
            InstallPersistenceServices(config);
        }

        private void InstallPersistenceServices(IConfiguration config)
        {

            _services.AddScoped<IStockBucket, StockBucket>();
            _services.AddTransient<IAmazonS3>(sp =>
            {
                var config = new AmazonS3Config
                {
                    RegionEndpoint = Amazon.RegionEndpoint.EUNorth1
                };
                return new AmazonS3Client(config);
            });

            _services.AddSingleton<IAmazonDynamoDB>(sp =>
            {
                var config = new AmazonDynamoDBConfig
                {
                    RegionEndpoint = Amazon.RegionEndpoint.EUNorth1
                };
                return new AmazonDynamoDBClient(config);
            });


            // _services.AddConfigurations(config.GetSection(nameof(OpenSearchOptions)), new OpenSearchOptions());
            _services.AddConfigurations(config.GetSection(nameof(CMSSettings)), new CMSSettings());


            _services.AddHttpClient<ICmsService, DatoCmsService>();
            _services.AddTransient<ICmsService, DatoCmsService>();

            _services.AddSingleton<ICache, CacheRepository>();

            _services.AddSingleton<IElasticDataConsumer, ElasticDataConsumer>();
            _services.AddSingleton<IElasticClientWrapper, ElasticClientWrapper>();
            _services.AddSingleton<IElasticIndexManager, ElasticIndexManager>();
            _services.AddSingleton<IIndicesNamespaceWrapper, IndicesNamespaceWrapper>();
            _services.AddSingleton<IIndexResponseWrapper, IndexResponseWrapper>();
            _services.AddSingleton<IOpenSearchClientProvider, OpenSearchClientProvider>();
        }
    }
}