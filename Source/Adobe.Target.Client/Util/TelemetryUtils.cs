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
    using System;
    using System.Collections.Generic;
    using Adobe.Target.Client.Model;
    using Adobe.Target.Delivery.Model;

    internal static class TelemetryUtils
    {
        internal static void AddTelemetry(TargetDeliveryRequest request, TargetClientConfig config, int execution = 0)
        {
            var telemetryEntry = GetTelemetryEntry(request, config, execution);
            if (telemetryEntry == null)
            {
                return;
            }

            var deliveryRequest = request.DeliveryRequest;
            deliveryRequest.Telemetry ??= new Delivery.Model.Telemetry(new List<TelemetryEntry>());
            deliveryRequest.Telemetry.Entries.Add(telemetryEntry);
        }

        internal static TelemetryEntry GetTelemetryEntry(TargetDeliveryRequest request, TargetClientConfig config, int execution = 0)
        {
            if (!config.TelemetryEnabled)
            {
                return null;
            }

            var decisioningMethod = request.DecisioningMethod ?? config.DecisioningMethod;

            return new TelemetryEntry(
                request.DeliveryRequest.RequestId,
                DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
                execution,
                new TelemetryFeatures(decisioningMethod));
        }
    }
}
