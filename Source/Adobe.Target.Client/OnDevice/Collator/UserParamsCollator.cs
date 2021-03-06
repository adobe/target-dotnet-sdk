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
    using UAParser;

    internal sealed class UserParamsCollator : IParamsCollator
    {
        internal const string UserBrowserType = "browserType";
        internal const string UserBrowserVersion = "browserVersion";
        internal const string UserPlatform = "platform";
        private const string Unknown = "unknown";
        private const string Ios = "iOS";
        private const string Mac = "mac";

        private readonly Parser uaParser;

        public UserParamsCollator()
        {
            this.uaParser = Parser.GetDefault();
        }

        public Dictionary<string, object> CollateParams(TargetDeliveryRequest deliveryRequest, RequestDetails requestDetails = default)
        {
            var result = new Dictionary<string, object>();
            var userAgent = deliveryRequest.DeliveryRequest?.Context?.UserAgent;

            if (string.IsNullOrEmpty(userAgent))
            {
                result.Add(UserBrowserType, Unknown);
                result.Add(UserBrowserVersion, Unknown);
                result.Add(UserPlatform, Unknown);
                return result;
            }

            var info = this.uaParser.Parse(userAgent);

            result.Add(UserBrowserType, GetBrowserType(info));
            result.Add(UserBrowserVersion, info.UA.Major == Parser.Other ? Unknown : info.UA.Major);
            result.Add(UserPlatform, GetPlatform(info));

            return result;
        }

        private static string GetBrowserType(ClientInfo info)
        {
            if (info.UA.Family == Parser.Other)
            {
                return Unknown;
            }

            if (info.OS.Family == Ios && info.Device.Family != Parser.Other)
            {
                return info.Device.Family.ToLowerInvariant();
            }

            return info.UA.Family.ToLowerInvariant();
        }

        private static string GetPlatform(ClientInfo info)
        {
            if (info.OS.Family == Parser.Other)
            {
                return Unknown;
            }

            var platform = info.OS.Family.ToLowerInvariant();

            if (info.OS.Family == Ios && info.Device.Family != Parser.Other)
            {
                return Mac;
            }

            return platform.IndexOf(' ') == -1 ? platform : platform.Substring(0, platform.IndexOf(' '));
        }
    }
}
