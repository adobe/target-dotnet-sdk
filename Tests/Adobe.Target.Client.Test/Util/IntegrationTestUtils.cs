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
namespace Adobe.Target.Client.Test.Util
{
    using System;
    using System.Collections.Generic;
    using Client.Util;
    using Delivery.Model;
    using FluentAssertions;
    using FluentAssertions.Equivalency;
    using Moq;
    using Newtonsoft.Json;
    using OnDevice;
    using Service;
    using Action = Delivery.Model.Action;

    public static class IntegrationTestUtils
    {
        public static T ConvertObject<T>(object from)
        {
            var serialized = JsonConvert.SerializeObject(from);
            return JsonConvert.DeserializeObject<T>(serialized);
        }

        public static DeliveryResponse ConvertExpectedResponse(object expectedResponseObject)
        {
            var deserialized = ConvertObject<DeliveryResponse>(expectedResponseObject);
            return TargetService.ConvertResponseOptions(deserialized);
        }

        public static EquivalencyAssertionOptions<DeliveryResponse> RootResponseEquivalenceOptions(
            EquivalencyAssertionOptions<DeliveryResponse> options) =>
            options
                .ExcludingMissingMembers()
                .Excluding(ctx => ctx.Path == "_Client")
                .IgnoringCyclicReferences()
                .ComparingByMembers<DeliveryResponse>()
                .ComparingByMembers<VisitorId>()
                .ComparingByMembers<PageLoadResponse>()
                .ComparingByMembers<ExecuteResponse>()
                .ComparingByMembers<PrefetchResponse>()
                .Using<int>(ctx =>
                {
                    if (ctx.Expectation == 0)
                    {
                        ctx.Subject.Should().BeInRange(200, 299);
                        return;
                    }

                    ctx.Subject.Should().Be(ctx.Expectation);
                })
                .When(info => info.Path == "Status")
                .Using<string>(ctx =>
                {
                    if (ctx.Expectation == null)
                    {
                        ctx.Subject.Should().NotBeEmpty();
                        return;
                    }

                    ctx.Subject.Should().Be(ctx.Expectation);
                })
                .When(info => info.Path == "RequestId")
                .Using<PrefetchResponse>(ctx =>
                {
                    if (ctx.Expectation == null)
                    {
                        return;
                    }

                    ctx.Subject.Should().BeEquivalentTo(ctx.Expectation, NestedResponseEquivalenceOptions);
                })
                .When(info => info.Type == typeof(PrefetchResponse))
                .Using<ExecuteResponse>(ctx =>
                {
                    if (ctx.Expectation == null)
                    {
                        return;
                    }

                    ctx.Subject.Should().BeEquivalentTo(ctx.Expectation, NestedResponseEquivalenceOptions);
                })
                .When(info => info.Type == typeof(ExecuteResponse))
                .Using<VisitorId>(ctx =>
                {
                    if (ctx.Expectation == null)
                    {
                        return;
                    }

                    ctx.Subject.Should().BeEquivalentTo(ctx.Expectation, NestedResponseEquivalenceOptions);
                })
                .When(info => info.Type == typeof(VisitorId))
                .WithTracing();

        public static Mock<IGeoClient> GetMockGeo(IDictionary<string, object> test)
        {
            if (!test.ContainsKey("mockGeo"))
            {
                return null;
            }

            var mockGeo = ConvertObject<Geo>(test["mockGeo"]);

            var geoMock = new Mock<IGeoClient>();
            geoMock.Setup(x => x.LookupGeoAsync(It.IsAny<Geo>()))
                .ReturnsAsync<Geo, IGeoClient, Geo>(geo =>
                {
                    if (geo != null && !string.IsNullOrEmpty(geo.IpAddress))
                    {
                        mockGeo.IpAddress = geo.IpAddress;
                    }

                    return mockGeo;
                });

            return geoMock;
        }

        public static void SetupMockDateTime(IDictionary<string, object> test)
        {
            if (!test.ContainsKey("mockDate"))
            {
                return;
            }

            var mockDateObj = (IDictionary<string, object>)test["mockDate"];
            mockDateObj.TryGetValue("year", out var year);
            mockDateObj.TryGetValue("month", out var month);
            mockDateObj.TryGetValue("date", out var date);
            mockDateObj.TryGetValue("hours", out var hours);

            var minutes = 0;
            if (mockDateObj.TryGetValue("minutes", out var minutesInt64))
            {
                minutes = Convert.ToInt32(minutesInt64);
            }

            var mockDateTime = DateTime.SpecifyKind(
                new DateTime(Convert.ToInt32(year), Convert.ToInt32(month), Convert.ToInt32(date),
                    Convert.ToInt32(hours), minutes, 0), DateTimeKind.Utc);

            var timeMock = new Mock<TimeProvider>();
            timeMock
                .SetupGet(tp => tp.UtcNow)
                .Returns(mockDateTime.ToUniversalTime());
            TimeProvider.Current = timeMock.Object;
        }

        private static EquivalencyAssertionOptions<T> NestedResponseEquivalenceOptions<T>(
            EquivalencyAssertionOptions<T> options) =>
            options
                .ExcludingMissingMembers()
                .ComparingByMembers<VisitorId>()
                .ComparingByMembers<PageLoadResponse>()
                .ComparingByMembers<ExecuteResponse>()
                .ComparingByMembers<PrefetchResponse>()
                .ComparingByMembers<MboxResponse>()
                .ComparingByMembers<PrefetchMboxResponse>()
                .ComparingByMembers<View>()
                .ComparingByMembers<Option>()
                .ComparingByMembers<Action>()
                .Using<Dictionary<string, object>>(ctx =>
                {
                    if (ctx.Expectation == null)
                    {
                        return;
                    }

                    ctx.Subject.Should().BeEquivalentTo(ctx.Expectation);
                })
                .When(info => info.Path.EndsWith(".ResponseTokens"))
                .WithTracing();

        public static EquivalencyAssertionOptions<DeliveryRequest> RootRequestEquivalenceOptions(
            EquivalencyAssertionOptions<DeliveryRequest> options) =>
            options
                .ExcludingMissingMembers()
                .Excluding(ctx => ctx.Path == "RequestId")
                .Excluding(ctx => ctx.Path == "ImpressionId")
                .Excluding(ctx => ctx.Path == "Context.UserAgent")
                .Excluding(ctx => ctx.Path == "ExperienceCloud")
                .IgnoringCyclicReferences()
                .ComparingByMembers<DeliveryRequest>()
                .ComparingByMembers<Context>()
                .ComparingByMembers<Notification>()
                .ComparingByMembers<Telemetry>()
                .ComparingByMembers<TelemetryEntry>()
                .ComparingByMembers<TelemetryFeatures>()
                .Using<string>(ctx =>
                {
                    if (ctx.Expectation == "expect.any(String)")
                    {
                        ctx.Subject.Should().NotBeNullOrEmpty();
                        return;
                    }

                    ctx.Subject.Should().Be(ctx.Expectation);
                })
                .When(info => info.Type == typeof(string))
                .Using<long>(ctx =>
                {
                    if (ctx.Expectation == -999L)
                    {
                        ctx.Subject.Should().BePositive();
                        return;
                    }

                    ctx.Subject.Should().Be(ctx.Expectation);
                })
                .When(info => info.Type == typeof(long))
                .Using<int>(ctx =>
                {
                    if (ctx.Expectation == -999)
                    {
                        ctx.Subject.Should().BePositive();
                        return;
                    }

                    ctx.Subject.Should().Be(ctx.Expectation);
                })
                .When(info => info.Type == typeof(int))
                .WithTracing();
    }
}
