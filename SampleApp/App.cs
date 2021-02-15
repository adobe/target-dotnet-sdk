/*
 * Copyright 2021 Adobe. All rights reserved.
 * This file is licensed to you under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License. You may obtain a copy
 * of the License at http://www.apache.org/licenses/LICENSE-2.0
 * Unless required by applicable law or agreed to in writing, software distributed under
 * the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR REPRESENTATIONS
 * OF ANY KIND, either express or implied. See the License for the specific language
 * governing permissions and limitations under the License.
 */
namespace SampleApp
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Adobe.Target.Client;
    using Adobe.Target.Client.Model;
    using Adobe.Target.Client.Util;
    using Adobe.Target.Delivery.Model;
    using Microsoft.Extensions.Logging;

    public class App
    {
        private readonly ITargetClient targetClient;
        private readonly ILogger<App> logger;

        public App(ITargetClient targetClient, ILogger<App> logger)
        {
            this.targetClient = targetClient;
            this.logger = logger;
        }

        public async Task RunAsync(string[] args)
        {
            Console.WriteLine("Async app");
            this.logger.LogInformation("Starting ...");

            var targetClientConfig = new TargetClientConfig.Builder("adobetargetmobile", "B8A054D958807F770A495DD6@AdobeOrg")
                .SetLogger(this.logger)
                .Build();

            this.targetClient.Initialize(targetClientConfig);

            var deliveryRequest = new TargetDeliveryRequest.Builder()
                .SetThirdPartyId("testThirdPartyId")
                .SetContext(new Context(ChannelType.Web))
                .SetExecute(new ExecuteRequest(null, new List<MboxRequest>
                {
                    new MboxRequest(index:1, name: "a1-serverside-ab")
                }))
                .Build();

            var response = await this.targetClient.GetOffersAsync(deliveryRequest);

            App.PrintCookies(this.logger, response);

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

            App.PrintCookies(this.logger, await this.targetClient.SendNotificationsAsync(notificationRequest));

            this.logger.LogInformation("Done");

            await Task.CompletedTask;
        }

        internal static void PrintCookies(ILogger logger, TargetDeliveryResponse response)
        {
            logger.LogInformation("Mbox cookie: " + response.GetCookies()[TargetConstants.MboxCookieName].Value);
            logger.LogInformation("Cluster cookie: " + response.GetCookies()[TargetConstants.ClusterCookieName].Value);
        }
    }
}
