using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

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
        public void Run([CosmosDBTrigger(
            databaseName: "PushNotifications",
            containerName: "Notifications",
            Connection = "CosmosDBConnection",
            LeaseContainerName = "NotificationsLeaseCollection",
            CreateLeaseContainerIfNotExists = true)] IReadOnlyList<Notification> notifications,
            [CosmosDBInput(
            databaseName: "PushNotifications",
            containerName: "Subscriptions",
            Connection = "CosmosDBConnection")] CosmosClient cosmosClient)
        {
            if (notifications != null)
            {
                Container subscriptionsContainer = cosmosClient.GetDatabase("PushNotifications").GetContainer("Subscriptions");
            }
        }
    }

    public class Notification
    {
        public string? Topic { get; set; }

        public string Content { get; set; } = String.Empty;

        public int? TimeToLive { get; set; }

        //public PushMessageUrgency Urgency { get; set; }
    }
}
