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

    /// <summary>
    /// Common Params Collator interface
    /// </summary>
    internal interface IParamsCollator
    {
        /// <summary>
        /// Collate params
        /// </summary>
        /// <param name="deliveryRequest">Delivery request</param>
        /// <param name="requestDetails">Request details</param>
        /// <returns>Collated params dictionary</returns>
        Dictionary<string, object> CollateParams(
            TargetDeliveryRequest deliveryRequest = default,
            RequestDetails requestDetails = default);
    }
}
