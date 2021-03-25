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
    using System.Collections.Generic;

    internal sealed class OnDeviceDecisioningEvaluation
    {
        public OnDeviceDecisioningEvaluation(
            bool allLocal,
            string reason = default,
            string globalMbox = default,
            IReadOnlyList<string> remoteMboxes = default,
            IReadOnlyList<string> remoteViews = default)
        {
            this.AllLocal = allLocal;
            this.Reason = reason;
            this.GlobalMbox = globalMbox;
            this.RemoteMboxes = remoteMboxes;
            this.RemoteViews = remoteViews;
        }

        internal bool AllLocal { get; }

        internal string Reason { get; }

        internal string GlobalMbox { get; }

        internal IReadOnlyList<string> RemoteMboxes { get; }

        internal IReadOnlyList<string> RemoteViews { get; }
    }
}
