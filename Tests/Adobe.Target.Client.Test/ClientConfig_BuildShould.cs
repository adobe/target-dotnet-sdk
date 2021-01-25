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
    using Microsoft.Extensions.Logging;
    using Moq;
    using Xunit;

    public class ClientConfig_BuildShould
    {
        private const string DeliveryPathSuffix = "/rest/v1/delivery";
        private const string testClientId = "testClientId";
        private const string testOrgId = "testClientId";
        private const string testDomain = "testOrgId";
        private const string testPropertyToken = "testPropertyToken";
        private const int testTimeout = 20000;

        [Fact]
        public void Build_ReturnClientConfig()
        {
            WebProxy testWebProxy = new Mock<WebProxy>().Object;
            ILogger testLogger = new Mock<ILogger>().Object;

            ClientConfig clientConfig = new ClientConfig.Builder(testClientId, testOrgId)
                .SetServerDomain(testDomain)
                .SetDefaultPropertyToken(testPropertyToken)
                .SetSecure(true)
                .SetLogger(testLogger)
                .SetTimeout(testTimeout)
                .SetProxy(testWebProxy)
                .Build();

            Assert.Equal(testClientId, clientConfig.Client);
            Assert.Equal(testOrgId, clientConfig.OrganizationId);
            Assert.Equal("https://" + testClientId + "." + testDomain + DeliveryPathSuffix, clientConfig.DefaultUrl);
            Assert.Equal(testPropertyToken, clientConfig.DefaultPropertyToken);
            Assert.Equal(testLogger, clientConfig.Logger);
            Assert.Equal(testTimeout, clientConfig.Timeout);
            Assert.Equal(testWebProxy, clientConfig.Proxy);
        }
    }
}
