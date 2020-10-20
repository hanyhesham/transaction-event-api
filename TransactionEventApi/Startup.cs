using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Azure.Storage.Files.Shares;
using Glasswall.Administration.K8.TransactionEventApi.Business.Configuration;
using Glasswall.Administration.K8.TransactionEventApi.Business.Serialisation;
using Glasswall.Administration.K8.TransactionEventApi.Business.Services;
using Glasswall.Administration.K8.TransactionEventApi.Business.Store;
using Glasswall.Administration.K8.TransactionEventApi.Common.Configuration;
using Glasswall.Administration.K8.TransactionEventApi.Common.Configuration.Validation;
using Glasswall.Administration.K8.TransactionEventApi.Common.Serialisation;
using Glasswall.Administration.K8.TransactionEventApi.Common.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;

namespace Glasswall.Administration.K8.TransactionEventApi
{
    [ExcludeFromCodeCoverage]
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddLogging();
            services.AddControllers();
            services.AddCors(options =>
            {
                options.AddPolicy("*",
                    builder =>
                    {
                        builder
                            .AllowAnyHeader()
                            .AllowAnyMethod()
                            .AllowAnyOrigin();
                    });
            });

            services.TryAddTransient<IConfigurationParser, EnvironmentVariableParser>();
            services.TryAddTransient<IDictionary<string, IConfigurationItemValidator>>(_ => new Dictionary<string, IConfigurationItemValidator>
            {
                {nameof(ITransactionEventApiConfiguration.TransactionStoreConnectionStringCsv), new StringValidator(1)},
                {nameof(ITransactionEventApiConfiguration.ShareName), new StringValidator(1, 100)}
            });
            services.TryAddSingleton<ITransactionEventApiConfiguration>(serviceProvider =>
            {
                var configuration = serviceProvider.GetRequiredService<IConfigurationParser>();
                return configuration.Parse<TransactionEventApiConfiguration>();
            });

            services.TryAddTransient<ITransactionService, TransactionService>();
            services.TryAddSingleton<ISerialiser, JsonSerialiser>();
            services.TryAddTransient<IXmlSerialiser, XmlSerialiser>();
            services.TryAddTransient<IJsonSerialiser, JsonSerialiser>();

            services.TryAddTransient<IEnumerable<ShareClient>>(s =>
            {
                var configuration = s.GetRequiredService<ITransactionEventApiConfiguration>();
                return configuration.TransactionStoreConnectionStringCsv.Split(',').Select(
                    connectionString => new ShareServiceClient(connectionString).GetShareClient(configuration.ShareName)
                ).ToArray();
            });

            services.TryAddTransient<IEnumerable<IFileShare>>(s =>
            {
                var clients = s.GetRequiredService<IEnumerable<ShareClient>>();
                return clients.Select(client => new AzureFileShare(client)).ToArray();
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
                app.UseDeveloperExceptionPage();

            app.UseRouting();
            app.UseAuthorization();

            app.Use((context, next) =>
            {
                context.Response.Headers["Access-Control-Expose-Headers"] = "*";
                context.Response.Headers["Access-Control-Allow-Headers"] = "*";
                context.Response.Headers["Access-Control-Allow-Origin"] = "*";

                if (context.Request.Method != "OPTIONS") return next.Invoke();
                
                context.Response.StatusCode = 200;
                return context.Response.WriteAsync("OK");
            });

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            app.UseCors("*");
        }
    }
}
