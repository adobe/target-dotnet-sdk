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
    using System.Reflection;

    /// <summary>
    /// Target Constants
    /// </summary>
    public static class TargetConstants
    {
        /// <summary>
        /// Mbox cookie name
        /// </summary>
        public const string MboxCookieName = "mbox";

        /// <summary>
        /// Cluster cookie name
        /// </summary>
        public const string ClusterCookieName = "mboxEdgeCluster";

        /// <summary>
        /// SessionId cookie name
        /// </summary>
        public const string SessionIdCookieName = "session";

        /// <summary>
        /// DeviceId cookie name
        /// </summary>
        public const string DeviceIdCookieName = "PC";

        /// <summary>
        /// Marketing Cloud Visitor Id Visitor field
        /// </summary>
        public const string MarketingCloudVisitorId = "MCMID";

        /// <summary>
        /// AAM Location Hint Visitor field
        /// </summary>
        public const string AamLocationHint = "MCAAMLH";

        /// <summary>
        /// AAM Blob Visitor field
        /// </summary>
        public const string AamBlob = "MCAAMB";

        /// <summary>
        /// Default Consumer Id for Visitor SDID generation
        /// </summary>
        public const string DefaultSdidConsumerId = "target-global-mbox";

        /// <summary>
        /// SDK Version
        /// </summary>
        public static readonly string SdkVersion = Assembly.GetEntryAssembly()
            !.GetCustomAttribute<AssemblyInformationalVersionAttribute>()
            !.InformationalVersion;

        internal const string SdkNameHeader = "X-EXC-SDK";

        internal const string SdkNameValue = "AdobeTargetNet";

        internal const string SdkVersionHeader = "X-EXC-SDK-Version";

        internal static readonly string SdkUserAgent = $"{SdkNameValue}/{SdkVersion}";
    }
}
