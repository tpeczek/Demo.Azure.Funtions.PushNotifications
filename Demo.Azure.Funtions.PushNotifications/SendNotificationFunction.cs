using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.WebJobs;
using Lib.Net.Http.WebPush;
using Lib.Azure.WebJobs.Extensions.WebPush.Bindings;


namespace Demo.Azure.Funtions.PushNotifications
{
    public class Notification
    {
        public string Topic { get; set; }

        public string Content {  get; set; }

        public int? TimeToLive { get; set; }

        public PushMessageUrgency Urgency { get; set; }
    }

    public static class SendNotificationFunction
    {
        [FunctionName("SendNotification")]
        public static async Task Run([CosmosDBTrigger(
            databaseName: "PushNotifications",
            containerName: "Notifications",
            Connection = "CosmosDBConnection",
            LeaseContainerName = "NotificationsLeaseCollection",
            CreateLeaseContainerIfNotExists = true)]IReadOnlyList<Notification> notifications,
            [CosmosDB(
            databaseName: "PushNotifications",
            containerName: "Subscriptions",
            Connection = "CosmosDBConnection")]CosmosClient cosmosClient,
            [PushService(
            PublicKeySetting = "ApplicationServerPublicKey",
            PrivateKeySetting = "ApplicationServerPrivateKey",
            SubjectSetting = "ApplicationServerSubject")]PushServiceClient pushServiceClient)
        {
            if (notifications != null)
            {
                Container subscriptionsContainer = cosmosClient.GetDatabase("PushNotifications").GetContainer("Subscriptions");
                using (FeedIterator<PushSubscription> subscriptionsIterator = subscriptionsContainer.GetItemQueryIterator<PushSubscription>())
                {
                    while (subscriptionsIterator.HasMoreResults)
                    {
                        foreach (PushSubscription subscription in await subscriptionsIterator.ReadNextAsync())
                        {
                            foreach (Notification notification in notifications)
                            {
                                // Fire-and-forget
                                pushServiceClient.RequestPushMessageDeliveryAsync(subscription, new PushMessage(notification.Content)
                                {
                                    Topic = notification.Topic,
                                    TimeToLive = notification.TimeToLive,
                                    Urgency = notification.Urgency
                                });
                            }
                        }
                    }
                }
            }
        }
    }
}
