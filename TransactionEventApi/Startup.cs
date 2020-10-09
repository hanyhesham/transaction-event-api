using System;
using System.Collections.Generic;
using System.Linq;
using Glasswall.Administration.K8.TransactionEventApi.Business.Configuration;
using Glasswall.Administration.K8.TransactionEventApi.Business.Serialisation;
using Glasswall.Administration.K8.TransactionEventApi.Business.Services;
using Glasswall.Administration.K8.TransactionEventApi.Common.Configuration;
using Glasswall.Administration.K8.TransactionEventApi.Common.Configuration.Validation;
using Glasswall.Administration.K8.TransactionEventApi.Common.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Glasswall.Administration.K8.TransactionEventApi
{
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

            services.TryAddTransient<ITransactionService, TransactionService>();
            
            services.TryAddTransient<IDictionary<string, IConfigurationItemValidator>>(_ => new Dictionary<string, IConfigurationItemValidator>
            {
                {nameof(ITransactionEventApiConfiguration.TransactionStoreConnectionStringCsv), new StringValidator(1)},
            });

            services.TryAddTransient<IConfigurationParser, EnvironmentVariableParser>();

            services.TryAddSingleton<ITransactionEventApiConfiguration>(serviceProvider =>
            {
                var configuration = serviceProvider.GetRequiredService<IConfigurationParser>();
                return configuration.Parse<TransactionEventApiConfiguration>();
            });

            services.TryAddSingleton<ISerialiser, JsonSerialiser>();

            services.TryAddTransient<IEnumerable<IFileStore>>(s =>
            {
                var configuration = s.GetRequiredService<ITransactionEventApiConfiguration>();
                var logger = s.GetRequiredService<ILogger<IFileStore>>();
                var serialiser = s.GetRequiredService<ISerialiser>();
                return configuration.TransactionStoreConnectionStringCsv.Split(',').Select(
                    connectionString => new FileStore(logger, serialiser, connectionString)
                ).ToArray();
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