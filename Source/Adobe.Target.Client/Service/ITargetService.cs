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
namespace Adobe.Target.Client.Service
{
    using System.Threading.Tasks;
    using Adobe.Target.Client.Model;

    /// <summary>
    /// Target Service
    /// </summary>
    public interface ITargetService
    {
        /// <summary>
        /// Execute sync Delivery request
        /// </summary>
        /// <param name="request">Target Delivery request</param>
        /// <returns>Target Delivery Response</returns>
        TargetDeliveryResponse ExecuteRequest(TargetDeliveryRequest request);

        /// <summary>
        /// Execute async Delivery request
        /// </summary>
        /// <param name="request">Target Delivery request</param>
        /// <returns>Target Delivery Response</returns>
        Task<TargetDeliveryResponse> ExecuteRequestAsync(TargetDeliveryRequest request);
    }
}
