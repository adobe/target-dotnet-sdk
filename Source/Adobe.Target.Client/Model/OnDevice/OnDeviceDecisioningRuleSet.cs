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
namespace Adobe.Target.Client.Model.OnDevice
{
    using System.Collections.Generic;
    using Adobe.Target.Client.Util;
    using Newtonsoft.Json;

    internal sealed class OnDeviceDecisioningRuleSet
    {
        [JsonConstructor]
        internal OnDeviceDecisioningRuleSet(string version, string globalMbox, bool geoTargetingEnabled, IReadOnlyList<string> remoteMboxes, IReadOnlyList<string> remoteViews, IReadOnlyList<string> localMboxes, IReadOnlyList<string> localViews, IReadOnlyList<string> responseTokens, OnDeviceDecisioningRules rules, IDictionary<string, object> meta)
        {
            this.Version = version;
            this.GlobalMbox = globalMbox;
            this.GeoTargetingEnabled = geoTargetingEnabled;
            this.RemoteMboxes = remoteMboxes;
            this.RemoteViews = remoteViews;
            this.LocalMboxes = localMboxes;
            this.LocalViews = localViews;
            this.ResponseTokens = responseTokens;
            this.Rules = rules;
            this.Meta = meta;
        }

        internal string Version { get; }

        internal string GlobalMbox { get; }

        internal bool GeoTargetingEnabled { get; }

        internal IReadOnlyList<string> RemoteMboxes { get; }

        internal IReadOnlyList<string> RemoteViews { get; }

        internal IReadOnlyList<string> LocalMboxes { get; }

        internal IReadOnlyList<string> LocalViews { get; }

        internal IReadOnlyList<string> ResponseTokens { get; }

        internal OnDeviceDecisioningRules Rules { get; }

        internal IDictionary<string, object> Meta { get; }

        public override string ToString() => SerializationUtils.Serialize(this);
    }
}
