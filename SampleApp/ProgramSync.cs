namespace SampleApp
{
    using System;
    using System.Collections.Generic;
    using Adobe.Target.Client;
    using Adobe.Target.Client.Model;
    using Adobe.Target.Delivery.Model;

    internal class ProgramSync
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("Sync app");

            var targetClientConfig = new TargetClientConfig.Builder("adobetargetmobile", "B8A054D958807F770A495DD6@AdobeOrg")
                .Build();
            var targetClient = TargetClient.Create(targetClientConfig);

            var deliveryRequest = new TargetDeliveryRequest.Builder()
                .SetSessionId(Guid.NewGuid().ToString())
                .SetThirdPartyId("testThirdPartyId")
                .SetContext(new Context(ChannelType.Web))
                .SetExecute(new ExecuteRequest(null, new List<MboxRequest>
                {
                    new MboxRequest(index:1, name: "a1-serverside-ab")
                }))
                .Build();

            var response = targetClient.GetOffers(deliveryRequest);

            App.PrintResponse(response);
        }
    }
}
