using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Glasswall.Administration.K8.TransactionEventApi.Controllers;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.EnvironmentVariables;
using Microsoft.Extensions.DependencyInjection;

[assembly: FunctionsStartup(typeof(TransactionEventApi.Function.Startup))]

namespace TransactionEventApi.Function
{
    public class Startup : FunctionsStartup
    {
        [ExcludeFromCodeCoverage]
        public override void Configure(IFunctionsHostBuilder builder)
        {
            var mainStartup = new Glasswall.Administration.K8.TransactionEventApi.Startup(new ConfigurationRoot(new List<IConfigurationProvider>() { new EnvironmentVariablesConfigurationProvider()}));

            mainStartup.ConfigureServices(builder.Services);
            builder.Services.AddTransient<TransactionController>();
        }
    }
}