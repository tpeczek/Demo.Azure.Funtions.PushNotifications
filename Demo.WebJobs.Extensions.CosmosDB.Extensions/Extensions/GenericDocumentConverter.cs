using System;
using System.Collections.Generic;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.Documents;
using Newtonsoft.Json;

namespace Demo.WebJobs.Extensions.CosmosDB.Extensions.Extensions
{
    internal class GenericDocumentConverter<T> : IConverter<IReadOnlyList<Document>, IReadOnlyList<T>>
    {
        public IReadOnlyList<T> Convert(IReadOnlyList<Document> input)
        {
            List<T> output = new List<T>(input.Count);

            foreach(Document item in input)
            {
                output.Add(Convert(item));
            }

            return output.AsReadOnly();
        }

        private static T Convert(Document document)
        {
            return JsonConvert.DeserializeObject<T>(document.ToString());
        }
    }
}
