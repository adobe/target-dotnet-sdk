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
namespace Adobe.Target.Client.Service
{
    using System;
    using System.Threading;
    using Adobe.Target.Client.OnDevice.Collator;

    /// <summary>
    /// OnDevice Decisioning Service
    /// </summary>
    internal sealed class OnDeviceDecisioningService
    {
        private readonly TargetClientConfig clientConfig;
        private readonly TargetService targetService;
        private readonly RuleLoader ruleLoader;

        /// <summary>
        /// Initializes a new instance of the <see cref="OnDeviceDecisioningService"/> class.
        /// </summary>
        /// <param name="clientConfig">Client config</param>
        /// <param name="targetService">Target Service</param>
        internal OnDeviceDecisioningService(TargetClientConfig clientConfig, TargetService targetService)
        {
            this.clientConfig = clientConfig;
            this.targetService = targetService;
            this.ruleLoader = new RuleLoader(clientConfig);
        }
    }
}
