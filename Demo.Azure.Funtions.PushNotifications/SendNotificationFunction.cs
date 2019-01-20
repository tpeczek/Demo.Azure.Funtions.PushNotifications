using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.Documents.Linq;
using Microsoft.Azure.Documents.Client;
using Lib.Net.Http.WebPush;
using Lib.Net.Http.WebPush.Authentication;

namespace Demo.Azure.Funtions.PushNotifications
{
    public static class SendNotificationFunction
    {
        private static readonly PushServiceClient _pushClient = new PushServiceClient
        {
            DefaultAuthentication = new VapidAuthentication("<Application Server Public Key>", "<Application Server Private Key>")
            {
                Subject = "https://localhost:65506/"
            }
        };
        private static readonly Uri _subscriptionsCollectionUri = UriFactory.CreateDocumentCollectionUri("PushNotifications", "SubscriptionsCollection");

        [FunctionName("SendNotification")]
        public static async Task Run(
            [CosmosDBTrigger("PushNotifications", "NotificationsCollection", LeaseCollectionName = "NotificationsLeaseCollection", CreateLeaseCollectionIfNotExists = true, ConnectionStringSetting = "CosmosDBConnection")]
            IReadOnlyList<PushMessage> notifications,
            [CosmosDB("PushNotifications", "SubscriptionsCollection", ConnectionStringSetting = "CosmosDBConnection")]
            DocumentClient client)
        {
            if (notifications != null)
            {
                IDocumentQuery<PushSubscription> subscriptionQuery = client.CreateDocumentQuery<PushSubscription>(_subscriptionsCollectionUri, new FeedOptions
                {
                    EnableCrossPartitionQuery = true,
                    MaxItemCount = -1
                }).AsDocumentQuery();

                while (subscriptionQuery.HasMoreResults)
                {
                    foreach (PushSubscription subscription in await subscriptionQuery.ExecuteNextAsync())
                    {
                        foreach (PushMessage notification in notifications)
                        {
                            // Fire-and-forget
                            _pushClient.RequestPushMessageDeliveryAsync(subscription, notification);
                        }
                    }
                }
            }
        }
    }
}
