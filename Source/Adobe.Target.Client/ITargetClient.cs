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
namespace Adobe.Target.Client
{
    using System.Threading.Tasks;
    using Adobe.Target.Client.Model;

    /// <summary>
    /// Main ITargetClient interface
    /// </summary>
    public interface ITargetClient
    {
        /// <summary>
        /// Initializes an ITargetClient using provided Target configuration
        /// Call this after an ITargetClient has been injected via DI
        /// </summary>
        /// <param name="clientConfig">Target Client configuration</param>
        void Initialize(TargetClientConfig clientConfig);

        /// <summary>
        /// Get Offers
        /// </summary>
        /// <param name="request">Target Delivery Request</param>
        /// <returns>Target Delivery Response</returns>
        TargetDeliveryResponse GetOffers(TargetDeliveryRequest request);

        /// <summary>
        /// Get Offers Async
        /// </summary>
        /// <param name="request">Target Delivery Request</param>
        /// <returns>Target Delivery Response Task</returns>
        Task<TargetDeliveryResponse> GetOffersAsync(TargetDeliveryRequest request);

        /// <summary>
        /// Send Notifications
        /// </summary>
        /// <param name="request">Target Delivery Request</param>
        /// <returns>Target Delivery Response</returns>
        TargetDeliveryResponse SendNotifications(TargetDeliveryRequest request);

        /// <summary>
        /// Get Offers Async
        /// </summary>
        /// <param name="request">Target Delivery Request</param>
        /// <returns>Target Delivery Response Task</returns>
        Task<TargetDeliveryResponse> SendNotificationsAsync(TargetDeliveryRequest request);

        /// <summary>
        /// Makes a getOffers() call and converts all returned JSON offers into Attributes
        /// </summary>
        /// <param name="request">optional TargetDeliveryRequest used to set specific request fields. If null, one will be generated</param>
        /// <param name="mboxes">Mbox names for attribute retrieval</param>
        /// <returns>Requested Target attributes</returns>
        TargetAttributes GetAttributes(TargetDeliveryRequest request, params string[] mboxes);

        /// <summary>
        /// Makes a getOffers() call and converts all returned JSON offers into Attributes
        /// </summary>
        /// <param name="request">optional TargetDeliveryRequest used to set specific request fields. If null, one will be generated</param>
        /// <param name="mboxes">Mbox names for attribute retrieval</param>
        /// <returns>Task returning requested Target attributes</returns>
        Task<TargetAttributes> GetAttributesAsync(TargetDeliveryRequest request, params string[] mboxes);
    }
}
