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
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Net;
    using System.Threading.Tasks;
    using Adobe.Target.Client.Extension;
    using Adobe.Target.Client.Model;
    using Adobe.Target.Client.Model.OnDevice;
    using Adobe.Target.Client.OnDevice;
    using Adobe.Target.Client.OnDevice.Collator;
    using Adobe.Target.Client.Util;
    using Adobe.Target.Delivery.Model;
    using Microsoft.Extensions.Logging;

    /// <summary>
    /// OnDevice Decisioning Service
    /// </summary>
    internal sealed class OnDeviceDecisioningService : ITargetService
    {
        internal const string ContextKeyGeo = "geo";
        private const string ContextKeyUser = "user";
        private const string ContextKeyPage = "page";
        private const string ContextKeyReferring = "referring";
        private const string ContextKeyCustom = "mbox";

        private static readonly IReadOnlyDictionary<string, IParamsCollator> RequestParamsCollators =
            new Dictionary<string, IParamsCollator>
            {
                { ContextKeyUser, new UserParamsCollator() },
                { ContextKeyGeo, new GeoParamsCollator() },
            };

        private static readonly IReadOnlyDictionary<string, IParamsCollator> DetailsParamsCollators =
            new Dictionary<string, IParamsCollator>
            {
                { ContextKeyPage, new PageParamsCollator() },
                { ContextKeyReferring, new PageParamsCollator(true) },
                { ContextKeyCustom, new CustomParamsCollator() },
            };

        private static readonly TimeParamsCollator TimeParamsCollator = new ();

        private readonly TargetClientConfig clientConfig;
        private readonly ITargetService targetService;
        private readonly RuleLoader ruleLoader;
        private readonly ClusterLocator clusterLocator;
        private readonly DecisioningEvaluator decisioningEvaluator;
        private readonly DecisioningDetailsExecutor decisionHandler;
        private readonly IGeoClient geoClient;

        /// <summary>
        /// Initializes a new instance of the <see cref="OnDeviceDecisioningService"/> class.
        /// </summary>
        /// <param name="clientConfig">Client config</param>
        /// <param name="targetService">Target Service</param>
        /// <param name="geoClient">Optional Geo client</param>
        internal OnDeviceDecisioningService(TargetClientConfig clientConfig, ITargetService targetService, IGeoClient geoClient = null)
        {
            this.clientConfig = clientConfig;
            this.targetService = targetService;
            this.ruleLoader = new RuleLoader(clientConfig);
            this.clusterLocator = new ClusterLocator(clientConfig, targetService);
            this.decisioningEvaluator = new DecisioningEvaluator(this.ruleLoader);
            this.decisionHandler = new DecisioningDetailsExecutor(clientConfig);
            this.geoClient = geoClient ?? new GeoClient(clientConfig);
        }

        public TargetDeliveryResponse ExecuteRequest(TargetDeliveryRequest deliveryRequest)
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();

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
                    Messages.RulesetUnavailable);
            }

            var requestContext = new Dictionary<string, object>(TimeParamsCollator.CollateParams());
            this.GeoLookup(deliveryRequest.DeliveryRequest, ruleSet.GeoTargetingEnabled);
            requestContext.AddAll(this.CollateParams(RequestParamsCollators, deliveryRequest));

            var responseTokens = new HashSet<string>(ruleSet.ResponseTokens);
            var targetResponse = this.BuildDeliveryResponse(deliveryRequest);
            var visitorId = this.GetOrCreateVisitorId(deliveryRequest, targetResponse);
            var prefetchDetails = this.GetPrefetchDetails(deliveryRequest.DeliveryRequest);

            TraceHandler traceHandler = null;
            if (deliveryRequest.DeliveryRequest.Trace != null)
            {
                traceHandler = new TraceHandler(this.clientConfig, this.ruleLoader, ruleSet, deliveryRequest);
            }

            this.HandleDetails(
                prefetchDetails,
                requestContext,
                deliveryRequest,
                visitorId,
                responseTokens,
                traceHandler,
                ruleSet,
                targetResponse.Response.Prefetch);

            var executeDetails = this.GetExecuteDetails(deliveryRequest.DeliveryRequest);
            var notifications = new List<Notification>();

            this.HandleDetails(
                executeDetails,
                requestContext,
                deliveryRequest,
                visitorId,
                responseTokens,
                traceHandler,
                ruleSet,
                null,
                targetResponse.Response.Execute,
                notifications);

            stopwatch.Stop();
            var elapsedMilliseconds = (int)stopwatch.ElapsedMilliseconds;
            var telemetry = deliveryRequest.GetTelemetryEntry(this.clientConfig, elapsedMilliseconds);

            this.SendNotifications(deliveryRequest, targetResponse, notifications, telemetry);

            TargetClient.Logger?.LogDebug(targetResponse.ToString());
            return targetResponse;
        }

        public Task<TargetDeliveryResponse> ExecuteRequestAsync(TargetDeliveryRequest deliveryRequest)
        {
            return Task.Run(() => this.ExecuteRequest(deliveryRequest));
        }

        internal OnDeviceDecisioningEvaluation EvaluateLocalExecution(TargetDeliveryRequest request)
        {
            return this.decisioningEvaluator.EvaluateLocalExecution(request);
        }

        private static string RemoveLocationHint(string tntId)
        {
            if (string.IsNullOrEmpty(tntId))
            {
                return tntId;
            }

            var index = tntId.IndexOf('.');
            return index <= 0 ? tntId : tntId.Substring(0, index);
        }

        private void HandleDetails(
            IList<RequestDetailsUnion> detailsList,
            IDictionary<string, object> requestContext,
            TargetDeliveryRequest deliveryRequest,
            string visitorId,
            ISet<string> responseTokens,
            TraceHandler traceHandler,
            OnDeviceDecisioningRuleSet ruleSet,
            PrefetchResponse prefetchResponse,
            ExecuteResponse executeResponse = default,
            IList<Notification> notifications = default)
        {
            foreach (var details in detailsList)
            {
                var detailsContext = new Dictionary<string, object>(requestContext);
                detailsContext.AddAll(this.CollateParams(DetailsParamsCollators, deliveryRequest, details.GetRequestDetails()));

                this.decisionHandler.ExecuteDetails(
                    deliveryRequest,
                    detailsContext,
                    visitorId,
                    responseTokens,
                    traceHandler,
                    ruleSet,
                    details,
                    prefetchResponse,
                    executeResponse,
                    notifications);
            }
        }

        private void SendNotifications(TargetDeliveryRequest request, TargetDeliveryResponse targetResponse, List<Notification> notifications, TelemetryEntry telemetryEntry)
        {
            if (notifications.Count == 0 && telemetryEntry == null)
            {
                return;
            }

            var deliveryRequest = request.DeliveryRequest;
            var locationHint = request.LocationHint ?? this.clusterLocator.GetLocationHint();
            var telemetry = telemetryEntry != null ? new Telemetry(new List<TelemetryEntry> { telemetryEntry }) : null;

            var notificationRequest = new TargetDeliveryRequest.Builder()
                .SetLocationHint(locationHint)
                .SetSessionId(request.SessionId)
                .SetVisitor(request.Visitor)
                .SetDecisioningMethod(DecisioningMethod.ServerSide)
                .SetImpressionId(Guid.NewGuid().ToString())
                .SetId(deliveryRequest.Id ?? targetResponse.Response.Id)
                .SetExperienceCloud(deliveryRequest.ExperienceCloud)
                .SetContext(deliveryRequest.Context)
                .SetEnvironmentId(deliveryRequest.EnvironmentId)
                .SetQaMode(deliveryRequest.QaMode)
                .SetProperty(deliveryRequest.Property)
                .SetNotifications(notifications)
                .SetTelemetry(telemetry)
                .SetTrace(deliveryRequest.Trace)
                .Build();

            _ = Task.Run(() => _ = this.targetService.ExecuteRequest(notificationRequest));
        }

        private string GetOrCreateVisitorId(TargetDeliveryRequest deliveryRequest, TargetDeliveryResponse targetResponse)
        {
            string vid = null;
            var visitorId = deliveryRequest.DeliveryRequest.Id;
            if (visitorId != null)
            {
                vid = new[] { visitorId.MarketingCloudVisitorId, RemoveLocationHint(visitorId.TntId), visitorId.ThirdPartyId }
                    .FirstOrDefault(s => !string.IsNullOrEmpty(s));
            }

            vid ??= RemoveLocationHint(targetResponse.Response.Id?.TntId);

            if (vid != null)
            {
                return vid;
            }

            var newTntId = this.GenerateTntId();

            visitorId ??= new VisitorId();
            visitorId.TntId = newTntId;

            targetResponse.Response.Id = visitorId;

            return RemoveLocationHint(newTntId);
        }

        private string GenerateTntId()
        {
            var tntId = Guid.NewGuid().ToString();
            var locationHint = this.clusterLocator.GetLocationHint();

            return locationHint == null ? tntId : $"{tntId}.{locationHint}_0";
        }

        private TargetDeliveryResponse BuildDeliveryResponse(TargetDeliveryRequest deliveryRequest)
        {
            var localEvaluation = this.EvaluateLocalExecution(deliveryRequest);
            var status = localEvaluation.AllLocal ? HttpStatusCode.OK : HttpStatusCode.PartialContent;
            var deliveryResponse = new DeliveryResponse(
                (int)status,
                deliveryRequest.DeliveryRequest.RequestId,
                deliveryRequest.DeliveryRequest.Id,
                this.clientConfig.Client);
            deliveryResponse.Prefetch = new PrefetchResponse();
            deliveryResponse.Execute = new ExecuteResponse();
            var locations = new Locations(localEvaluation.RemoteMboxes, localEvaluation.RemoteViews, localEvaluation.GlobalMbox);

            return new TargetDeliveryResponse(
                deliveryRequest,
                deliveryResponse,
                status,
                localEvaluation.AllLocal ? Messages.OnDeviceResponse : localEvaluation.Reason,
                locations);
        }

        private IList<RequestDetailsUnion> GetExecuteDetails(DeliveryRequest deliveryRequest)
        {
            var result = new List<RequestDetailsUnion>();
            var execute = deliveryRequest.Execute;
            if (execute == null)
            {
                return result;
            }

            if (execute.Mboxes is { Count: > 0 })
            {
                result.AddRange(execute.Mboxes.Select(mbox => new RequestDetailsUnion(mbox)));
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

            if (prefetch.Mboxes is { Count: > 0 })
            {
                result.AddRange(prefetch.Mboxes.Select(mbox => new RequestDetailsUnion(mbox)));
            }

            if (prefetch.Views is { Count: > 0 })
            {
                result.AddRange(prefetch.Views.Select(view => new RequestDetailsUnion(view)));
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
