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
namespace Adobe.Target.Client
{
    using System;
    using System.Net;
    using System.Threading.Tasks;
    using Adobe.Target.Delivery.Model;
    using Microsoft.Extensions.Logging;
    using Polly;
    using RestSharp;
    using Action = System.Action;

    /// <summary>
    /// Target ClientConfig
    /// </summary>
    public sealed class TargetClientConfig
    {
        private const string ClusterPrefix = "mboxedge";
        private const string DefaultDomain = "tt.omtrdc.net";

        private TargetClientConfig()
        {
        }

        private TargetClientConfig(Builder builder)
        {
            ValidateConfig(builder);
            this.Client = builder.Client;
            this.OrganizationId = builder.OrganizationId;
            this.Protocol = $"{(builder.Secure ? Uri.UriSchemeHttps : Uri.UriSchemeHttp)}{Uri.SchemeDelimiter}";
            this.DefaultPropertyToken = builder.DefaultPropertyToken;
            this.DefaultUrl = builder.ServerDomain != DefaultDomain
                ? $"{this.Protocol}{builder.ServerDomain}"
                : $"{this.Protocol}{this.Client}.{DefaultDomain}";
            this.ClusterUrlPrefix = $"{this.Protocol}{ClusterPrefix}";
            this.ClusterUrlSuffix = $".{DefaultDomain}";
            this.Logger = builder.Logger;
            this.Timeout = builder.Timeout;
            this.Proxy = builder.Proxy;
            this.RetryPolicy = builder.RetryPolicy;
            this.AsyncRetryPolicy = builder.AsyncRetryPolicy;
            this.DecisioningMethod = builder.DecisioningMethod;
            this.TelemetryEnabled = builder.TelemetryEnabled;
            this.OnDeviceDecisioningReady = builder.OnDeviceDecisioningReady;
            this.ArtifactDownloadSucceeded = builder.ArtifactDownloadSucceeded;
            this.ArtifactDownloadFailed = builder.ArtifactDownloadFailed;
            this.OnDeviceEnvironment = builder.OnDeviceEnvironment;
            this.OnDeviceConfigHostname = builder.OnDeviceConfigHostname;
            this.OnDeviceDecisioningPollingIntSecs = builder.OnDeviceDecisioningPollingIntSecs;
            this.OnDeviceArtifactPayload = builder.OnDeviceArtifactPayload;
            this.LocalArtifactOnly = builder.LocalArtifactOnly;
        }

        /// <summary>
        /// OnDeviceDecisioningReady delegate
        /// </summary>
        public delegate void OnDeviceDecisioningOtherDelegate();

        /// <summary>
        /// Client
        /// </summary>
        public string Client { get; }

        /// <summary>
        /// OrganizationId
        /// </summary>
        public string OrganizationId { get; }

        /// <summary>
        /// Protocol
        /// </summary>
        public string Protocol { get; }

        /// <summary>
        /// DefaultUrl
        /// </summary>
        public string DefaultUrl { get; }

        /// <summary>
        /// Default Property Token
        /// </summary>
        public string DefaultPropertyToken { get; }

        /// <summary>
        /// Logger
        /// </summary>
        public ILogger Logger { get; }

        /// <summary>
        /// Timeout (defaults to 100000 ms)
        /// </summary>
        public int Timeout { get; }

        /// <summary>
        /// Proxy
        /// </summary>
        public WebProxy Proxy { get; }

        /// <summary>
        /// Retry Policy
        /// </summary>
        public Policy<IRestResponse> RetryPolicy { get; }

        /// <summary>
        /// Async Retry Policy
        /// </summary>
        public AsyncPolicy<IRestResponse> AsyncRetryPolicy { get; }

        /// <summary>
        /// Decisioning Method
        /// </summary>
        public DecisioningMethod DecisioningMethod { get; }

        /// <summary>
        /// Telemetry
        /// </summary>
        public bool TelemetryEnabled { get; }

        /// <summary>
        /// OnDeviceDecisioning Ready Delegate
        /// </summary>
        public Action OnDeviceDecisioningReady { get; }

        /// <summary>
        /// Artifact Download Succeeded Delegate
        /// </summary>
        public Action<string> ArtifactDownloadSucceeded { get; }

        /// <summary>
        /// Artifact Download Failed Delegate
        /// </summary>
        public Action<Exception> ArtifactDownloadFailed { get; }

        /// <summary>
        /// OnDevice Environment
        /// </summary>
        public string OnDeviceEnvironment { get; }

        /// <summary>
        /// OnDevice Configuration Hostname
        /// </summary>
        public string OnDeviceConfigHostname { get; }

        /// <summary>
        /// Number of seconds between OnDevice rule update requests
        /// </summary>
        public int OnDeviceDecisioningPollingIntSecs { get; }

        /// <summary>
        /// OnDevice Artifact Payload
        /// </summary>
        public string OnDeviceArtifactPayload { get; }

        /// <summary>
        /// When true, Target SDK won't attempt to update the locally set artifact <br/>
        /// Used together with <see cref="OnDeviceArtifactPayload"/>
        /// </summary>
        public bool LocalArtifactOnly { get; }

        /// <summary>
        /// ClusterUrlPrefix
        /// </summary>
        internal string ClusterUrlPrefix { get; }

        /// <summary>
        /// ClusterUrlSuffix
        /// </summary>
        internal string ClusterUrlSuffix { get; }

        private static void ValidateConfig(Builder builder)
        {
            if (builder.Client == null)
            {
                throw new ArgumentException("Client cannot be null");
            }

            if (builder.OrganizationId == null)
            {
                throw new ArgumentException("OrganizationId cannot be null");
            }
        }

        /// <summary>
        /// Target ClientConfig Builder
        /// </summary>
        public sealed class Builder
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="Builder"/> class.
            /// </summary>
            /// <param name="client">Client</param>
            /// <param name="organizationId">OrganizationId</param>
            public Builder(string client, string organizationId)
            {
                this.Client = client;
                this.OrganizationId = organizationId;
            }

            /// <summary>
            /// Client
            /// </summary>
            internal string Client { get; private set; }

            /// <summary>
            /// OrganizationId
            /// </summary>
            internal string OrganizationId { get; private set; }

            /// <summary>
            /// ServerDomain
            /// </summary>
            internal string ServerDomain { get; private set; } = DefaultDomain;

            /// <summary>
            /// DefaultPropertyToken
            /// </summary>
            internal string DefaultPropertyToken { get; private set; }

            /// <summary>
            /// Secure
            /// </summary>
            internal bool Secure { get; private set; } = true;

            /// <summary>
            /// Logger
            /// </summary>
            internal ILogger Logger { get; private set; }

            /// <summary>
            /// Timeout
            /// </summary>
            internal int Timeout { get; private set; } = 100000;

            /// <summary>
            /// Proxy
            /// </summary>
            internal WebProxy Proxy { get; private set; }

            /// <summary>
            /// Retry Policy
            /// </summary>
            internal Policy<IRestResponse> RetryPolicy { get; private set; }

            /// <summary>
            /// Async Retry Policy
            /// </summary>
            internal AsyncPolicy<IRestResponse> AsyncRetryPolicy { get; private set; }

            /// <summary>
            /// Decisioning Method
            /// </summary>
            internal DecisioningMethod DecisioningMethod { get; private set; } = DecisioningMethod.ServerSide;

            /// <summary>
            /// Telemetry (enabled by default)
            /// </summary>
            internal bool TelemetryEnabled { get; private set; } = true;

            /// <summary>
            /// OnDeviceDecisioning Ready delegate
            /// </summary>
            internal Action OnDeviceDecisioningReady { get; private set; }

            /// <summary>
            /// Artifact Download Succeeded delegate
            /// </summary>
            internal Action<string> ArtifactDownloadSucceeded { get; private set; }

            /// <summary>
            /// Artifact Download Failed delegate
            /// </summary>
            internal Action<Exception> ArtifactDownloadFailed { get; private set; }

            /// <summary>
            /// OnDevice Environment
            /// </summary>
            internal string OnDeviceEnvironment { get; private set; } = "production";

            /// <summary>
            /// OnDevice Configuration Hostname
            /// </summary>
            internal string OnDeviceConfigHostname { get; private set; } = "assets.adobetarget.com";

            /// <summary>
            /// Number of seconds between OnDevice rule update requests
            /// </summary>
            internal int OnDeviceDecisioningPollingIntSecs { get; private set; } = 300;

            /// <summary>
            /// OnDevice Artifact Payload
            /// </summary>
            internal string OnDeviceArtifactPayload { get; private set; }

            internal bool LocalArtifactOnly { get; private set; }

            /// <summary>
            /// Sets ServerDomain <br/>
            /// Default: "tt.omtrdc.net"
            /// </summary>
            /// <param name="serverDomain">ServerDomain</param>
            /// <returns><see cref="Builder"/> instance</returns>
            public Builder SetServerDomain(string serverDomain)
            {
                this.ServerDomain = serverDomain;
                return this;
            }

            /// <summary>
            /// Sets DefaultPropertyToken
            /// </summary>
            /// <param name="defaultPropertyToken">DefaultPropertyToken</param>
            /// <returns><see cref="Builder"/> instance</returns>
            public Builder SetDefaultPropertyToken(string defaultPropertyToken)
            {
                this.DefaultPropertyToken = defaultPropertyToken;
                return this;
            }

            /// <summary>
            /// Sets Secure
            /// </summary>
            /// <param name="secure">Secure</param>
            /// <returns><see cref="Builder"/> instance</returns>
            public Builder SetSecure(bool secure)
            {
                this.Secure = secure;
                return this;
            }

            /// <summary>
            /// Sets Logger
            /// </summary>
            /// <param name="logger">Logger</param>
            /// <returns><see cref="Builder"/> instance</returns>
            public Builder SetLogger(ILogger logger)
            {
                this.Logger = logger;
                return this;
            }

            /// <summary>
            /// Sets Timeout (in milliseconds) <br/>
            /// Default: 100000 ms
            /// </summary>
            /// <param name="timeout">Timeout</param>
            /// <returns><see cref="Builder"/> instance</returns>
            public Builder SetTimeout(int timeout)
            {
                this.Timeout = timeout;
                return this;
            }

            /// <summary>
            /// Sets Proxy
            /// </summary>
            /// <param name="proxy">Proxy</param>
            /// <returns><see cref="Builder"/> instance</returns>
            public Builder SetProxy(WebProxy proxy)
            {
                this.Proxy = proxy;
                return this;
            }

            /// <summary>
            /// Sets Retry Policy
            /// </summary>
            /// <param name="retryPolicy">Retry Policy</param>
            /// <returns><see cref="Builder"/> instance</returns>
            public Builder SetRetryPolicy(Policy<IRestResponse> retryPolicy)
            {
                this.RetryPolicy = retryPolicy;
                return this;
            }

            /// <summary>
            /// Sets Async Retry Policy
            /// </summary>
            /// <param name="asyncRetryPolicy">Async Retry Policy</param>
            /// <returns><see cref="Builder"/> instance</returns>
            public Builder SetRetryPolicy(AsyncPolicy<IRestResponse> asyncRetryPolicy)
            {
                this.AsyncRetryPolicy = asyncRetryPolicy;
                return this;
            }

            /// <summary>
            /// Sets Decisioning Method <br/>
            /// Default: ServerSide
            /// </summary>
            /// <param name="decisioningMethod">Decisioning Method</param>
            /// <returns><see cref="Builder"/> instance</returns>
            public Builder SetDecisioningMethod(DecisioningMethod decisioningMethod)
            {
                this.DecisioningMethod = decisioningMethod;
                return this;
            }

            /// <summary>
            /// Sets Telemetry
            /// </summary>
            /// <param name="telemetry">Telemetry</param>
            /// <returns><see cref="Builder"/> instance</returns>
            public Builder SetTelemetryEnabled(bool telemetry)
            {
                this.TelemetryEnabled = telemetry;
                return this;
            }

            /// <summary>
            /// Sets OnDeviceDecisioning Ready delegate
            /// This is called once when on-device execution is ready <br/>
            /// Note: Each callback delegate runs on a dedicated Task on <see cref="TaskScheduler.Default"/> scheduler
            /// </summary>
            /// <param name="delegate">OnDeviceDecisioning Ready delegate</param>
            /// <returns><see cref="Builder"/> instance</returns>
            public Builder SetOnDeviceDecisioningReady(Action @delegate)
            {
                this.OnDeviceDecisioningReady = @delegate;
                return this;
            }

            /// <summary>
            /// Sets Artifact Download Succeeded delegate <br/>
            /// Note: Each callback delegate runs on a dedicated Task on <see cref="TaskScheduler.Default"/> scheduler
            /// </summary>
            /// <param name="delegate">Artifact Download Succeeded delegate</param>
            /// <returns><see cref="Builder"/> instance</returns>
            public Builder SetArtifactDownloadSucceeded(Action<string> @delegate)
            {
                this.ArtifactDownloadSucceeded = @delegate;
                return this;
            }

            /// <summary>
            /// Sets Artifact Download Failed delegate <br/>
            /// Note: Each callback delegate runs on a dedicated Task on <see cref="TaskScheduler.Default"/> scheduler
            /// </summary>
            /// <param name="delegate">Artifact Download Failed delegate</param>
            /// <returns><see cref="Builder"/> instance</returns>
            public Builder SetArtifactDownloadFailed(Action<Exception> @delegate)
            {
                this.ArtifactDownloadFailed = @delegate;
                return this;
            }

            /// <summary>
            /// Sets OnDevice Environment <br/>
            /// Default: "production"
            /// </summary>
            /// <param name="environment">OnDevice Environment</param>
            /// <returns><see cref="Builder"/> instance</returns>
            public Builder SetOnDeviceEnvironment(string environment)
            {
                this.OnDeviceEnvironment = environment;
                return this;
            }

            /// <summary>
            /// Sets OnDevice Configuration Hostname <br/>
            /// Default: "assets.adobetarget.com"
            /// </summary>
            /// <param name="hostname">OnDevice Configuration Hostname</param>
            /// <returns><see cref="Builder"/> instance</returns>
            public Builder SetOnDeviceConfigHostname(string hostname)
            {
                this.OnDeviceConfigHostname = hostname;
                return this;
            }

            /// <summary>
            /// Sets OnDevice Decisioning Artifact Payload
            /// </summary>
            /// <param name="artifactPayload">OnDevice Decisioning Artifact Payload</param>
            /// <returns><see cref="Builder"/> instance</returns>
            public Builder SetOnDeviceArtifactPayload(string artifactPayload)
            {
                this.OnDeviceArtifactPayload = artifactPayload;
                return this;
            }

            /// <summary>
            /// Sets the number of seconds between OnDevice rule update requests <br/>
            /// Default: 300
            /// </summary>
            /// <param name="pollingSeconds">OnDevice Decisioning Polling Interval Seconds</param>
            /// <returns><see cref="Builder"/> instance</returns>
            public Builder SetOnDeviceDecisioningPollingIntSecs(int pollingSeconds)
            {
                this.OnDeviceDecisioningPollingIntSecs = pollingSeconds;
                return this;
            }

            /// <summary>
            /// Builds the <see cref="TargetClientConfig"/>
            /// </summary>
            /// <returns>Built <see cref="TargetClientConfig"/></returns>
            public TargetClientConfig Build()
            {
                return new (this);
            }

            internal Builder SetLocalArtifactOnly(bool localOnly)
            {
                this.LocalArtifactOnly = localOnly;
                return this;
            }
        }
    }
}
