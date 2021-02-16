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
namespace Adobe.Target.Client.Model.OnDevice
{
    using System;

    /// <summary>
    /// OnDeviceDecisioning event handler
    /// </summary>
    public interface IOnDeviceDecisioningHandler
    {
        /// <summary>
        /// This is called once when on-device execution is ready
        /// </summary>
        void OnDeviceDecisioningReady();

        /// <summary>
        /// Called each time an artifact download succeeds
        /// </summary>
        /// <param name="artifactData">Artifact Json</param>
        void ArtifactDownloadSucceeded(string artifactData);

        /// <summary>
        /// /// Called each time an artifact download fails
        /// </summary>
        /// <param name="e">Exception that caused the artifact download to fail</param>
        void ArtifactDownloadFailed(ApplicationException e);
    }
}
