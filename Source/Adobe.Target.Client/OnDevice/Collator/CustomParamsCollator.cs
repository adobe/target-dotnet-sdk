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
namespace Adobe.Target.Client.OnDevice.Collator
{
    using System.Collections.Generic;
    using Adobe.Target.Client.Model;
    using Adobe.Target.Delivery.Model;

    internal sealed class CustomParamsCollator : IParamsCollator
    {
        private const string LowerCasePostfix = "_lc";

        public Dictionary<string, object> CollateParams(TargetDeliveryRequest deliveryRequest = default, RequestDetails requestDetails = default)
        {
            var result = new Dictionary<string, object>();
            if (requestDetails?.Parameters == null || requestDetails.Parameters.Count == 0)
            {
                return result;
            }

            foreach (var param in requestDetails.Parameters)
            {
                var paramStringValue = (string)param.Value;
                result.Add(param.Key, paramStringValue);
                result.Add(param.Key + LowerCasePostfix, paramStringValue?.ToLowerInvariant());
            }

            return this.CreateNestedParametersFromDots(result);
        }

        public Dictionary<string, object> CreateNestedParametersFromDots(Dictionary<string, object> custom)
        {
            var result = new Dictionary<string, object>();
            foreach (KeyValuePair<string, object> entry in custom)
            {
                if (entry.Key.Contains(".")
                    && !entry.Key.Contains("..")
                    && entry.Key[0] != '.'
                    && entry.Key[entry.Key.Length - 1] != '.')
                {
                    this.AddNestedKeyToParameters(result, entry.Key, entry.Value);
                }

                result.Add(entry.Key, entry.Value);
            }

            return result;
        }

        public void AddNestedKeyToParameters(Dictionary<string, object> custom, string key, object value)
        {
            string[] keys = key.Split('.');
            for (int i = 0; i < keys.Length - 1; i++)
            {
                if (!custom.ContainsKey(keys[i]))
                {
                    custom.Add(keys[i], new Dictionary<string, object>());
                }

                custom = (Dictionary<string, object>)custom[keys[i]];
            }

            custom.Add(keys[keys.Length - 1], value);
        }
    }
}
