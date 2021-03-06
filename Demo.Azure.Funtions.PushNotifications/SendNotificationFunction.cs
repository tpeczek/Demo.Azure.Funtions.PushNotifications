using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.Documents.Linq;
using Microsoft.Azure.Documents.Client;
using Lib.Net.Http.WebPush;
using Lib.Azure.WebJobs.Extensions.WebPush.Bindings;

namespace Demo.Azure.Funtions.PushNotifications
{
    public static class SendNotificationFunction
    {
        private static readonly Uri _subscriptionsCollectionUri = UriFactory.CreateDocumentCollectionUri("PushNotifications", "SubscriptionsCollection");

        [FunctionName("SendNotification")]
        public static async Task Run([CosmosDBTrigger(
            databaseName: "PushNotifications",
            collectionName: "NotificationsCollection",
            ConnectionStringSetting = "CosmosDBConnection",
            LeaseCollectionName = "NotificationsLeaseCollection",
            CreateLeaseCollectionIfNotExists = true)]IReadOnlyList<PushMessage> notifications,
            [CosmosDB(
            databaseName: "PushNotifications",
            collectionName: "SubscriptionsCollection",
            ConnectionStringSetting = "CosmosDBConnection")]DocumentClient cosmosDbClient,
            [PushService(
            PublicKeySetting = "ApplicationServerPublicKey",
            PrivateKeySetting = "ApplicationServerPrivateKey",
            SubjectSetting = "ApplicationServerSubject")]PushServiceClient pushServiceClient)
        {
            if (notifications != null)
            {
                IDocumentQuery<PushSubscription> subscriptionQuery = cosmosDbClient.CreateDocumentQuery<PushSubscription>(_subscriptionsCollectionUri, new FeedOptions
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
                            pushServiceClient.RequestPushMessageDeliveryAsync(subscription, notification);
                        }
                    }
                }
            }
        }
    }
}
