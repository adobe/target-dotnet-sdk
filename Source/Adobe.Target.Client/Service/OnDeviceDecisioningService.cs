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
#pragma warning disable VSTHRD002
namespace Adobe.Target.Client.Service
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Threading.Tasks;
    using Adobe.Target.Client.Extension;
    using Adobe.Target.Client.Model;
    using Adobe.Target.Client.Model.OnDevice;
    using Adobe.Target.Client.OnDevice;
    using Adobe.Target.Client.OnDevice.Collator;
    using Adobe.Target.Delivery.Model;
    using Microsoft.Extensions.Logging;

    /// <summary>
    /// OnDevice Decisioning Service
    /// </summary>
    internal sealed class OnDeviceDecisioningService
    {
        private const string ContextKeyUser = "user";
        private const string ContextKeyGeo = "geo";
        private const string ContextKeyPage = "page";
        private const string ContextKeyReferring = "referring";
        private const string ContextKeyCustom = "mbox";

        private static readonly IReadOnlyDictionary<string, IParamsCollator> RequestParamsCollators =
            new Dictionary<string, IParamsCollator>()
            {
                { ContextKeyUser, new UserParamsCollator() },
                { ContextKeyGeo, new GeoParamsCollator() },
            };

        private static readonly IReadOnlyDictionary<string, IParamsCollator> DetailsParamsCollators =
            new Dictionary<string, IParamsCollator>()
            {
                { ContextKeyPage, new PageParamsCollator() },
                { ContextKeyReferring, new PageParamsCollator(true) },
                { ContextKeyCustom, new CustomParamsCollator() },
            };

        private static readonly TimeParamsCollator TimeParamsCollator = new TimeParamsCollator();

        private readonly TargetClientConfig clientConfig;
        private readonly TargetService targetService;
        private readonly RuleLoader ruleLoader;
        private readonly GeoClient geoClient;

        /// <summary>
        /// Initializes a new instance of the <see cref="OnDeviceDecisioningService"/> class.
        /// </summary>
        /// <param name="clientConfig">Client config</param>
        /// <param name="targetService">Target Service</param>
        internal OnDeviceDecisioningService(TargetClientConfig clientConfig, TargetService targetService)
        {
            this.clientConfig = clientConfig;
            this.targetService = targetService;
            this.ruleLoader = new RuleLoader(clientConfig);
            this.geoClient = new GeoClient(clientConfig);
        }

        internal TargetDeliveryResponse ExecuteRequest(TargetDeliveryRequest deliveryRequest)
        {
            var ruleSet = this.ruleLoader.GetLatestRules();
            if (ruleSet == null)
            {
                var deliveryResponse = new DeliveryResponse(
                    (int)HttpStatusCode.ServiceUnavailable,
                    deliveryRequest.DeliveryRequest.RequestId,
                    deliveryRequest.DeliveryRequest.Id,
                    this.clientConfig.Client);
                return new TargetDeliveryResponse(
                    deliveryRequest,
                    deliveryResponse,
                    HttpStatusCode.ServiceUnavailable,
                    "Local-decisioning rules not available");
            }

            var requestContext = new Dictionary<string, object>(TimeParamsCollator.CollateParams());
            this.GeoLookup(deliveryRequest.DeliveryRequest, ruleSet.GeoTargetingEnabled);
            requestContext.AddAll(this.CollateParams(RequestParamsCollators, deliveryRequest));

            var responseTokens = new HashSet<string>(ruleSet.ResponseTokens);
            var targetResponse = this.BuildDeliveryResponse(deliveryRequest);
            var visitorId = this.GetOrCreateVisitorId(deliveryRequest, targetResponse);
            var prefetchDetails = this.GetPrefetchDetails(deliveryRequest.DeliveryRequest);

            this.HandleDetails(
                prefetchDetails,
                requestContext,
                deliveryRequest,
                visitorId,
                responseTokens,
                ruleSet,
                targetResponse.Response.Prefetch);

            var executeDetails = this.GetExecuteDetails(deliveryRequest.DeliveryRequest);
            IList<Notification> notifications = new List<Notification>();

            this.HandleDetails(
                executeDetails,
                requestContext,
                deliveryRequest,
                visitorId,
                responseTokens,
                ruleSet,
                null,
                targetResponse.Response.Execute,
                notifications);

            // TODO: Send notifications
            TargetClient.Logger?.LogDebug(targetResponse.ToString());
            return targetResponse;
        }

        private void HandleDetails(
            IList<RequestDetailsUnion> detailsList,
            Dictionary<string, object> requestContext,
            TargetDeliveryRequest deliveryRequest,
            string visitorId,
            ISet<string> responseTokens,
            OnDeviceDecisioningRuleSet ruleSet,
            PrefetchResponse prefetchResponse,
            ExecuteResponse executeResponse = default,
            IList<Notification> notifications = default)
        {
            foreach (var details in detailsList)
            {
                var detailsContext =
                    this.CollateParams(DetailsParamsCollators, deliveryRequest, details.GetRequestDetails());

                // TODO: call decision handler
            }
        }

        private string GetOrCreateVisitorId(TargetDeliveryRequest deliveryRequest, TargetDeliveryResponse targetResponse)
        {
            string vid = null;
            var visitorId = deliveryRequest.DeliveryRequest.Id;
            if (visitorId != null)
            {
                vid = new string[] { visitorId.MarketingCloudVisitorId, visitorId.TntId, visitorId.ThirdPartyId }
                    .FirstOrDefault(s => !string.IsNullOrEmpty(s));
            }

            vid ??= targetResponse.Response.Id?.TntId;

            if (vid != null)
            {
                return vid;
            }

            vid = this.GenerateTntId();
            if (visitorId == null)
            {
                visitorId = new VisitorId();
                visitorId.TntId = vid;
            }
            else
            {
                visitorId.TntId = vid;
            }

            targetResponse.Response.Id = visitorId;

            return vid;
        }

        private string GenerateTntId()
        {
            // TODO: Implement
            return null;
        }

        private TargetDeliveryResponse BuildDeliveryResponse(TargetDeliveryRequest deliveryRequest)
        {
            // TODO: Implement
            return new TargetDeliveryResponse(deliveryRequest, new DeliveryResponse(), HttpStatusCode.Accepted);
        }

        private IList<RequestDetailsUnion> GetExecuteDetails(DeliveryRequest deliveryRequest)
        {
            var result = new List<RequestDetailsUnion>();
            var execute = deliveryRequest.Execute;
            if (execute == null)
            {
                return result;
            }

            if (execute.Mboxes?.Count != 0)
            {
                result.AddRange(execute.Mboxes!.Select(mbox => new RequestDetailsUnion(mbox)));
            }

            if (execute.PageLoad != null)
            {
                result.Add(new RequestDetailsUnion(execute.PageLoad));
            }

            return result;
        }

        private IList<RequestDetailsUnion> GetPrefetchDetails(DeliveryRequest deliveryRequest)
        {
            var result = new List<RequestDetailsUnion>();
            var prefetch = deliveryRequest.Prefetch;
            if (prefetch == null)
            {
                return result;
            }

            if (prefetch.Mboxes?.Count != 0)
            {
                result.AddRange(prefetch.Mboxes!.Select(mbox => new RequestDetailsUnion(mbox)));
            }

            if (prefetch.Views?.Count != 0)
            {
                result.AddRange(prefetch.Mboxes!.Select(mbox => new RequestDetailsUnion(mbox)));
            }

            if (prefetch.PageLoad != null)
            {
                result.Add(new RequestDetailsUnion(prefetch.PageLoad));
            }

            return result;
        }

        private IDictionary<string, object> CollateParams(
            IReadOnlyDictionary<string, IParamsCollator> paramsCollators,
            TargetDeliveryRequest deliveryRequest,
            RequestDetails requestDetails = default)
        {
            return paramsCollators.ToDictionary<KeyValuePair<string, IParamsCollator>, string, object>(
                paramsCollator => paramsCollator.Key,
                paramsCollator => paramsCollator.Value.CollateParams(deliveryRequest, requestDetails));
        }

        private void GeoLookup(DeliveryRequest deliveryRequest, bool geoTargetingEnabled)
        {
            if (!geoTargetingEnabled)
            {
                return;
            }

            var geo = deliveryRequest.Context.Geo;
            deliveryRequest.Context.Geo = this.geoClient.LookupGeoAsync(geo).Result;
        }

        private async Task GeoLookupAsync(DeliveryRequest deliveryRequest, bool geoTargetingEnabled)
        {
            if (!geoTargetingEnabled)
            {
                return;
            }

            var geo = deliveryRequest.Context.Geo;
            deliveryRequest.Context.Geo = await this.geoClient.LookupGeoAsync(geo);
        }
    }
}
#pragma warning restore VSTHRD002
