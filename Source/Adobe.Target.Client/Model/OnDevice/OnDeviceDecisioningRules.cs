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

    internal sealed class OnDeviceDecisioningRules
    {
        internal IReadOnlyDictionary<string, IReadOnlyList<OnDeviceDecisioningRule>> Mboxes { get; set; }

        internal IReadOnlyDictionary<string, IReadOnlyList<OnDeviceDecisioningRule>> Views { get; set; }

        public override string ToString() => JsonConvert.SerializeObject(this, Newtonsoft.Json.Formatting.Indented);
    }
}