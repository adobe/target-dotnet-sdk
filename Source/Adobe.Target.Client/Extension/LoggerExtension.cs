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

// ReSharper disable TemplateIsNotCompileTimeConstantProblem
namespace Adobe.Target.Client.Extension
{
    using System;
    using Adobe.Target.Client.Model;
    using Adobe.Target.Client.Util;
    using Adobe.Target.Delivery.Model;
    using Microsoft.Extensions.Logging;

    internal static class LoggerExtension
    {
        internal static void LogRequest(this ILogger logger, TargetDeliveryRequest request)
        {
            logger?.LogDebug(Messages.LogTargetServiceRequest, request.SessionId, request.DeliveryRequest.ToJson());
        }

        internal static void LogResponse(this ILogger logger, DeliveryResponse response)
        {
            logger?.LogDebug(Messages.LogTargetServiceResponse, response.ToJson());
        }

        internal static void LogException(this ILogger logger, Exception exception)
        {
            logger?.LogError(exception, exception.Message);
        }
    }
}
