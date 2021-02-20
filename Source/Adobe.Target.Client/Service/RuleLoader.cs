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
namespace Adobe.Target.Client.Service
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Threading;
    using System.Threading.Tasks;
    using Adobe.Target.Client.Extension;
    using Adobe.Target.Client.Model.OnDevice;
    using Adobe.Target.Client.Util;
    using Adobe.Target.Delivery.Model;
    using Microsoft.Extensions.Logging;
    using Newtonsoft.Json;
    using Polly;
    using RestSharp;

    internal sealed class RuleLoader
    {
        private const string AcceptHeader = "Accept";
        private const string EtagHeader = "ETag";
        private const string NoneMatchHeader = "If-None-Match";
        private const string AcceptHeaderValue = "application/json";
        private const string MajorVersion = "1";
        private const string ArtifactFilename = "/rules.json";
        private const int MaxRetries = 10;

        private static readonly IReadOnlyList<HttpStatusCode> HttpStatusCodesWorthRetrying = new[]
        {
            HttpStatusCode.RequestTimeout,
            HttpStatusCode.InternalServerError,
            HttpStatusCode.BadGateway,
            HttpStatusCode.ServiceUnavailable,
            HttpStatusCode.GatewayTimeout,
        };

        private readonly TargetClientConfig clientConfig;
        private readonly ILogger? logger;
        private Timer? timer;
        private volatile OnDeviceDecisioningRuleSet? latestRules;
        private string? lastETag;

        internal RuleLoader(TargetClientConfig clientConfig)
        {
            this.clientConfig = clientConfig;
            this.logger = this.clientConfig.Logger;
            this.SetLocalRules();
            this.ScheduleTimer(0);
        }

        private void SetLocalRules()
        {
            var artifactPayload = this.clientConfig.OnDeviceArtifactPayload;
            if (string.IsNullOrEmpty(artifactPayload))
            {
                return;
            }

            // TODO: parse and set local artifact
        }

        private async Task LoadRulesAsync()
        {
            if (this.clientConfig.DecisioningMethod == DecisioningMethod.ServerSide)
            {
                return;
            }

            RestClient client = new (Uri.UriSchemeHttps + "://" + this.clientConfig.OnDeviceConfigHostname);

            client.ClearHandlers();
            client.AddDefaultHeader(AcceptHeader, AcceptHeaderValue);

            if (this.clientConfig.Proxy != null)
            {
                client.Proxy = this.clientConfig.Proxy;
            }

            var policyResult = await Policy
                .Handle<HttpRequestException>()
                .OrResult<IRestResponse>(r => HttpStatusCodesWorthRetrying.Contains(r.StatusCode))
                .RetryAsync(MaxRetries)
                .ExecuteAndCaptureAsync(() => client.ExecuteAsync(this.GetRequest()))
                .ConfigureAwait(false);

            this.ProcessResult(policyResult);

            this.ScheduleTimer(2000);
        }

        private IRestRequest GetRequest()
        {
            var artifactPath = this.clientConfig.Client + "/" + this.clientConfig.OnDeviceEnvironment.ToLower()
                       + "/v" + MajorVersion + ArtifactFilename;
            var request = new RestRequest(artifactPath, Method.GET);
            if (this.lastETag != null)
            {
                request.AddHeader(NoneMatchHeader, this.lastETag);
            }

            return request;
        }

        private void ProcessResult(PolicyResult<IRestResponse> policyResult)
        {
            if (policyResult.Outcome == OutcomeType.Failure)
            {
                this.logger.LogException(Messages.RuleLoadingFailed, policyResult.FinalException);
                return;
            }

            var result = policyResult.Result;

            this.SetLastEtag(result);

            if (result.StatusCode == HttpStatusCode.NotModified)
            {
                return;
            }

            if (!this.DeserializeRules(result.Content, out var deserialized))
            {
                return;
            }

            Interlocked.Exchange(ref this.latestRules, deserialized);
        }

        private void SetLastEtag(IRestResponse response)
        {
            foreach (var header in response.Headers)
            {
                if (header.Name != EtagHeader || header.Value == null)
                {
                    continue;
                }

                this.lastETag = header.Value?.ToString();
                break;
            }
        }

        private bool DeserializeRules(string rules, out OnDeviceDecisioningRuleSet? deserialized)
        {
            try
            {
                deserialized = JsonConvert.DeserializeObject<OnDeviceDecisioningRuleSet>(rules);
            }
            catch (Exception e)
            {
                this.logger.LogException(Messages.RuleDeserializationFailed, e);
                deserialized = null;
                return false;
            }

            return true;
        }

        private void ScheduleTimer(int startDelay)
        {
            this.timer?.Dispose();
            this.timer = new Timer(this.LoadRules, null, startDelay, Timeout.Infinite);
        }

        private void LoadRules(object? state)
        {
            _ = this.LoadRulesAsync();
        }
    }
}
