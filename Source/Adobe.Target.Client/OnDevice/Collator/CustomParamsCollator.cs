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
                result.Add(param.Key, param.Value);
                result.Add(param.Key + LowerCasePostfix, param.Value?.ToLowerInvariant());
            }

            return result;
        }
    }
}
