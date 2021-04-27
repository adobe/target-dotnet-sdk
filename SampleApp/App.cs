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

            // Initialize the SDK

            var targetClientConfig = new TargetClientConfig.Builder("adobetargetmobile", "B8A054D958807F770A495DD6@AdobeOrg")
                .SetLogger(this.logger)
                .SetDecisioningMethod(DecisioningMethod.OnDevice)
                .SetOnDeviceDecisioningReady(this.DecisioningReady)
                .SetArtifactDownloadSucceeded(artifact => Console.WriteLine("ArtifactDownloadSucceeded: " + artifact))
                .SetArtifactDownloadFailed(exception => Console.WriteLine("ArtifactDownloadFailed " + exception.Message))
                .Build();

            this.targetClient.Initialize(targetClientConfig);

            // sample server-side GetOffers call

            var deliveryRequest = new TargetDeliveryRequest.Builder()
                .SetDecisioningMethod(DecisioningMethod.ServerSide)
                .SetThirdPartyId("testThirdPartyId")
                .SetContext(new Context(ChannelType.Web))
                .SetExecute(new ExecuteRequest(null, new List<MboxRequest>
                {
                    new MboxRequest(index:1, name: "a1-serverside-ab")
                }))
                .Build();

            var response = await this.targetClient.GetOffersAsync(deliveryRequest);

            App.PrintCookies(this.logger, response);

            // sample SendNotifications call

            var notificationRequest = new TargetDeliveryRequest.Builder()
                .SetDecisioningMethod(DecisioningMethod.ServerSide)
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

            await Task.CompletedTask;
        }

        internal static void PrintCookies(ILogger logger, TargetDeliveryResponse response)
        {
            logger.LogInformation("Mbox cookie: " + response.GetCookies()[TargetConstants.MboxCookieName].Value);
            logger.LogInformation("Cluster cookie: " + response.GetCookies()[TargetConstants.ClusterCookieName].Value);
        }

        private void DecisioningReady()
        {
            Console.WriteLine("OnDeviceDecisioningReady");
            _ = this.GetOnDeviceOffersAsync();
        }

        private async Task GetOnDeviceOffersAsync()
        {
            // sample on-device GetOffers call

            var deliveryRequest = new TargetDeliveryRequest.Builder()
                .SetContext(new Context(ChannelType.Web, geo: new Geo("193.105.140.131")))
                .SetExecute(new ExecuteRequest(new RequestDetails(), new List<MboxRequest>
                {
                    new(index:1, name: "a1-mobile-tstsree")
                }))
                .Build();

            var response = await targetClient.GetOffersAsync(deliveryRequest);
            App.PrintCookies(this.logger, response);
        }
    }
}
