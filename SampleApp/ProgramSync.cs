namespace SampleApp
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using Adobe.Target.Client;
    using Adobe.Target.Client.Model;
    using Adobe.Target.Client.Model.OnDevice;
    using Adobe.Target.Delivery.Model;
    using Microsoft.Extensions.Logging;

    internal class ProgramSync
    {
        private static TargetClient targetClient;
        public static void Main(string[] args)
        {
            Console.WriteLine("Sync app");

            using var loggerFactory = LoggerFactory.Create(builder =>
            {
                builder.AddSimpleConsole(options => options.TimestampFormat = "hh:mm:ss ");
                builder.SetMinimumLevel(LogLevel.Debug);
            });
            var logger = loggerFactory.CreateLogger<ProgramSync>();

            var targetClientConfig = new TargetClientConfig.Builder("adobetargetmobile", "B8A054D958807F770A495DD6@AdobeOrg")
                .SetLogger(logger)
                .SetDecisioningMethod(DecisioningMethod.OnDevice)
                .SetOnDeviceDecisioningHandler(new OnDeviceDecisioningHandler())
                .Build();
            targetClient = TargetClient.Create(targetClientConfig);

            Thread.Sleep(3000);

            /*var deliveryRequest = new TargetDeliveryRequest.Builder()
                .SetThirdPartyId("testThirdPartyId")
                .SetContext(new Context(ChannelType.Web, geo: new Geo("193.105.140.131")))
                .SetExecute(new ExecuteRequest(null, new List<MboxRequest>
                {
                    new MboxRequest(index:1, name: "a1-serverside-ab")
                }))
                .Build();

            var response = targetClient.GetOffers(deliveryRequest);

            App.PrintCookies(logger, response);

            var notificationRequest = new TargetDeliveryRequest.Builder()
                .SetSessionId(response.Request.SessionId)
                .SetTntId(response.Response?.Id?.TntId)
                .SetThirdPartyId("testThirdPartyId")
                .SetContext(new Context(ChannelType.Web))
                .SetNotifications(new List<Notification>()
                {
                    { new(id:"notificationId1", type: MetricType.Display, timestamp: DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
                        tokens: new List<string>())}
                })
                .Build();

            App.PrintCookies(logger, targetClient.SendNotifications(notificationRequest));*/
        }

        private class OnDeviceDecisioningHandler : IOnDeviceDecisioningHandler
        {
            public void OnDeviceDecisioningReady()
            {
                Console.WriteLine("OnDeviceDecisioningReady");

                var deliveryRequest = new TargetDeliveryRequest.Builder()
                    .SetThirdPartyId("testThirdPartyId")
                    .SetContext(new Context(ChannelType.Web, geo: new Geo("193.105.140.131")))
                    .SetExecute(new ExecuteRequest(null, new List<MboxRequest>
                    {
                        new MboxRequest(index:1, name: "a1-serverside-ab")
                    }))
                    .Build();

                var response = targetClient.GetOffers(deliveryRequest);
            }

            public void ArtifactDownloadSucceeded(string artifactData) => Console.WriteLine("ArtifactDownloadSucceeded: " + artifactData);

            public void ArtifactDownloadFailed(ApplicationException e) => Console.WriteLine("ArtifactDownloadFailed " + e.Message);
        }
    }
}
