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
    using System.Text;
    using Adobe.Target.Client.Model;
    using Adobe.Target.Client.Model.OnDevice;
    using Adobe.Target.Client.Util;

    internal sealed class DecisioningEvaluator
    {
        private const string RemoteActivities = "Remote activities in: ";
        private const string Mboxes = "mboxes ";
        private const string Views = "views ";

        private readonly RuleLoader ruleLoader;

        public DecisioningEvaluator(RuleLoader ruleLoader) => this.ruleLoader = ruleLoader;

        internal OnDeviceDecisioningEvaluation EvaluateLocalExecution(TargetDeliveryRequest request)
        {
            if (request == null)
            {
                return new OnDeviceDecisioningEvaluation(false, Messages.RequestNonNull);
            }

            var ruleSet = this.ruleLoader.GetLatestRules();
            if (ruleSet == null)
            {
                return new OnDeviceDecisioningEvaluation(false, Messages.RulesetUnavailable);
            }

            var remoteMboxes = this.ComputeRemoteMboxes(request, ruleSet);
            var remoteViews = this.ComputeRemoteViews(request, ruleSet);
            var hasRemoteMboxes = remoteMboxes.Count != 0;
            var hasRemoteViews = remoteViews.Count != 0;

            if (!hasRemoteMboxes && !hasRemoteViews)
            {
                return new OnDeviceDecisioningEvaluation(true, globalMbox: ruleSet.GlobalMbox);
            }

            var reason = new StringBuilder(RemoteActivities);
            if (hasRemoteMboxes)
            {
                reason.Append(Mboxes).Append(string.Join(",", remoteMboxes)).Append(' ');
            }

            if (hasRemoteViews)
            {
                reason.Append(Views).Append(string.Join(",", remoteViews));
            }

            return new OnDeviceDecisioningEvaluation(
                false,
                reason.ToString(),
                ruleSet.GlobalMbox,
                hasRemoteMboxes ? remoteMboxes : default,
                hasRemoteViews ? remoteViews : default);
        }

        private IReadOnlyList<string> ComputeRemoteMboxes(TargetDeliveryRequest request, OnDeviceDecisioningRuleSet ruleSet)
        {
            var result = new HashSet<string>();

            var requestMboxNames = this.GetAllMboxNames(request, ruleSet);
            if (requestMboxNames.Count == 0)
            {
                return result.ToList();
            }

            foreach (var requestMboxName in requestMboxNames)
            {
                if (!ruleSet.LocalMboxes.Contains(requestMboxName) || ruleSet.RemoteMboxes.Contains(requestMboxName))
                {
                    result.Add(requestMboxName);
                }
            }

            return result.ToList();
        }

        private IReadOnlyList<string> ComputeRemoteViews(TargetDeliveryRequest request, OnDeviceDecisioningRuleSet ruleSet)
        {
            var result = new HashSet<string>();

            var requestViewNames = this.GetAllViewNames(request);
            if (requestViewNames.Count == 0)
            {
                return result.ToList();
            }

            if (requestViewNames.Count == 1 && requestViewNames[0] == null)
            {
                return ruleSet.RemoteViews;
            }

            foreach (var requestViewName in requestViewNames)
            {
                if (!ruleSet.LocalViews.Contains(requestViewName) || ruleSet.RemoteViews.Contains(requestViewName))
                {
                    result.Add(requestViewName);
                }
            }

            return result.ToList();
        }

        private IList<string> GetAllMboxNames(TargetDeliveryRequest request, OnDeviceDecisioningRuleSet ruleSet)
        {
            var result = new List<string>();

            if (request == null || ruleSet == null)
            {
                return result;
            }

            var prefetch = request.DeliveryRequest?.Prefetch;
            var execute = request.DeliveryRequest?.Execute;

            if (prefetch?.PageLoad != null || execute?.PageLoad != null)
            {
                result.Add(ruleSet.GlobalMbox);
            }

            result.AddRange(prefetch?.Mboxes?.Select(mbox => mbox.Name) ?? Array.Empty<string>());
            result.AddRange(execute?.Mboxes?.Select(mbox => mbox.Name) ?? Array.Empty<string>());

            return result;
        }

        private IList<string> GetAllViewNames(TargetDeliveryRequest request)
        {
            var result = new List<string>();

            if (request == null)
            {
                return result;
            }

            var prefetch = request.DeliveryRequest?.Prefetch;
            result.AddRange(prefetch?.Views?.Select(view => view.Name) ?? Array.Empty<string>());

            return result;
        }
    }
}
