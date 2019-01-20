using System;
using Microsoft.Azure.WebJobs;

namespace Demo.WebJobs.Extensions.CosmosDB.Extensions.Config
{
    internal static class CosmosDBExtensionsWebJobsBuilderExtensions
    {
        public static IWebJobsBuilder AddCosmosDBExtensions(this IWebJobsBuilder builder)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            builder.AddExtension<CosmosDBExtensionExtensionsConfigProvider>();

            return builder;
        }
    }
}
