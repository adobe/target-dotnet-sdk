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
    using Newtonsoft.Json;

    internal sealed class OnDeviceDecisioningRule
    {
        [JsonConstructor]
        internal OnDeviceDecisioningRule(string ruleKey, string activityId, IReadOnlyList<string> propertyTokens, object condition, IReadOnlyDictionary<string, object> consequence, IReadOnlyDictionary<string, object> meta)
        {
            this.RuleKey = ruleKey;
            this.ActivityId = activityId;
            this.PropertyTokens = propertyTokens;
            this.Condition = condition;
            this.Consequence = consequence;
            this.Meta = meta;
        }

        internal string RuleKey { get; }

        internal string ActivityId { get; }

        internal IReadOnlyList<string> PropertyTokens { get; }

        internal object Condition { get; }

        internal IReadOnlyDictionary<string, object> Consequence { get; }

        internal IReadOnlyDictionary<string, object> Meta { get; }

        public override string ToString() => JsonConvert.SerializeObject(this, Newtonsoft.Json.Formatting.Indented);
    }
}
