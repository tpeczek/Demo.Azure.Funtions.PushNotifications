using System;
using System.Collections.Generic;
using Microsoft.Azure.Documents;
using Microsoft.Azure.WebJobs.Description;
using Microsoft.Azure.WebJobs.Host.Bindings;
using Microsoft.Azure.WebJobs.Host.Config;
using Demo.WebJobs.Extensions.CosmosDB.Extensions.Extensions;

namespace Demo.WebJobs.Extensions.CosmosDB.Extensions.Config
{
    [Extension("CosmosDBExtensions")]
    internal class CosmosDBExtensionExtensionsConfigProvider : IExtensionConfigProvider
    {
        public void Initialize(ExtensionConfigContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }

            context.AddOpenConverter<IReadOnlyList<Document>, IReadOnlyList<OpenType>>(typeof(GenericDocumentConverter<>));
        }
    }
}
