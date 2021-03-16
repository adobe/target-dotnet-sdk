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
    using Action = System.Action;

    internal sealed class RuleLoader
    {
        private const string AcceptHeader = "Accept";
        private const string EtagHeader = "ETag";
        private const string NoneMatchHeader = "If-None-Match";
        private const string AcceptHeaderValue = "application/json";
        private const string MajorVersion = "1";
        private const string ArtifactFilename = "/rules.json";
        private const int MaxRetries = 5;

        private static readonly IReadOnlyList<HttpStatusCode> HttpStatusCodesWorthRetrying = new[]
        {
            HttpStatusCode.RequestTimeout,
            HttpStatusCode.InternalServerError,
            HttpStatusCode.BadGateway,
            HttpStatusCode.ServiceUnavailable,
            HttpStatusCode.GatewayTimeout,
        };

        private readonly int timerStartDelayMs;
        private readonly TargetClientConfig clientConfig;
        private readonly ILogger? logger;
        private Timer? timer;
        private volatile OnDeviceDecisioningRuleSet? latestRules;
        private string? lastETag;
        private bool succeeded;

        internal RuleLoader(TargetClientConfig clientConfig)
        {
            this.clientConfig = clientConfig;
            this.timerStartDelayMs = clientConfig.OnDeviceDecisioningPollingIntSecs * 1000;
            this.logger = TargetClient.Logger;
            this.SetLocalRules();
            this.ScheduleTimer(0);
        }

        internal OnDeviceDecisioningRuleSet? GetLatestRules()
        {
            return this.latestRules;
        }

        private static void RunOnDeviceDecisioningDelegate(Action? onDeviceDecisioningDelegate)
        {
            if (onDeviceDecisioningDelegate == null)
            {
                return;
            }

            _ = Task.Factory.StartNew(
                onDeviceDecisioningDelegate,
                CancellationToken.None,
                TaskCreationOptions.None,
                TaskScheduler.Default);
        }

        private void SetLocalRules()
        {
            var artifactPayload = this.clientConfig.OnDeviceArtifactPayload;
            if (string.IsNullOrEmpty(artifactPayload))
            {
                return;
            }

            if (!this.DeserializeRules(artifactPayload, out var deserialized))
            {
                return;
            }

            Interlocked.Exchange(ref this.latestRules, deserialized);

            if (!this.succeeded)
            {
                this.succeeded = true;
                RunOnDeviceDecisioningDelegate(this.clientConfig.OnDeviceDecisioningReady);
            }
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

            this.ScheduleTimer(this.timerStartDelayMs);
        }

        private IRestRequest GetRequest()
        {
            var request = new RestRequest(this.GetArtifactUrl(), Method.GET);
            if (this.lastETag != null)
            {
                request.AddHeader(NoneMatchHeader, this.lastETag);
            }

            return request;
        }

        private string GetArtifactUrl()
        {
            return this.clientConfig.Client + "/" + this.clientConfig.OnDeviceEnvironment.ToLowerInvariant()
                + "/v" + MajorVersion + ArtifactFilename;
        }

        private void ProcessResult(PolicyResult<IRestResponse> policyResult)
        {
            if (policyResult.Outcome == OutcomeType.Failure)
            {
                this.LogArtifactException(Messages.RuleLoadingFailed, policyResult.FinalException.Message);
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

            if (this.clientConfig.ArtifactDownloadSucceeded != null)
            {
                RunOnDeviceDecisioningDelegate(() => this.clientConfig.ArtifactDownloadSucceeded(result.Content));
            }

            Interlocked.Exchange(ref this.latestRules, deserialized);

            if (!this.succeeded)
            {
                this.succeeded = true;
                RunOnDeviceDecisioningDelegate(this.clientConfig.OnDeviceDecisioningReady);
            }
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
                this.LogArtifactException(Messages.RuleDeserializationFailed, e.Message);
                deserialized = null;
                return false;
            }

            if (deserialized.Version == null || !deserialized.Version.StartsWith(MajorVersion + '.'))
            {
                this.LogArtifactException(
                    Messages.RuleDeserializationFailed,
                    Messages.UnknownArtifactVersion + deserialized.Version);
                return false;
            }

            return true;
        }

        private void LogArtifactException(string message, string exceptionMessage)
        {
            var exception = new ApplicationException(message + exceptionMessage);
            this.logger.LogException(exception);
            if (this.clientConfig.ArtifactDownloadFailed == null)
            {
                return;
            }

            RunOnDeviceDecisioningDelegate(() => this.clientConfig.ArtifactDownloadFailed(exception));
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
