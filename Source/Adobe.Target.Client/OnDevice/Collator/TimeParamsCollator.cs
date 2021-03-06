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
    using System;
    using System.Collections.Generic;
    using Adobe.Target.Client.Model;
    using Adobe.Target.Client.Util;
    using Adobe.Target.Delivery.Model;

    internal sealed class TimeParamsCollator : IParamsCollator
    {
        internal const string CurrentTimestamp = "current_timestamp";
        internal const string CurrentDay = "current_day";
        internal const string CurrentTime = "current_time";

        public Dictionary<string, object> CollateParams(TargetDeliveryRequest deliveryRequest = default, RequestDetails requestDetails = default)
        {
            var result = new Dictionary<string, object>();
            var dateTime = TimeProvider.Current.UtcNow;
            result.Add(CurrentTimestamp, new DateTimeOffset(dateTime).ToUnixTimeMilliseconds());
            result.Add(CurrentDay, dateTime.DayOfWeek == 0 ? 7 : (int)dateTime.DayOfWeek);
            result.Add(CurrentTime, dateTime.ToString("HHmm"));
            return result;
        }
    }
}
