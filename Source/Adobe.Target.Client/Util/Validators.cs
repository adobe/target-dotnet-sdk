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
    using System.ComponentModel.DataAnnotations;
    using Adobe.Target.Client.Model;
    using Adobe.Target.Client.Service;

    internal static class Validators
    {
        internal static void ValidateClientInit(TargetService service)
        {
            if (service == null)
            {
                throw new ApplicationException(Messages.ClientNotInitialized);
            }
        }

        internal static void ValidateGetOffers(TargetDeliveryRequest deliveryRequest)
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

        internal static void ValidateSendNotifications(TargetDeliveryRequest deliveryRequest)
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
    }
}
