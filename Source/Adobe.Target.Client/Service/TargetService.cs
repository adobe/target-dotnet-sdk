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
namespace Adobe.Target.Client.Service
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Threading;
    using System.Threading.Tasks;
    using Adobe.Target.Client.Extension;
    using Adobe.Target.Client.Model;
    using Adobe.Target.Client.Util;
    using Adobe.Target.Delivery.Api;
    using Adobe.Target.Delivery.Client;
    using Adobe.Target.Delivery.Model;
    using Microsoft.Extensions.Logging;
    using Newtonsoft.Json.Linq;
    using Action = Adobe.Target.Delivery.Model.Action;

    /// <summary>
    /// Target Service
    /// </summary>
    internal sealed class TargetService : ITargetService
    {
        private readonly TargetClientConfig clientConfig;
        private readonly ILogger logger;
        private readonly DeliveryApi deliveryApi;
        private volatile string stickyLocationHint;
        private volatile string stickyBaseUrl;

        /// <summary>
        /// Initializes a new instance of the <see cref="TargetService"/> class.
        /// </summary>
        /// <param name="clientConfig">Target Client Config <see cref="TargetClientConfig"/></param>
        internal TargetService(TargetClientConfig clientConfig)
        {
            this.clientConfig = clientConfig;
            this.logger = TargetClient.Logger;
            this.stickyBaseUrl = this.clientConfig.DefaultUrl;
            this.deliveryApi = new DeliveryApi(this.GetDeliveryApiConfig(this.stickyBaseUrl))
            {
                ExceptionFactory = clientConfig.ExceptionFactory,
            };
            RetryConfiguration.RetryPolicy = clientConfig.RetryPolicy;
            RetryConfiguration.AsyncRetryPolicy = clientConfig.AsyncRetryPolicy;
        }

        public TargetDeliveryResponse ExecuteRequest(TargetDeliveryRequest request)
        {
            this.SetUrl(this.GetLocationHint(request));
            request.AddTelemetry(this.clientConfig);
            this.logger.LogRequest(request);
            var response = this.deliveryApi.Execute(this.clientConfig.OrganizationId, request.SessionId, request.DeliveryRequest);

            return this.GetTargetDeliveryResponse(request, response);
        }

        public Task<TargetDeliveryResponse> ExecuteRequestAsync(TargetDeliveryRequest request)
        {
            this.SetUrl(this.GetLocationHint(request));
            request.AddTelemetry(this.clientConfig);
            this.logger.LogRequest(request);
            var executeTask = this.deliveryApi.ExecuteAsync(this.clientConfig.OrganizationId, request.SessionId, request.DeliveryRequest);

            return executeTask.ContinueWith(task => this.GetTargetDeliveryResponse(request, task.Result), TaskScheduler.Default);
        }

        internal static DeliveryResponse ConvertResponseOptions(DeliveryResponse deliveryResponse)
        {
            var allOptions = new List<List<Option>>
            {
                deliveryResponse.Execute?.PageLoad?.Options, deliveryResponse.Prefetch?.PageLoad?.Options,
            };
            allOptions.AddRange(deliveryResponse.Execute?.Mboxes?.Select(response => response.Options) ?? Array.Empty<List<Option>>());
            allOptions.AddRange(deliveryResponse.Prefetch?.Mboxes?.Select(response => response.Options) ?? Array.Empty<List<Option>>());
            allOptions.AddRange(deliveryResponse.Prefetch?.Views?.Select(response => response.Options) ?? Array.Empty<List<Option>>());

            allOptions.ForEach(ConvertOptionsContent);

            return deliveryResponse;
        }

        private static void ConvertOptionsContent(IEnumerable<Option> options)
        {
            if (options == null)
            {
                return;
            }

            foreach (var option in options.Where(option => option.Type == OptionType.Actions))
            {
                option.Content = ((JArray)option.Content).ToObject<IList<Action>>();
            }
        }

        private static IDictionary<string, string> GetHeaders()
        {
            return new Dictionary<string, string>()
            {
                { TargetConstants.SdkNameHeader, TargetConstants.SdkNameValue },
                { TargetConstants.SdkVersionHeader, TargetConstants.SdkVersion },
            };
        }

        private TargetDeliveryResponse GetTargetDeliveryResponse(TargetDeliveryRequest request, DeliveryResponse response)
        {
            if (response == null)
            {
                this.logger?.LogWarning("Null response for requestId: {requestId}, sessionId: {sessionId}", request.DeliveryRequest.RequestId, request.SessionId);
                return new TargetDeliveryResponse(request, null, HttpStatusCode.ServiceUnavailable);
            }

            this.UpdateStickyLocationHint(response);
            this.logger.LogResponse(response);
            return new TargetDeliveryResponse(request, ConvertResponseOptions(response), (HttpStatusCode)response.Status);
        }

        private void UpdateStickyLocationHint(DeliveryResponse deliveryResponse)
        {
            var tntId = deliveryResponse?.Id?.TntId;
            if (tntId != null
                && deliveryResponse.Status >= (int)HttpStatusCode.OK
                && deliveryResponse.Status < (int)HttpStatusCode.Ambiguous)
            {
                Interlocked.Exchange(ref this.stickyLocationHint, CookieUtils.LocationHintFromTntId(tntId));
            }
        }

        private Configuration GetDeliveryApiConfig(string basePath)
        {
            return new ()
            {
                BasePath = basePath,
                UserAgent = TargetConstants.SdkUserAgent,
                DefaultHeaders = GetHeaders(),
                Timeout = this.clientConfig.Timeout,
                Proxy = this.clientConfig.Proxy,
            };
        }

        private void SetUrl(string locationHint)
        {
            if (string.IsNullOrEmpty(locationHint))
            {
                return;
            }

            var newUrl = this.clientConfig.ClusterUrlPrefix + locationHint + this.clientConfig.ClusterUrlSuffix;
            if (newUrl == this.stickyBaseUrl)
            {
                return;
            }

            Interlocked.Exchange(ref this.stickyBaseUrl, newUrl);
            ((ApiClient)this.deliveryApi.Client).SetBaseUrl(this.stickyBaseUrl);
            ((ApiClient)this.deliveryApi.AsynchronousClient).SetBaseUrl(this.stickyBaseUrl);
        }

        private string GetLocationHint(TargetDeliveryRequest request)
        {
            return request.LocationHint ?? this.stickyLocationHint;
        }
    }
}
