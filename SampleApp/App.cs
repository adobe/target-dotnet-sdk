/*
 * Copyright 2020 Adobe. All rights reserved.
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
            this.logger.LogInformation("Starting ...");

            var targetClientConfig = new TargetClientConfig.Builder("adobetargetmobile", "B8A054D958807F770A495DD6@AdobeOrg")
                .Build();

            Console.WriteLine("Target init");
            this.targetClient.Initialize(targetClientConfig);

            var deliveryRequest = new TargetDeliveryRequest.Builder()
                .SetSessionId(Guid.NewGuid().ToString())
                .SetThirdPartyId("testThirdPartyId")
                .SetContext(new Context(ChannelType.Web))
                .SetExecute(new ExecuteRequest(null, new List<MboxRequest>
                {
                    new MboxRequest(index:1, name: "a1-serverside-ab")
                }))
                .Build();

            var response = await this.targetClient.GetOffersAsync(deliveryRequest);

            App.PrintResponse(response);

            this.logger.LogInformation("Done");

            await Task.CompletedTask;
        }

        internal static void PrintResponse(TargetDeliveryResponse response)
        {
            Console.WriteLine(response.Response.ToJson());
            Console.WriteLine("Mbox cookie: " + response.GetCookies()[TargetConstants.MboxCookieName].Value);
            Console.WriteLine("Cluster cookie: " + response.GetCookies()[TargetConstants.ClusterCookieName].Value);
        }
    }
}
