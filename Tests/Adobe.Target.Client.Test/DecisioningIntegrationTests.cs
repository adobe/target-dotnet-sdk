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
    using System.Linq;
    using System.Threading;
    using Client.Util;
    using Delivery.Model;
    using FluentAssertions;
    using Model;
    using Moq;
    using Service;
    using Util;
    using Xunit;
    using Xunit.Abstractions;

    [Collection("Datetime-mocked Collection")]
    public class DecisioningIntegrationTests : IClassFixture<IntegrationTestFixture>
    {
        private readonly IntegrationTestFixture fixture;
        private readonly ITestOutputHelper output;

        public DecisioningIntegrationTests(IntegrationTestFixture fixture, ITestOutputHelper output)
        {
            this.fixture = fixture;
            this.output = output;
        }

        [Fact]
        public void RunIntegrationTests()
        {
            foreach (var entry in this.fixture.Tests)
            {
                IDictionary<string, object> suite = entry.Value;
                this.output.WriteLine($"Suite: {entry.Key} - {suite["description"]}");
                var tests = (IDictionary<string, object>) suite["test"];
                this.RunSuiteTests(suite, tests);
            }
        }

        [Fact]
        public void RunSingleIntegrationTest()
        {
            const string suiteName = "TEST_SUITE_TIMEFRAME";
            const string testName = "date_range_1";

            var suite = (IDictionary<string, object>) this.fixture.Tests[suiteName];
            var test = ((IDictionary<string, object>) suite["test"])[testName];

            this.RunSuiteTest(suite, new KeyValuePair<string, object>(testName, test));
        }

        private void RunSuiteTests(IDictionary<string, object> suite, IDictionary<string, object> tests)
        {
            foreach (var testEntry in tests)
            {
                this.RunSuiteTest(suite, testEntry);
            }
        }

        private void RunSuiteTest(IDictionary<string, object> suite, KeyValuePair<string, object> testEntry)
        {
            var test = (IDictionary<string, object>) testEntry.Value;
            this.output.WriteLine($"Test: {testEntry.Key} - {test["description"]}");

            var config = (IDictionary<string, object>) (test.ContainsKey("conf") ? test["conf"] : suite["conf"]);
            var artifact = (string) (test.ContainsKey("artifact") ? test["artifact"] : suite["artifact"]);
            var clientConfig = this.GetTargetClientConfig(config, artifact);

            VisitorProvider.Initialize(clientConfig.OrganizationId);
            var mockTargetService = new Mock<ITargetService>();
            var mockGeo = IntegrationTestUtils.GetMockGeo(test);
            IntegrationTestUtils.SetupMockDateTime(test);
            var decisioningService = new OnDeviceDecisioningService(clientConfig, mockTargetService.Object, mockGeo?.Object);
            var targetRequest = GetTargetDeliveryRequest(test);

            var expectedResponseObject = test.ContainsKey("output") ? test["output"] : null;
            var expectedNotificationObject = test.ContainsKey("notificationOutput") ? test["notificationOutput"] : null;
            var response = decisioningService.ExecuteRequest(targetRequest);

            if (expectedResponseObject != null)
            {
                var expectedResponse = IntegrationTestUtils.ConvertExpectedResponse(expectedResponseObject);
                _ = response.Response.Should().BeEquivalentTo(expectedResponse, IntegrationTestUtils.RootResponseEquivalenceOptions);
            }

            if (expectedNotificationObject != null)
            {
                var expectedNotificationRequestObject = ((IDictionary<string, object>) expectedNotificationObject)["request"];
                var expectedNotification = SerializationUtils.ConvertObject<DeliveryRequest>(expectedNotificationRequestObject);
                Thread.Sleep(500);
                var notificationRequest = ((TargetDeliveryRequest) mockTargetService.Invocations.Last().Arguments.First()).DeliveryRequest;
                _ = notificationRequest.Should().BeEquivalentTo(expectedNotification, IntegrationTestUtils.RootRequestEquivalenceOptions);
            }

            TimeProvider.ResetToDefault();
        }

        private static TargetDeliveryRequest GetTargetDeliveryRequest(IDictionary<string, object> test)
        {
            var requestObject = ((IDictionary<string, object>) test["input"])["request"];
            var deliveryRequest = SerializationUtils.ConvertObject<DeliveryRequest>(requestObject);

            return new TargetDeliveryRequest.Builder(deliveryRequest).Build();
        }

        private TargetClientConfig GetTargetClientConfig(IDictionary<string, object> config, string artifact)
        {
            var configBuilder =
                new TargetClientConfig.Builder((string) config["client"], (string) config["organizationId"])
                    .SetUpdateLocalArtifact(false)
                    .SetOnDeviceArtifactPayload(this.fixture.Artifacts[artifact]);

            if (config.ContainsKey("decisioningMethod"))
            {
                // Enum.TryParse<DecisioningMethod>((string)config["decisioningMethod"], out var parsedDecisioning);
                var e = Enum.Parse<DecisioningMethod>((string)config["decisioningMethod"]);
                configBuilder.SetDecisioningMethod(e);
            }

            if (config.ContainsKey("telemetryEnabled"))
            {
                configBuilder.SetTelemetryEnabled((bool) config["telemetryEnabled"]);
            }

            return configBuilder.Build();
        }
    }
}
