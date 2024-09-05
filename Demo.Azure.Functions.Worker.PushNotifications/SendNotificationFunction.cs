using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Lib.Net.Http.WebPush;
using Lib.Azure.Functions.Worker.Extensions.WebPush;

namespace Demo.Azure.Functions.Worker.PushNotifications
{
    public class SendNotificationFunction
    {
        private readonly ILogger _logger;

        public SendNotificationFunction(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<SendNotificationFunction>();
        }

        [Function("SendNotificationFunction")]
        public async Task Run([CosmosDBTrigger(
            databaseName: "PushNotifications",
            containerName: "Notifications",
            Connection = "CosmosDBConnection",
            LeaseContainerName = "NotificationsLeaseCollection",
            CreateLeaseContainerIfNotExists = true)] IReadOnlyList<Notification> notifications,
            [CosmosDBInput(
            databaseName: "PushNotifications",
            containerName: "Subscriptions",
            Connection = "CosmosDBConnection")] CosmosClient cosmosClient,
            [PushServiceInput(
            PublicKeySetting = "ApplicationServerPublicKey",
            PrivateKeySetting = "ApplicationServerPrivateKey",
            SubjectSetting = "ApplicationServerSubject")] PushServiceClient pushServiceClient)
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

    public class Notification
    {
        public string? Topic { get; set; }

        public string Content { get; set; } = String.Empty;

        public int? TimeToLive { get; set; }

        public PushMessageUrgency Urgency { get; set; }
    }
}
