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
#nullable enable
namespace Adobe.Target.Client.Util
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Adobe.Target.Client.Model;

    /// <summary>
    /// Cookie Utils
    /// </summary>
    internal static class CookieUtils
    {
        private const char CookieValueSeparator = '|';
        private const char InternalCookieSerializationSeparator = '#';
        private const int SessionIdCookieMaxAge = 1860;
        private const int DeviceIdCookieMaxAge = 63244800;
        private const int ClusterLocationHintMaxAge = 1860;

        /// <summary>
        /// Parse Target cookie
        /// </summary>
        /// <param name="targetCookie">Target cookie</param>
        /// <returns>Parsed Target cookies</returns>
        internal static Dictionary<string, string> ParseTargetCookie(string? targetCookie)
        {
            if (string.IsNullOrEmpty(targetCookie))
            {
                return new Dictionary<string, string>();
            }

            return targetCookie!.Split(CookieValueSeparator)
                .TakeWhile(cookie => !string.IsNullOrEmpty(cookie))
                .Select(DeserializeInternalCookie)
                .Where(internalCookie => internalCookie != null && internalCookie.MaxAge > GetNowInSeconds())
                .ToDictionary(internalCookie => internalCookie!.Name, internalCookie => internalCookie!.Value);
        }

        /// <summary>
        /// Gets Location Hint from Tnt Id
        /// </summary>
        /// <param name="tntId">Tnt Id</param>
        /// <returns>Location hint</returns>
        internal static string? LocationHintFromTntId(string tntId)
        {
            var parts = tntId.Split('.');
            if (parts.Length != 2)
            {
                return null;
            }

            var nodeDetails = parts[1].Split('_');
            return nodeDetails.Length != 2 ? null : nodeDetails[0];
        }

        internal static TargetCookie? CreateTargetCookie(string? sessionId, string? deviceId)
        {
            var nowInSeconds = GetNowInSeconds();
            var targetCookieValue = new StringBuilder();
            var expires = 0;
            expires = CreateSessionId(sessionId, nowInSeconds, targetCookieValue, expires);
            expires = CreateDeviceId(deviceId, nowInSeconds, targetCookieValue, expires);
            var maxAge = expires == 0 ? 0 : expires - nowInSeconds;
            var cookieValue = targetCookieValue.ToString();

            return string.IsNullOrEmpty(cookieValue) ? null : new TargetCookie(TargetConstants.MboxCookieName, cookieValue, maxAge);
        }

        internal static TargetCookie? CreateClusterCookie(string? tntId)
        {
            if (tntId == null)
            {
                return null;
            }

            var locationHint = LocationHintFromTntId(tntId);

            if (string.IsNullOrEmpty(locationHint))
            {
                return null;
            }

            return new TargetCookie(TargetConstants.ClusterCookieName, locationHint, ClusterLocationHintMaxAge);
        }

        private static int GetNowInSeconds()
        {
            return (int)(DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() / 1000);
        }

        private static int CreateDeviceId(string? deviceId, int nowInSeconds, StringBuilder targetCookieValue, int expires)
        {
            if (string.IsNullOrEmpty(deviceId))
            {
                return expires;
            }

            var deviceIdExpires = nowInSeconds + DeviceIdCookieMaxAge;
            expires = Math.Max(expires, deviceIdExpires);
            AppendCookieValue(deviceId, targetCookieValue, deviceIdExpires, TargetConstants.DeviceIdCookieName);

            return expires;
        }

        private static int CreateSessionId(string? sessionId, int nowInSeconds, StringBuilder targetCookieValue, int expires)
        {
            if (string.IsNullOrEmpty(sessionId))
            {
                return expires;
            }

            var sessionIdExpires = nowInSeconds + SessionIdCookieMaxAge;
            expires = sessionIdExpires;
            AppendCookieValue(sessionId, targetCookieValue, sessionIdExpires, TargetConstants.SessionIdCookieName);

            return expires;
        }

        private static void AppendCookieValue(string? id, StringBuilder targetCookieValue, int expires, string cookieName)
        {
            targetCookieValue.Append(cookieName)
                .Append(InternalCookieSerializationSeparator)
                .Append(id)
                .Append(InternalCookieSerializationSeparator)
                .Append(expires)
                .Append(CookieValueSeparator);
        }

        private static TargetCookie? DeserializeInternalCookie(string cookie)
        {
            var cookieTokens = cookie.Split(InternalCookieSerializationSeparator);
            if (cookieTokens.Length != 3)
            {
                return null;
            }

            if (!int.TryParse(cookieTokens[2], out var maxAge))
            {
                return null;
            }

            return new TargetCookie(cookieTokens[0], cookieTokens[1], maxAge);
        }
    }
}
