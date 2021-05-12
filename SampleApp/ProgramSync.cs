namespace SampleApp
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using Adobe.Target.Client;
    using Adobe.Target.Client.Model;
    using Adobe.Target.Delivery.Model;
    using Microsoft.Extensions.Logging;

    internal class ProgramSync
    {
        private static TargetClient targetClient;
        private static ILogger<ProgramSync> logger;
        public static void Main(string[] args)
        {
            Console.WriteLine("Sync app");

            using var loggerFactory = LoggerFactory.Create(builder =>
            {
                builder.AddSimpleConsole(options => options.TimestampFormat = "hh:mm:ss ");
                builder.SetMinimumLevel(LogLevel.Debug);
            });
            logger = loggerFactory.CreateLogger<ProgramSync>();

            // Initialize the SDK

            var targetClientConfig = new TargetClientConfig.Builder("adobetargetmobile", "B8A054D958807F770A495DD6@AdobeOrg")
                .SetLogger(logger)
                .SetDecisioningMethod(DecisioningMethod.OnDevice)
                .SetOnDeviceDecisioningReady(DecisioningReady)
                .SetArtifactDownloadSucceeded(artifact => Console.WriteLine("ArtifactDownloadSucceeded: " + artifact))
                .SetArtifactDownloadFailed(exception => Console.WriteLine("ArtifactDownloadFailed " + exception.Message))
                .Build();
            targetClient = TargetClient.Create(targetClientConfig);

            // sample server-side GetOffers call

            var deliveryRequest = new TargetDeliveryRequest.Builder()
                .SetDecisioningMethod(DecisioningMethod.ServerSide)
                .SetThirdPartyId("testThirdPartyId")
                .SetContext(new Context(ChannelType.Web))
                .SetExecute(new ExecuteRequest(null, new List<MboxRequest>
                {
                    new (index: 0, name: "a1-serverside-ab")
                }))
                .Build();

            var response = targetClient.GetOffers(deliveryRequest);

            App.PrintCookies(logger, response);

            // sample SendNotifications call

            var notificationRequest = new TargetDeliveryRequest.Builder()
                .SetDecisioningMethod(DecisioningMethod.ServerSide)
                .SetSessionId(response.Request.SessionId)
                .SetTntId(response.Response?.Id?.TntId)
                .SetThirdPartyId("testThirdPartyId")
                .SetContext(new Context(ChannelType.Web))
                .SetNotifications(new List<Notification>()
                {
                    new (id:"notificationId1", type: MetricType.Display, timestamp: DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
                        tokens: new List<string>())
                })
                .Build();

            var notificationResponse = targetClient.SendNotifications(notificationRequest);
            App.PrintCookies(logger, notificationResponse);

            Thread.Sleep(3000);
        }

        private static void DecisioningReady()
        {
            Console.WriteLine("OnDeviceDecisioningReady");

            // sample on-device GetOffers call

            var deliveryRequest = new TargetDeliveryRequest.Builder()
                .SetContext(new Context(ChannelType.Web, geo: new Geo("193.105.140.131")))
                .SetExecute(new ExecuteRequest(new RequestDetails(), new List<MboxRequest>
                {
                    new (index: 1, name: "a1-mobile-tstsree")
                }))
                .Build();

            var response = targetClient.GetOffers(deliveryRequest);
            App.PrintCookies(logger, response);
        }
    }
}
