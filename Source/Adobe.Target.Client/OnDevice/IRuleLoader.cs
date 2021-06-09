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
namespace Adobe.Target.Client.OnDevice
{
    using System;
    using Adobe.Target.Client.Model.OnDevice;

    /// <summary>
    /// Rule Loader
    /// </summary>
    public interface IRuleLoader
    {
        /// <summary>
        /// Artifact URL
        /// </summary>
        string ArtifactUrl { get; }

        /// <summary>
        /// Polling Interval
        /// </summary>
        int PollingInterval { get; }

        /// <summary>
        /// Fetch Count
        /// </summary>
        int FetchCount { get; }

        /// <summary>
        /// Last fetch datetime
        /// </summary>
        DateTimeOffset LastFetch { get; }

        /// <summary>
        /// Gets latest rules
        /// </summary>
        /// <returns>Last downloaded rules artifact</returns>
        OnDeviceDecisioningRuleSet? GetLatestRules();
    }
}
