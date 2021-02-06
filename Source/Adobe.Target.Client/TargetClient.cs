/*
 * Copyright 2020 Adobe. All rights reserved.
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
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.Threading.Tasks;
    using Adobe.Target.Client.Model;
    using Adobe.Target.Client.Service;
    using Adobe.Target.Client.Util;
    using Adobe.Target.Delivery.Model;

    /// <summary>
    /// The main TargetClient class
    /// Contains methods for creating and using TargetClient SDK
    /// </summary>
    public sealed class TargetClient : ITargetClient
    {
        private TargetService targetService;
        private DecisioningMethod defaultDecisioningMethod;
        private string defaultPropertyToken;

        /// <summary>
        /// Creates a TargetClient using provided Target configuration
        /// </summary>
        /// <param name="clientConfig">Target Client configuration</param>
        /// <returns>Created <see cref="TargetClient"/> instance</returns>
        public static TargetClient Create(TargetClientConfig clientConfig)
        {
            var targetClient = new TargetClient();
            targetClient.Initialize(clientConfig);

            return targetClient;
        }

        /// <inheritdoc />
        public void Initialize(TargetClientConfig clientConfig)
        {
            this.targetService = new TargetService(clientConfig);
            this.defaultDecisioningMethod = clientConfig.DecisioningMethod;
            this.defaultPropertyToken = clientConfig.DefaultPropertyToken;
            Console.WriteLine("Initialized " + clientConfig.OrganizationId);
        }

        /// <inheritdoc/>
        public TargetDeliveryResponse GetOffers(TargetDeliveryRequest request)
        {
            ValidateGetOffers(request);

            var decisioning = request.DecisioningMethod ?? this.defaultDecisioningMethod;
            this.UpdatePropertyToken(request);

            if (decisioning == DecisioningMethod.OnDevice || decisioning == DecisioningMethod.Hybrid)
            {
                throw new NotImplementedException();
            }

            return this.targetService.ExecuteRequest(request);
        }

        /// <inheritdoc/>
        public Task<TargetDeliveryResponse> GetOffersAsync(TargetDeliveryRequest request)
        {
            ValidateGetOffers(request);

            var decisioning = request.DecisioningMethod ?? this.defaultDecisioningMethod;
            this.UpdatePropertyToken(request);

            if (decisioning == DecisioningMethod.OnDevice || decisioning == DecisioningMethod.Hybrid)
            {
                throw new NotImplementedException();
            }

            return this.targetService.ExecuteRequestAsync(request);
        }

        /// <inheritdoc />
        public TargetDeliveryResponse SendNotifications(TargetDeliveryRequest request)
        {
            ValidateSendNotifications(request);
            return this.targetService.ExecuteRequest(request);
        }

        /// <inheritdoc />
        public Task<TargetDeliveryResponse> SendNotificationsAsync(TargetDeliveryRequest request)
        {
            ValidateSendNotifications(request);
            return this.targetService.ExecuteRequestAsync(request);
        }

        private static void ValidateGetOffers(TargetDeliveryRequest deliveryRequest)
        {
            var request = deliveryRequest?.DeliveryRequest;
            if (request == null)
            {
                throw new ArgumentNullException(nameof(deliveryRequest));
            }

            if (request.Execute != null && request.Execute.PageLoad == null &&
                (request.Execute.Mboxes == null || request.Execute.Mboxes.Count == 0))
            {
                throw new ValidationException(Messages.ExecuteFieldsRequired);
            }

            if (request.Prefetch != null && request.Prefetch.PageLoad == null &&
                (request.Prefetch.Mboxes == null || request.Prefetch.Mboxes.Count == 0) &&
                (request.Prefetch.Views == null || request.Prefetch.Views.Count == 0))
            {
                throw new ValidationException(Messages.PrefetchFieldsRequired);
            }
        }

        private static void ValidateSendNotifications(TargetDeliveryRequest deliveryRequest)
        {
            var request = deliveryRequest?.DeliveryRequest;
            if (request == null)
            {
                throw new ArgumentNullException(nameof(deliveryRequest));
            }

            if (request.Notifications == null || request.Notifications.Count == 0)
            {
                throw new ValidationException(Messages.NotificationsRequired);
            }
        }

        private void UpdatePropertyToken(TargetDeliveryRequest request)
        {
            var property = request.DeliveryRequest.Property;

            if (string.IsNullOrEmpty(this.defaultPropertyToken) || property?.Token != null)
            {
                return;
            }

            if (property == null)
            {
                property = new Property(this.defaultPropertyToken);
                request.DeliveryRequest.Property = property;
                return;
            }

            property.Token = this.defaultPropertyToken;
        }
    }
}
