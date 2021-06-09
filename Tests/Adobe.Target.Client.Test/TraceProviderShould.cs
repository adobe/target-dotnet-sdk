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
namespace Adobe.Target.Client.Test
{
    using System;
    using System.Collections.Generic;
    using Client.Util;
    using Delivery.Model;
    using Model;
    using Model.OnDevice;
    using Moq;
    using OnDevice;
    using Xunit;

    public class TraceProviderShould
    {
        private const string Client = "testClient";
        private const string OrgId = "testOrgId";
        private const int PollingInterval = 123;
        private const int FetchCount = 123;
        private const string ArtifactUrl = "artifactUrl";
        private const string ArtifactVersion = "1.2.3";
        private const string SessionId = "abcdefghijklmnop";
        private const string LocationHint = "35_0";
        private const string MarketingCloudId = "testMarketingCloudId123";
        private const string LastFetchString = "0021-06-01T04:20Z";
        private static readonly string TntId = $"{SessionId}.{LocationHint}";
        private static readonly DateTimeOffset LastFetch = new (21, 06, 01, 04, 20, 00, TimeSpan.Zero);
        private static readonly IDictionary<string, object> RuleSetMeta = new Dictionary<string, object> {{ "environment", "production" }};

        [Fact]
        public void Constructor_InitializeTrace()
        {
            var clientConfig = new TargetClientConfig.Builder(Client, OrgId).Build();
            VisitorProvider.Initialize(clientConfig.OrganizationId);

            var ruleLoaderMock = new Mock<IRuleLoader>();
            ruleLoaderMock.SetupGet(r => r.PollingInterval).Returns(PollingInterval);
            ruleLoaderMock.SetupGet(r => r.FetchCount).Returns(FetchCount);
            ruleLoaderMock.SetupGet(r => r.ArtifactUrl).Returns(ArtifactUrl);
            ruleLoaderMock.SetupGet(r => r.LastFetch).Returns(LastFetch);

            var ruleSet = new OnDeviceDecisioningRuleSet(ArtifactVersion, null, false, null, null, null, null, null, null, RuleSetMeta);

            var request = new DeliveryRequest(context: new Context(ChannelType.Web), id: new VisitorId(TntId, marketingCloudVisitorId: MarketingCloudId));
            var deliveryRequest = new TargetDeliveryRequest.Builder(request).Build();

            var traceHandler = new TraceHandler(clientConfig, ruleLoaderMock.Object, ruleSet, deliveryRequest);

            var trace = traceHandler.CurrentTrace;
            Assert.Equal(Client, trace["clientCode"]);

            var artifactTrace = (Dictionary<string, object>)trace["artifact"];

            Assert.Equal(ArtifactVersion, artifactTrace["artifactVersion"]);
            Assert.Equal(PollingInterval, artifactTrace["pollingInterval"]);
            Assert.Equal(FetchCount, artifactTrace["artifactRetrievalCount"]);
            Assert.Equal(ArtifactUrl, artifactTrace["artifactLocation"]);
            Assert.Equal(LastFetchString, artifactTrace["artifactLastRetrieved"]);

            var visitorTrace = (Dictionary<string, object>)((Dictionary<string, object>)trace["profile"])["visitorId"];

            Assert.Equal(TntId, visitorTrace["tntId"]);
            Assert.Equal(LocationHint, visitorTrace["profileLocation"]);
            Assert.Equal(MarketingCloudId, visitorTrace["marketingCloudVisitorId"]);
        }
    }
}
