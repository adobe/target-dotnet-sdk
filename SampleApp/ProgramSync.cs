namespace SampleApp
{
    using System;
    using System.Collections.Generic;
    using Adobe.Target.Client;
    using Adobe.Target.Client.Model;
    using Adobe.Target.Delivery.Model;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;

    internal class ProgramSync
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("Sync app");

            var targetClientConfig = new TargetClientConfig.Builder("adobetargetmobile", "B8A054D958807F770A495DD6@AdobeOrg")
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

            App.PrintResponse(response);

            var cont = response.Response.Execute.Mboxes[0].Options[0].Content;
            // var dict = (Dictionary<string, Object>) cont;
            var dict = ((JObject)cont).ToObject<Dictionary<string, Object>>();
            Console.WriteLine("ZZ: " + dict["title"] + " X " + dict["title"].GetType() + " Y " + (dict["title"] is string));

            var dd = JsonConvert.DeserializeObject<Object>("{\"a\":1, \"b\":1.2, \"c\":false}");
            var dict2 = ((JObject)dd).ToObject<Dictionary<string, Object>>();
            Console.WriteLine("TT:" + dd.GetType() + " T " + dict2.GetType());
            Console.WriteLine("ZZ: " + dict2["a"] + " X " + dict2["a"].GetType() + " Y " + (dict2["a"] is int));
            Console.WriteLine("ZZ: " + dict2["b"] + " X " + dict2["b"].GetType() + " Y " + (dict2["b"] is double));
            Console.WriteLine("ZZ: " + dict2["c"] + " X " + dict2["c"].GetType() + " Y " + (dict2["c"] is bool));

            var z = TestConvert(dict2["a"]);
            Console.WriteLine("VV: " + TestConvert(dict2["a"]) + TestConvert(dict2["b"]) + TestConvert(dict2["c"]));

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

            App.PrintResponse(targetClient.SendNotifications(notificationRequest));
        }

        private static int TestConvert(object value)
        {
            try
            {
                return (int)Convert.ChangeType(value, TypeCode.Int32);
            }
            catch (Exception)
            {
                return 0;
            }
        }
    }
}
