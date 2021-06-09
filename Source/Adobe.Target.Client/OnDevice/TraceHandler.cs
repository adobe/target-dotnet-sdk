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
    using System.Collections;
    using System.Collections.Generic;
    using Adobe.Target.Client.Model;
    using Adobe.Target.Client.Model.OnDevice;
    using Adobe.Target.Client.Util;
    using Adobe.Target.Delivery.Model;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;

    internal sealed class TraceHandler
    {
        private const string ClientCodeKey = "clientCode";
        private const string ArtifactKey = "artifact";
        private const string ProfileKey = "profile";
        private const string RequestKey = "request";
        private const string ContextKey = "context";
        private const string ActivityIdKey = "id";
        private const string ActivityNameKey = "activityName";
        private const string ActivityTypeKey = "activityType";
        private const string SessionIdKey = "sessionId";
        private const string EnvironmentIdKey = "environmentId";
        private const string MatchedIdsKey = "matchedSegmentIds";
        private const string UnmatchedIdsKey = "unmatchedSegmentIds";
        private const string MatchedRulesKey = "matchedRuleConditions";
        private const string UnmatchedRulesKey = "unmatchedRuleConditions";
        private const string NameKey = "name";
        private const string MboxKey = "mbox";
        private const string ViewKey = "view";
        private const string PageUrlKey = "pageURL";
        private const string HostKey = "host";
        private const string VisitorIdKey = "visitorId";
        private const string TntIdKey = "tntId";
        private const string ProfileLocationKey = "profileLocation";
        private const string MarketingCloudVisitorIdKey = "marketingCloudVisitorId";
        private const string ThirdPartyIdKey = "thirdPartyId";
        private const string CustomerIdsKey = "customerIds";
        private const string ArtifactVersionKey = "artifactVersion";
        private const string PollingIntervalKey = "pollingInterval";
        private const string ArtifactRetrievalCountKey = "artifactRetrievalCount";
        private const string ArtifactLocationKey = "artifactLocation";
        private const string ArtifactLastRetrievedKey = "artifactLastRetrieved";
        private const string CampaignsKey = "campaigns";
        private const string EvaluatedCampaignTargetsKey = "evaluatedCampaignTargets";
        private const string BranchIdKey = "branchId";
        private const string OffersKey = "offers";
        private const string NotificationsKey = "notifications";

        private const string DateTimeFormat = "yyyy-MM-ddTHH:mmZ";
        private const string Execute = "execute";
        private const string Prefetch = "prefetch";
        private const string ActivityId = "activity.id";
        private const string ActivityName = "activity.name";
        private const string ActivityType = "activity.type";
        private const string ExperienceId = "experience.id";
        private const string AudienceIds = "audience.ids";
        private const string OfferId = "offer.id";

        private readonly OnDeviceDecisioningRuleSet ruleSet;
        private readonly IDictionary<string, object> trace;
        private readonly IDictionary<long, IDictionary<string, object>> campaigns;
        private readonly IDictionary<long, IDictionary<string, object>> evaluatedTargets;

        internal TraceHandler(
            TargetClientConfig clientConfig,
            IRuleLoader ruleLoader,
            OnDeviceDecisioningRuleSet ruleSet,
            TargetDeliveryRequest request)
        {
            this.ruleSet = ruleSet;
            this.trace = new Dictionary<string, object>
            {
                { ClientCodeKey, clientConfig.Client },
                { ArtifactKey, this.ArtifactTrace(ruleLoader) },
                { ProfileKey, ProfileTrace(request.DeliveryRequest.Id) },
            };
            this.campaigns = new Dictionary<long, IDictionary<string, object>>();
            this.evaluatedTargets = new Dictionary<long, IDictionary<string, object>>();
        }

        internal Dictionary<string, object> CurrentTrace =>
            new (this.trace)
            {
                { CampaignsKey, this.campaigns.Values },
                { EvaluatedCampaignTargetsKey, this.evaluatedTargets.Values },
            };

        internal void UpdateRequest(TargetDeliveryRequest request, RequestDetailsUnion details, bool execute)
        {
            if (!this.trace.TryGetValue(RequestKey, out var requestTrace))
            {
                requestTrace = CreateRequestTrace(request, details);
                this.trace.Add(RequestKey, requestTrace);
            }

            this.UpdateRequestTrace((Dictionary<string, object>)requestTrace, request, details, execute);
        }

        internal void AddCampaign(OnDeviceDecisioningRule rule, IDictionary<string, object> context, bool matched)
        {
            var meta = rule.Meta;
            if (!meta.TryGetValue(ActivityId, out var activityIdObj))
            {
                return;
            }

            var activityId = (long)activityIdObj;
            if (matched && !this.campaigns.ContainsKey(activityId))
            {
                var campaign = CampaignTrace(rule);
                meta.TryGetValue(ExperienceId, out var experienceId);
                meta.TryGetValue(OfferId, out var offerId);
                campaign.Add(BranchIdKey, experienceId);
                campaign.Add(OffersKey, offerId);
                this.campaigns.Add(activityId, campaign);
            }

            if (!this.evaluatedTargets.TryGetValue(activityId, out var target))
            {
                target = CampaignTargetTrace(rule, context);
                this.evaluatedTargets.Add(activityId, target);
            }

            meta.TryGetValue(AudienceIds, out var audienceIdsObj);
            var audienceIds = ((JArray)audienceIdsObj)?.ToObject<ArrayList>() ?? new ArrayList();

            var condition = SerializationUtils.Serialize(rule.Condition, Formatting.None);

            if (matched)
            {
                ((ArrayList)target[MatchedIdsKey]).AddRange(audienceIds);
                ((ArrayList)target[MatchedRulesKey]).Add(condition);
                return;
            }

            ((ArrayList)target[UnmatchedIdsKey]).AddRange(audienceIds);
            ((ArrayList)target[UnmatchedRulesKey]).Add(condition);
        }

        internal void AddNotification(OnDeviceDecisioningRule rule, Notification notification)
        {
            var meta = rule.Meta;
            if (!meta.TryGetValue(ActivityId, out var activityId))
            {
                return;
            }

            if (!this.campaigns.TryGetValue((long)activityId, out var campaign))
            {
                return;
            }

            var serialized = SerializationUtils.Serialize(notification);
            campaign.Add(NotificationsKey, new List<string> { serialized });
        }

        private static IReadOnlyDictionary<string, object> CreateRequestTrace(TargetDeliveryRequest request, RequestDetailsUnion details)
        {
            return new Dictionary<string, object>
            {
                { SessionIdKey, request.SessionId },
                { EnvironmentIdKey, request.DeliveryRequest.EnvironmentId },
            };
        }

        private static void AddTraceAddress(IDictionary<string, object> requestTrace, RequestDetailsUnion details)
        {
            if (requestTrace.ContainsKey(PageUrlKey))
            {
                return;
            }

            var address = details.GetRequestDetails().Address;
            if (address == null)
            {
                return;
            }

            requestTrace.Add(PageUrlKey, address.Url);
            try
            {
                var url = new Uri(address.Url);
                requestTrace.Add(HostKey, url.Host);
            }
            catch (UriFormatException)
            {
                // ignore
            }
        }

        private static IReadOnlyDictionary<string, object> ProfileTrace(VisitorId visitorId)
        {
            var profile = new Dictionary<string, object>();
            if (string.IsNullOrEmpty(visitorId?.TntId))
            {
                return profile;
            }

            var tntId = visitorId.TntId;
            var visitorIdMap = new Dictionary<string, object>();
            var idx = tntId.LastIndexOf('.');
            if (idx >= 0 && idx < tntId.Length - 1)
            {
                visitorIdMap.Add(ProfileLocationKey, tntId.Substring(idx + 1));
            }

            visitorIdMap.Add(TntIdKey, tntId);

            var mcid = visitorId.MarketingCloudVisitorId;
            if (!string.IsNullOrEmpty(mcid))
            {
                visitorIdMap.Add(MarketingCloudVisitorIdKey, mcid);
            }

            var thirdPartyId = visitorId.ThirdPartyId;
            if (!string.IsNullOrEmpty(thirdPartyId))
            {
                visitorIdMap.Add(ThirdPartyIdKey, thirdPartyId);
            }

            var customerIds = visitorId.CustomerIds;
            if (customerIds is { Count: > 0 })
            {
                visitorIdMap.Add(CustomerIdsKey, SerializationUtils.ConvertObject<Dictionary<string, object>>(customerIds));
            }

            profile.Add(VisitorIdKey, visitorIdMap);

            return profile;
        }

        private static IDictionary<string, object> CampaignTrace(OnDeviceDecisioningRule rule)
        {
            var meta = rule.Meta;
            meta.TryGetValue(ActivityId, out var id);
            meta.TryGetValue(ActivityName, out var name);
            meta.TryGetValue(ActivityType, out var type);

            var campaign = new Dictionary<string, object>
            {
                { ActivityIdKey, id },
                { ActivityNameKey, name },
                { ActivityTypeKey, type },
            };

            return campaign;
        }

        private static IDictionary<string, object> CampaignTargetTrace(OnDeviceDecisioningRule rule, IDictionary<string, object> context)
        {
            var result = CampaignTrace(rule);
            result.Add(ContextKey, context);
            result.Add(MatchedIdsKey, new ArrayList());
            result.Add(UnmatchedIdsKey, new ArrayList());
            result.Add(MatchedRulesKey, new ArrayList());
            result.Add(UnmatchedRulesKey, new ArrayList());

            return result;
        }

        private IReadOnlyDictionary<string, object> ArtifactTrace(IRuleLoader ruleLoader)
        {
            var artifacts = new Dictionary<string, object>(this.ruleSet.Meta)
            {
                { ArtifactVersionKey, this.ruleSet.Version },
                { PollingIntervalKey, ruleLoader.PollingInterval },
                { ArtifactRetrievalCountKey, ruleLoader.FetchCount },
                { ArtifactLocationKey, ruleLoader.ArtifactUrl },
                { ArtifactLastRetrievedKey, ruleLoader.LastFetch.ToString(DateTimeFormat) },
            };

            return artifacts;
        }

        private void UpdateRequestTrace(IDictionary<string, object> requestTrace, TargetDeliveryRequest request, RequestDetailsUnion details, bool execute)
        {
            var type = execute ? Execute : Prefetch;

            if (!requestTrace.TryGetValue(type, out var locationList))
            {
                locationList = new ArrayList();
                requestTrace.Add(type, locationList);
            }

            var location = details.Match<object>(
                pageLoad =>
            {
                var mbox = SerializationUtils.ConvertObject<Dictionary<string, object>>(pageLoad);
                mbox.Add(NameKey, this.ruleSet.GlobalMbox);
                return new Dictionary<string, object> { { MboxKey, mbox } };
            }, mboxRequest =>
            {
                var mbox = SerializationUtils.ConvertObject<Dictionary<string, object>>(mboxRequest);
                return new Dictionary<string, object> { { MboxKey, mbox } };
            }, viewRequest =>
            {
                var view = SerializationUtils.ConvertObject<Dictionary<string, object>>(viewRequest);
                return new Dictionary<string, object> { { ViewKey, view } };
            });

            ((ArrayList)locationList).Add(location);

            AddTraceAddress(requestTrace, details);
        }
    }
}
