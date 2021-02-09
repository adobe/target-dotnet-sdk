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
namespace Adobe.Target.Client.Test
{
    using System.Net;
    using Adobe.Target.Client;
    using Delivery.Model;
    using Microsoft.Extensions.Logging;
    using Moq;
    using Xunit;

    public class ClientConfigBuildShould
    {
        private const string testClientId = "testClientId";
        private const string testOrgId = "testOrgId";
        private const string testDomain = "testDomain";
        private const string testPropertyToken = "testPropertyToken";
        private const int testTimeout = 20000;

        [Fact]
        public void Build_ReturnClientConfig()
        {
            var testWebProxy = new Mock<WebProxy>().Object;
            var testLogger = new Mock<ILogger>().Object;

            var targetClientConfig = new TargetClientConfig.Builder(testClientId, testOrgId)
                .SetServerDomain(testDomain)
                .SetDefaultPropertyToken(testPropertyToken)
                .SetSecure(true)
                .SetLogger(testLogger)
                .SetTimeout(testTimeout)
                .SetProxy(testWebProxy)
                .SetDecisioningMethod(DecisioningMethod.OnDevice)
                .Build();

            Assert.Equal(testClientId, targetClientConfig.Client);
            Assert.Equal(testOrgId, targetClientConfig.OrganizationId);
            Assert.Equal("https://" + testClientId + "." + testDomain, targetClientConfig.DefaultUrl);
            Assert.Equal(testPropertyToken, targetClientConfig.DefaultPropertyToken);
            Assert.Equal(testLogger, targetClientConfig.Logger);
            Assert.Equal(testTimeout, targetClientConfig.Timeout);
            Assert.Equal(testWebProxy, targetClientConfig.Proxy);
            Assert.Equal(DecisioningMethod.OnDevice, targetClientConfig.DecisioningMethod);
        }
    }
}
