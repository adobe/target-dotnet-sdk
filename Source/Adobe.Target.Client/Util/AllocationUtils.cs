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

    internal static class AllocationUtils
    {
        private const int TotalBuckets = 10_000;
        private const int MaxPercentage = 100;

        public static double CalculateAllocation(string clientId, string activityId, string visitorId, string salt)
        {
            return CalculateAllocation(GetDeviceId(clientId, activityId, visitorId, salt));
        }

        private static string GetDeviceId(string clientId, string activityId, string visitorId, string salt)
        {
            var index = visitorId.IndexOf('.');
            if (index > 0)
            {
                visitorId = visitorId.Substring(0, index);
            }

            return $"{clientId}.{activityId}.{visitorId}.{salt}";
        }

        private static double CalculateAllocation(string deviceId)
        {
            var hashValue = MurmurHash3.HashUnencodedChars(deviceId);

            var hashFixedBucket = Math.Abs(hashValue) % TotalBuckets;
            var allocationValue = (float)hashFixedBucket / TotalBuckets * MaxPercentage;

            return Math.Round(allocationValue * 100) / 100;
        }
    }
}
