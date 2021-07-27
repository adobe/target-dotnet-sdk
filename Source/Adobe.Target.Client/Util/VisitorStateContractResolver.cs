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
namespace Adobe.Target.Client.Util
{
    using Newtonsoft.Json.Serialization;

    internal sealed class VisitorStateContractResolver : DefaultContractResolver
    {
        private const string Sdid = "SupplementalDataId";
        private const string SdidReplace = "supplementalDataID";
        private const string OrgPostfix = "@AdobeOrg";

        protected override string ResolvePropertyName(string name)
        {
            if (string.IsNullOrEmpty(name) || name.EndsWith(OrgPostfix))
            {
                return name;
            }

            if (name.StartsWith(Sdid))
            {
                return name.Replace(Sdid, SdidReplace);
            }

            return char.ToLowerInvariant(name[0]) + name.Substring(1);
        }
    }
}
