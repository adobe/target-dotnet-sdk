namespace SampleApp
{
    using System;
    using System.Collections.Generic;
    using Adobe.Target.Client;
    using Adobe.Target.Client.Model;
    using Adobe.Target.Delivery.Model;
    using Microsoft.Extensions.Logging;

    internal class ProgramSync
    {
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
                .Build();
            var targetClient = TargetClient.Create(targetClientConfig);

            var deliveryRequest = new TargetDeliveryRequest.Builder()
                .SetThirdPartyId("testThirdPartyId")
                .SetContext(new Context(ChannelType.Web))
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

            App.PrintCookies(logger, targetClient.SendNotifications(notificationRequest));
        }
    }
}
