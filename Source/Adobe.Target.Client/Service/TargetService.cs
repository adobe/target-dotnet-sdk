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
namespace Adobe.Target.Client.Service
{
    using System.Collections.Generic;
    using System.Net;
    using System.Threading.Tasks;
    using Adobe.Target.Client.Model;
    using Adobe.Target.Client.Util;
    using Adobe.Target.Delivery.Api;
    using Adobe.Target.Delivery.Client;
    using Adobe.Target.Delivery.Model;

    /// <summary>
    /// Target Service
    /// </summary>
    public sealed class TargetService
    {
        private readonly TargetClientConfig clientConfig;
        private volatile DeliveryApi deliveryApi;
        private volatile string stickyLocationHint;
        private volatile string stickyBaseUrl;

        /// <summary>
        /// Initializes a new instance of the <see cref="TargetService"/> class.
        /// </summary>
        /// <param name="clientConfig">Target Client Config <see cref="TargetClientConfig"/></param>
        public TargetService(TargetClientConfig clientConfig)
        {
            this.clientConfig = clientConfig;
            this.stickyBaseUrl = this.clientConfig.DefaultUrl;
            this.SetDeliveryApi();
            RetryConfiguration.RetryPolicy = clientConfig.RetryPolicy;
            RetryConfiguration.AsyncRetryPolicy = clientConfig.AsyncRetryPolicy;
        }

        /// <summary>
        /// Execute sync Delivery request
        /// </summary>
        /// <param name="request">Target Delivery request</param>
        /// <returns>Target Delivery Response</returns>
        public TargetDeliveryResponse ExecuteRequest(TargetDeliveryRequest request)
        {
            this.SetUrl(this.GetLocationHint(request));
            var response = this.deliveryApi.Execute(this.clientConfig.OrganizationId, request.SessionId, request.DeliveryRequest);

            return this.GetTargetDeliveryResponse(request, response);
        }

        /// <summary>
        /// Execute async Delivery request
        /// </summary>
        /// <param name="request">Target Delivery request</param>
        /// <returns>Target Delivery Response</returns>
        public Task<TargetDeliveryResponse> ExecuteRequestAsync(TargetDeliveryRequest request)
        {
            this.SetUrl(this.GetLocationHint(request));
            var executeTask = this.deliveryApi.ExecuteAsync(this.clientConfig.OrganizationId, request.SessionId, request.DeliveryRequest);

            return executeTask.ContinueWith(task => this.GetTargetDeliveryResponse(request, task.Result), TaskScheduler.Default);
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
            this.UpdateStickyLocationHint(response);
            return new TargetDeliveryResponse(request, response, (HttpStatusCode)response.Status, null);
        }

        private void UpdateStickyLocationHint(DeliveryResponse deliveryResponse)
        {
            var tntId = deliveryResponse?.Id?.TntId;
            if (tntId != null
                && deliveryResponse.Status >= (int)HttpStatusCode.OK
                && deliveryResponse.Status < (int)HttpStatusCode.Ambiguous)
            {
                this.stickyLocationHint = CookieUtils.LocationHintFromTntId(tntId);
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

            this.stickyBaseUrl = newUrl;
            this.SetDeliveryApi();
        }

        private void SetDeliveryApi()
        {
            this.deliveryApi = new DeliveryApi(this.GetDeliveryApiConfig(this.stickyBaseUrl));
        }

        private string GetLocationHint(TargetDeliveryRequest request)
        {
            return request.LocationHint ?? this.stickyLocationHint;
        }
    }
}