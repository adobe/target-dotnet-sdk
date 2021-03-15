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
#nullable enable
namespace Adobe.Target.Client.OnDevice
{
    using System.Net;
    using System.Threading;
    using System.Threading.Tasks;
    using Adobe.Target.Client.Model;
    using Adobe.Target.Client.Service;
    using Adobe.Target.Client.Util;
    using Adobe.Target.Delivery.Model;

    internal sealed class ClusterLocator
    {
        private volatile string? locationHint;

        internal ClusterLocator(TargetClientConfig clientConfig, TargetService targetService)
        {
            if (clientConfig.DecisioningMethod == DecisioningMethod.ServerSide)
            {
                return;
            }

            this.FetchLocation(targetService);
        }

        internal string? GetLocationHint()
        {
            return this.locationHint;
        }

        private void FetchLocation(TargetService targetService)
        {
            _ = this.FetchLocationAsync(targetService);
        }

        private async Task FetchLocationAsync(TargetService targetService)
        {
            var request = new TargetDeliveryRequest.Builder()
                .SetContext(new Context(ChannelType.Web))
                .Build();

            var response = await targetService.ExecuteRequestAsync(request);

            var tntId = response.Response?.Id?.TntId;
            if (response.Status < HttpStatusCode.OK || response.Status >= HttpStatusCode.Ambiguous
                                                    || tntId == null)
            {
                return;
            }

            Interlocked.Exchange(ref this.locationHint, CookieUtils.LocationHintFromTntId(tntId));
        }
    }
}
