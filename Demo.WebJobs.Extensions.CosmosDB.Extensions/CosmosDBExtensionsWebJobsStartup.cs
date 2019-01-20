using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Hosting;
using Demo.WebJobs.Extensions.CosmosDB.Extensions;
using Demo.WebJobs.Extensions.CosmosDB.Extensions.Config;

[assembly: WebJobsStartup(typeof(CosmosDBExtensionsWebJobsStartup))]

namespace Demo.WebJobs.Extensions.CosmosDB.Extensions
{
    public class CosmosDBExtensionsWebJobsStartup : IWebJobsStartup
    {
        public void Configure(IWebJobsBuilder builder)
        {
            builder.AddCosmosDBExtensions();
        }
    }
}