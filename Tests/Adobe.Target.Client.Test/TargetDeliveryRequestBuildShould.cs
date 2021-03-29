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
    using System.Net;
    using Delivery.Model;
    using Model;
    using Moq;
    using Util;
    using Xunit;

    public class TargetDeliveryRequestBuildShould
    {
        private const string tntId = "19b5c629-606d-44c0-977d-9a101f84c24c.37_0";
        private const string marketingCloudVisitorId = "testMarketingCloudVisitorId";
        private const string thirdPartyId = "testThirdPartyId";
        private const string impressionId = "impressionId";
        private const string locationHint = "31";
        private const string requestId = "123456";
        private const string sessionId = "19b5c629-606d-44c0-977d-9a101f84c24c";
        private const string mboxCookieFormat = "session#19b5c629-606d-44c0-977d-9a101f84c24c#{0}|PC#19b5c629-606d-44c0-977d-9a101f84c24c.37_0#{0}|";
        private const string clusterCookie = "31";

        [Fact]
        public void Build_ReturnDeliveryRequest()
        {
            var context = new Mock<Context>().Object;
            var execute = new ExecuteRequest();
            var prefetch = new PrefetchRequest();
            var notifications = new List<Notification>();
            var customerIds = new List<CustomerId>();
            var property = new Property("testToken");
            var telemetry = new Telemetry();
            var trace = new Trace("testAuthToken");
            var expCloud = new ExperienceCloud();
            var qaMode = new QAMode();

            var request = new TargetDeliveryRequest.Builder()
                .SetTntId(tntId)
                .SetMarketingCloudVisitorId(marketingCloudVisitorId)
                .SetThirdPartyId(thirdPartyId)
                .SetContext(context)
                .SetExecute(execute)
                .SetPrefetch(prefetch)
                .SetNotifications(notifications)
                .SetProperty(property)
                .SetTelemetry(telemetry)
                .SetTrace(trace)
                .SetExperienceCloud(expCloud)
                .SetEnvironmentId(environmentId)
                .SetImpressionId(impressionId)
                .SetQaMode(qaMode)
                .SetLocationHint(locationHint)
                .SetRequestId(requestId)
                .SetSessionId(sessionId)
                .SetTargetCustomerIds(customerIds)
                .SetDecisioningMethod(DecisioningMethod.Hybrid)
                .Build();

            Assert.Equal(tntId, request.DeliveryRequest.Id.TntId);
            Assert.Equal(marketingCloudVisitorId, request.DeliveryRequest.Id.MarketingCloudVisitorId);
            Assert.Equal(thirdPartyId, request.DeliveryRequest.Id.ThirdPartyId);
            Assert.Equal(customerIds, request.DeliveryRequest.Id.CustomerIds);
            Assert.Equal(context, request.DeliveryRequest.Context);
            Assert.Equal(execute, request.DeliveryRequest.Execute);
            Assert.Equal(prefetch, request.DeliveryRequest.Prefetch);
            Assert.Equal(notifications, request.DeliveryRequest.Notifications);
            Assert.Equal(property, request.DeliveryRequest.Property);
            Assert.Equal(telemetry, request.DeliveryRequest.Telemetry);
            Assert.Equal(trace, request.DeliveryRequest.Trace);
            Assert.Equal(expCloud, request.DeliveryRequest.ExperienceCloud);
            Assert.Equal(qaMode, request.DeliveryRequest.QaMode);
            Assert.Equal(environmentId, request.DeliveryRequest.EnvironmentId);
            Assert.Equal(impressionId, request.DeliveryRequest.ImpressionId);
            Assert.Equal(requestId, request.DeliveryRequest.RequestId);
            Assert.Equal(locationHint, request.LocationHint);
            Assert.Equal(sessionId, request.SessionId);
            Assert.Equal(DecisioningMethod.Hybrid, request.DecisioningMethod);
        }

        private const long environmentId = 3L;

        [Fact]
        public void Build_SetCookieData()
        {
            var maxAge = (int)(DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() / 1000) + 1000;
            var mboxCookie = string.Format(mboxCookieFormat, maxAge);

            var cookies = new CookieCollection
            {
                new Cookie(TargetConstants.MboxCookieName, mboxCookie),
                new Cookie(TargetConstants.ClusterCookieName, clusterCookie)
            };

            var request = new TargetDeliveryRequest.Builder()
                .SetCookies(cookies)
                .Build();

            Assert.Equal(tntId, request.DeliveryRequest.Id.TntId);
            Assert.Equal(sessionId, request.SessionId);
            Assert.Equal(locationHint, request.LocationHint);
        }
    }
}
