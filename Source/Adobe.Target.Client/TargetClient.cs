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
    using Adobe.Target.Client.Service;
    using Adobe.Target.Client.Util;
    using Adobe.Target.Delivery.Model;
    using Microsoft.Extensions.Logging;

    /// <summary>
    /// The main TargetClient class
    /// Contains methods for creating and using TargetClient SDK
    /// </summary>
    public sealed class TargetClient : ITargetClient
    {
        private TargetService targetService;
        private OnDeviceDecisioningService localService;
        private DecisioningMethod defaultDecisioningMethod;
        private string defaultPropertyToken;

        internal static ILogger Logger { get; private set; }

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
            Logger = clientConfig.Logger;
            VisitorProvider.Initialize(clientConfig.OrganizationId);
            this.targetService = new TargetService(clientConfig);
            this.localService = new OnDeviceDecisioningService(clientConfig, this.targetService);
            this.defaultDecisioningMethod = clientConfig.DecisioningMethod;
            this.defaultPropertyToken = clientConfig.DefaultPropertyToken;
            Logger?.LogDebug("Initialized Target Client: " + clientConfig.OrganizationId);
        }

        /// <inheritdoc/>
        public TargetDeliveryResponse GetOffers(TargetDeliveryRequest request)
        {
            Validators.ValidateClientInit(this.targetService);
            Validators.ValidateGetOffers(request);

            var decisioning = request.DecisioningMethod != default(DecisioningMethod) ? request.DecisioningMethod : this.defaultDecisioningMethod;
            this.UpdatePropertyToken(request);

            if (decisioning == DecisioningMethod.OnDevice
                || (decisioning == DecisioningMethod.Hybrid && this.localService.EvaluateLocalExecution(request).AllLocal))
            {
                return this.localService.ExecuteRequest(request);
            }

            return this.targetService.ExecuteRequest(request);
        }

        /// <inheritdoc/>
        public Task<TargetDeliveryResponse> GetOffersAsync(TargetDeliveryRequest request)
        {
            Validators.ValidateClientInit(this.targetService);
            Validators.ValidateGetOffers(request);

            var decisioning = request.DecisioningMethod != default(DecisioningMethod) ? request.DecisioningMethod : this.defaultDecisioningMethod;
            this.UpdatePropertyToken(request);

            if (decisioning == DecisioningMethod.OnDevice
                || (decisioning == DecisioningMethod.Hybrid && this.localService.EvaluateLocalExecution(request).AllLocal))
            {
                return this.localService.ExecuteRequestAsync(request);
            }

            return this.targetService.ExecuteRequestAsync(request);
        }

        /// <inheritdoc />
        public TargetDeliveryResponse SendNotifications(TargetDeliveryRequest request)
        {
            Validators.ValidateClientInit(this.targetService);
            Validators.ValidateSendNotifications(request);
            return this.targetService.ExecuteRequest(request);
        }

        /// <inheritdoc />
        public Task<TargetDeliveryResponse> SendNotificationsAsync(TargetDeliveryRequest request)
        {
            Validators.ValidateClientInit(this.targetService);
            Validators.ValidateSendNotifications(request);
            return this.targetService.ExecuteRequestAsync(request);
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
