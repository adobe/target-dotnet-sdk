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
    using System.Collections.Generic;
    using System.Linq;
    using Adobe.Target.Client.Extension;
    using Adobe.Target.Client.Model.OnDevice;
    using Adobe.Target.Client.OnDevice.Collator;
    using Adobe.Target.Client.Service;
    using Adobe.Target.Client.Util;
    using Adobe.Target.Delivery.Model;
    using JsonLogic.Net;
    using Newtonsoft.Json.Linq;

    internal sealed class DecisioningRuleExecutor
    {
        private const string Allocation = "allocation";
        private const string State = "state";
        private const string ResponseTokenDecisioningMethod = "activity.decisioningMethod";
        private const string OnDevice = "on-device";

        private readonly TargetClientConfig clientConfig;
        private readonly JsonLogicEvaluator evaluator;

        public DecisioningRuleExecutor(TargetClientConfig clientConfig)
        {
            this.clientConfig = clientConfig;
            this.evaluator = new JsonLogicEvaluator(EvaluateOperators.Default);
        }

        public IDictionary<string, object> ExecuteRule(
            IDictionary<string, object> localContext,
            RequestDetailsUnion details,
            string visitorId,
            OnDeviceDecisioningRule rule,
            ISet<string> responseTokens)
        {
            localContext.Add(Allocation, this.ComputeAllocation(visitorId, rule));
            var matched = this.EvaluateRule((JObject)rule.Condition, localContext.ToExpandoObject());
            if (!matched)
            {
                return null;
            }

            return this.ReplaceCampaignMacros(rule, this.GetConsequence(responseTokens, rule, localContext), details);
        }

        private static IDictionary<string, object> GetGeoResponseTokens(IDictionary<string, object> localContext, ISet<string> responseTokenKeys)
        {
            var result = new Dictionary<string, object>();
            if (!localContext.TryGetValue(OnDeviceDecisioningService.ContextKeyGeo, out var geoContext))
            {
                return result;
            }

            foreach (var geo in (Dictionary<string, object>)geoContext)
            {
                var tokenKey = geo.Key == GeoParamsCollator.GeoRegion
                    ? $"{OnDeviceDecisioningService.ContextKeyGeo}.{State}"
                    : $"{OnDeviceDecisioningService.ContextKeyGeo}.{geo.Key}";

                if (responseTokenKeys.Contains(tokenKey))
                {
                    result.Add(tokenKey, geo.Value);
                }
            }

            return result;
        }

        private static IDictionary<string, object> GetMetaResponseTokens(OnDeviceDecisioningRule rule, ISet<string> responseTokenKeys)
        {
            var result = new Dictionary<string, object>();
            if (rule.Meta == null)
            {
                return result;
            }

            foreach (var meta in rule.Meta)
            {
                if (responseTokenKeys.Contains(meta.Key))
                {
                    result.Add(meta.Key, meta.Value);
                }
            }

            return result;
        }

        private IDictionary<string, object> ReplaceCampaignMacros(OnDeviceDecisioningRule rule, IDictionary<string, object> consequence, RequestDetailsUnion details)
        {
            if (consequence == null || !consequence.ContainsKey(DecisioningDetailsExecutor.Options))
            {
                return consequence;
            }

            var campaignMacroReplacer = new CampaignMacroReplacer(rule, consequence, details);

            consequence[DecisioningDetailsExecutor.Options] = campaignMacroReplacer.GetOptions();
            return consequence;
        }

        private IDictionary<string, object> GetConsequence(ISet<string> responseTokenKeys, OnDeviceDecisioningRule rule, IDictionary<string, object> localContext)
        {
            if (rule.Consequence == null)
            {
                return null;
            }

            var consequence = rule.Consequence
                .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

            if (responseTokenKeys.Count == 0 || !consequence.ContainsKey(DecisioningDetailsExecutor.Options))
            {
                return consequence;
            }

            var options = (JArray)consequence[DecisioningDetailsExecutor.Options];
            if (options == null || !options.HasValues)
            {
                return consequence;
            }

            var optionsList = options.ToObject<IList<Option>>();
            if (optionsList == null)
            {
                return consequence;
            }

            var option = optionsList[0]; // TODO: why not all of them as in Node.js?
            var responseTokens = option.ResponseTokens;
            responseTokens.Add(ResponseTokenDecisioningMethod, OnDevice);
            responseTokens.AddAll(GetGeoResponseTokens(localContext, responseTokenKeys));
            responseTokens.AddAll(GetMetaResponseTokens(rule, responseTokenKeys));
            consequence[DecisioningDetailsExecutor.Options] = optionsList;

            return consequence;
        }

        private double ComputeAllocation(string visitorId, OnDeviceDecisioningRule rule, string salt = "0")
        {
            return AllocationUtils.CalculateAllocation(this.clientConfig.Client, rule.ActivityId, visitorId, salt);
        }

        private bool EvaluateRule(JToken rule, object data)
        {
            var evaluationResult = this.evaluator.Apply(rule, data);
            return evaluationResult.IsTruthy();
        }
    }
}
