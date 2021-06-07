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
namespace Adobe.Target.Client.OnDevice
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Adobe.Target.Client.Model;
    using Adobe.Target.Client.Model.OnDevice;
    using Adobe.Target.Client.Util;
    using Adobe.Target.Delivery.Model;
    using Newtonsoft.Json.Linq;

    internal sealed class DecisioningDetailsExecutor
    {
        internal const string Options = "options";
        private const string Metrics = "metrics";
        private const string Name = "name";

        private readonly DecisioningRuleExecutor ruleExecutor;

        public DecisioningDetailsExecutor(TargetClientConfig clientConfig)
            => this.ruleExecutor = new DecisioningRuleExecutor(clientConfig);

        public void ExecuteDetails(
            TargetDeliveryRequest deliveryRequest,
            IDictionary<string, object> localContext,
            string visitorId,
            ISet<string> responseTokens,
            TraceHandler traceHandler,
            OnDeviceDecisioningRuleSet ruleSet,
            RequestDetailsUnion details,
            PrefetchResponse prefetchResponse,
            ExecuteResponse executeResponse,
            IList<Notification> notifications)
        {
            traceHandler?.UpdateRequest(deliveryRequest, details, executeResponse != null);
            var rules = GetDetailsRules(details, ruleSet);
            if (rules == null)
            {
                UnhandledResponse(details, prefetchResponse, executeResponse, traceHandler);
                return;
            }

            var propertyToken = deliveryRequest.DeliveryRequest.Property?.Token;
            var handledAtLeastOnce = false;
            var skipKeySet = new HashSet<string>();

            foreach (var rule in rules)
            {
                if (PropertyTokenMismatch(rule.PropertyTokens, propertyToken))
                {
                    continue;
                }

                if (rule.RuleKey != null && skipKeySet.Contains(rule.RuleKey))
                {
                    continue;
                }

                var consequence = this.ruleExecutor.ExecuteRule(localContext, details, visitorId, rule, responseTokens, traceHandler);
                if (!HandleResult(consequence, rule, details, prefetchResponse, executeResponse, notifications, traceHandler, ruleSet.GlobalMbox))
                {
                    continue;
                }

                handledAtLeastOnce = true;
                if (rule.RuleKey != null)
                {
                    skipKeySet.Add(rule.RuleKey);
                }

                if (details.Match(
                    _ => false,
                    _ => true,
                    _ => false))
                {
                    break;
                }
            }

            if (!handledAtLeastOnce)
            {
                UnhandledResponse(details, prefetchResponse, executeResponse, traceHandler);
            }
        }

        private static bool HandleResult(
            IDictionary<string, object> consequence,
            OnDeviceDecisioningRule rule,
            RequestDetailsUnion details,
            PrefetchResponse prefetchResponse,
            ExecuteResponse executeResponse,
            IList<Notification> notifications,
            TraceHandler traceHandler,
            string globalMbox = "target-global-mbox")
        {
            if (consequence == null || consequence.Count == 0)
            {
                return false;
            }

            consequence.TryGetValue(Name, out var nameObject);
            var consequenceName = (string)nameObject;

            consequence.TryGetValue(Options, out var optionsObject);
            var consequenceOptions = (List<Option>)optionsObject;

            consequence.TryGetValue(Metrics, out var metricsObject);
            var consequenceMetrics = GetMetrics(metricsObject);

            if (executeResponse != null)
            {
                var notification = CreateNotification(details, consequenceOptions, globalMbox);
                traceHandler?.AddNotification(rule, notification);
                notifications.Add(notification);
            }

            return details.Match(
                _ => HandlePageLoad(prefetchResponse, executeResponse, consequenceOptions, consequenceMetrics, traceHandler),
                mboxRequest => HandleMboxRequest(prefetchResponse, executeResponse, mboxRequest, consequenceOptions, consequenceMetrics, traceHandler),
                _ => HandleViewRequest(prefetchResponse, consequenceName, consequenceOptions, consequenceMetrics, traceHandler));
        }

        private static bool HandleViewRequest(
            PrefetchResponse prefetchResponse,
            string consequenceName,
            List<Option> consequenceOptions,
            List<Metric> consequenceMetrics,
            TraceHandler traceHandler)
        {
            if (prefetchResponse == null)
            {
                return false;
            }

            var responseView = new View(consequenceName, null, consequenceOptions, consequenceMetrics);
            responseView.Trace = traceHandler?.CurrentTrace;

            var views = prefetchResponse.Views;
            if (views == null)
            {
                views = new List<View>();
                prefetchResponse.Views = views;
            }

            var foundView = views.Find(view => view.Name == responseView.Name);
            if (foundView == null)
            {
                views.Add(responseView);
                return true;
            }

            if (responseView.Options != null)
            {
                foundView.Options ??= new List<Option>();
                foundView.Options.AddRange(responseView.Options);
            }

            if (responseView.Metrics != null)
            {
                foundView.Metrics ??= new List<Metric>();
                foundView.Metrics.AddRange(responseView.Metrics);
            }

            return true;
        }

        private static bool HandleMboxRequest(
            PrefetchResponse prefetchResponse,
            ExecuteResponse executeResponse,
            MboxRequest mboxRequest,
            List<Option> consequenceOptions,
            List<Metric> consequenceMetrics,
            TraceHandler traceHandler)
        {
            if (prefetchResponse != null)
            {
                var prefetchMboxResponse =
                    new PrefetchMboxResponse(mboxRequest.Index, mboxRequest.Name, consequenceOptions, consequenceMetrics);
                prefetchResponse.Mboxes ??= new List<PrefetchMboxResponse>();
                prefetchResponse.Mboxes.Add(prefetchMboxResponse);
                return true;
            }

            if (executeResponse == null)
            {
                return false;
            }

            var mboxResponse =
                new MboxResponse(mboxRequest.Index, mboxRequest.Name, metrics: consequenceMetrics)
                {
                    Options = consequenceOptions?.Select(
                            option =>
                            {
                                option.EventToken = null;
                                return option;
                            })
                        .Where(option => option.Type != null || option.Content != null)
                        .ToList(),
                    Trace = traceHandler?.CurrentTrace,
                };
            executeResponse.Mboxes ??= new List<MboxResponse>();
            executeResponse.Mboxes.Add(mboxResponse);
            return true;
        }

        private static bool HandlePageLoad(
            PrefetchResponse prefetchResponse,
            ExecuteResponse executeResponse,
            IList<Option> consequenceOptions,
            IList<Metric> consequenceMetrics,
            TraceHandler traceHandler)
        {
            PageLoadResponse pageLoad = null;
            if (prefetchResponse != null)
            {
                prefetchResponse.PageLoad ??= new PageLoadResponse();
                pageLoad = prefetchResponse.PageLoad;
            }
            else if (executeResponse != null)
            {
                executeResponse.PageLoad ??= new PageLoadResponse();
                pageLoad = executeResponse.PageLoad;
            }

            if (pageLoad == null)
            {
                return false;
            }

            pageLoad.Trace = traceHandler?.CurrentTrace;

            if (consequenceOptions != null)
            {
                foreach (var option in consequenceOptions)
                {
                    if (executeResponse != null)
                    {
                        option.EventToken = null;
                    }

                    if (option.Type == null && option.Content == null && option.EventToken == null)
                    {
                        continue;
                    }

                    pageLoad.Options ??= new List<Option>();
                    pageLoad.Options.Add(option);
                }
            }

            if (consequenceMetrics == null)
            {
                return true;
            }

            foreach (var metric in consequenceMetrics
                .Where(metric => pageLoad.Metrics == null || !pageLoad.Metrics.Contains(metric)))
            {
                pageLoad.Metrics ??= new List<Metric>();
                pageLoad.Metrics.Add(metric);
            }

            return true;
        }

        private static Notification CreateNotification(RequestDetailsUnion details, List<Option> options, string globalMbox)
        {
            var id = Guid.NewGuid().ToString();
            var impressionId = Guid.NewGuid().ToString();
            var dateTime = TimeProvider.Current.UtcNow;
            var timestamp = new DateTimeOffset(dateTime).ToUnixTimeMilliseconds();
            var tokens = options == null ? new List<string>() : options.Select(option => option.EventToken).ToList();
            var notification = new Notification(id: id, type: MetricType.Display, impressionId: impressionId, timestamp: timestamp, tokens: tokens);
            return details.Match(
                _ =>
                {
                    notification.Mbox = new NotificationMbox(globalMbox);
                    return notification;
                }, mboxRequest =>
                {
                    notification.Mbox = new NotificationMbox(mboxRequest.Name);
                    return notification;
                }, viewRequest =>
                {
                    notification.View = new NotificationView(viewRequest.Name, viewRequest.Key);
                    return notification;
                });
        }

        private static List<Metric> GetMetrics(object metricsObject)
        {
            return metricsObject is not JArray metricsArray || metricsArray.Count == 0 ? null : metricsArray.ToObject<List<Metric>>();
        }

        private static void UnhandledResponse(
            RequestDetailsUnion details,
            PrefetchResponse prefetchResponse,
            ExecuteResponse executeResponse,
            TraceHandler traceHandler)
        {
            _ = details.Match<object>(
                _ => UnhandledPageLoadResponse(prefetchResponse, executeResponse, traceHandler),
                mboxRequest => UnhandledMboxResponse(prefetchResponse, executeResponse, mboxRequest, traceHandler),
                _ => UnhandledViewResponse(prefetchResponse, traceHandler));
        }

        private static object UnhandledViewResponse(PrefetchResponse prefetchResponse, TraceHandler traceHandler)
        {
            var view = new View { Trace = traceHandler?.CurrentTrace };
            prefetchResponse.Views ??= new List<View>();
            prefetchResponse.Views.Add(view);
            return null;
        }

        private static object UnhandledMboxResponse(
            PrefetchResponse prefetchResponse,
            ExecuteResponse executeResponse,
            MboxRequest mboxRequest,
            TraceHandler traceHandler)
        {
            if (prefetchResponse != null)
            {
                var prefetchMboxResponse = new PrefetchMboxResponse(mboxRequest.Index, mboxRequest.Name)
                {
                    Trace = traceHandler?.CurrentTrace,
                };
                prefetchResponse.Mboxes ??= new List<PrefetchMboxResponse>();
                prefetchResponse.Mboxes.Add(prefetchMboxResponse);
                return null;
            }

            var response = new MboxResponse(mboxRequest.Index, mboxRequest.Name)
            {
                Trace = traceHandler?.CurrentTrace,
            };
            executeResponse.Mboxes ??= new List<MboxResponse>();
            executeResponse.Mboxes.Add(response);
            return null;
        }

        private static object UnhandledPageLoadResponse(
            PrefetchResponse prefetchResponse,
            ExecuteResponse executeResponse,
            TraceHandler traceHandler)
        {
            var response = new PageLoadResponse { Trace = traceHandler?.CurrentTrace };
            if (prefetchResponse != null)
            {
                prefetchResponse.PageLoad = response;
                return null;
            }

            executeResponse.PageLoad = response;
            return null;
        }

        private static bool PropertyTokenMismatch(IReadOnlyList<string> rulePropertyTokens, string propertyToken)
        {
            if (rulePropertyTokens == null || rulePropertyTokens.Count == 0)
            {
                return false;
            }

            if (string.IsNullOrEmpty(propertyToken))
            {
                return true;
            }

            return !rulePropertyTokens.Contains(propertyToken);
        }

        private static IReadOnlyList<OnDeviceDecisioningRule> GetDetailsRules(RequestDetailsUnion details, OnDeviceDecisioningRuleSet ruleSet)
        {
            return details.Match(
                _ => GetPageLoadRules(ruleSet),
                mboxRequest => GetMboxRules(ruleSet, mboxRequest),
                viewRequest => GetViewRules(ruleSet, viewRequest));
        }

        private static IReadOnlyList<OnDeviceDecisioningRule> GetViewRules(OnDeviceDecisioningRuleSet ruleSet, ViewRequest viewRequest)
        {
            if (string.IsNullOrEmpty(viewRequest.Name))
            {
                return ruleSet.Rules.Views.Values.SelectMany(list => list).ToList();
            }

            return ruleSet.Rules.Views.TryGetValue(viewRequest.Name, out var result) ? result : null;
        }

        private static IReadOnlyList<OnDeviceDecisioningRule> GetMboxRules(OnDeviceDecisioningRuleSet ruleSet, MboxRequest mboxRequest)
        {
            return ruleSet.Rules.Mboxes.TryGetValue(mboxRequest.Name, out var result) ? result : null;
        }

        private static IReadOnlyList<OnDeviceDecisioningRule> GetPageLoadRules(OnDeviceDecisioningRuleSet ruleSet)
        {
            return ruleSet.Rules.Mboxes.TryGetValue(ruleSet.GlobalMbox, out var result) ? result : null;
        }
    }
}
