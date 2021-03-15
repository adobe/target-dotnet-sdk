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
namespace Adobe.Target.Client.Model
{
    using System.Collections.Generic;

    /// <summary>
    /// Target Locations
    /// </summary>
    public class Locations
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Locations"/> class.
        /// </summary>
        /// <param name="remoteMboxes">Remote Mboxes</param>
        /// <param name="remoteViews">Remote Views</param>
        /// <param name="globalMbox">Global Mbox name</param>
        public Locations(IReadOnlyList<string> remoteMboxes, IReadOnlyList<string> remoteViews, string globalMbox = "target-global-mbox")
        {
            this.RemoteMboxes = remoteMboxes;
            this.RemoteViews = remoteViews;
            this.GlobalMbox = globalMbox;
        }

        /// <summary>
        /// Global Mbox name
        /// Default: target-global-mbox
        /// </summary>
        public string GlobalMbox { get; }

        /// <summary>
        /// Remote Mboxes
        /// List of mboxes for which only remote decisioning is available
        /// </summary>
        public IReadOnlyList<string> RemoteMboxes { get; }

        /// <summary>
        /// Remote Views
        /// List of views for which only remote decisioning is available
        /// </summary>
        public IReadOnlyList<string> RemoteViews { get; }
    }
}
