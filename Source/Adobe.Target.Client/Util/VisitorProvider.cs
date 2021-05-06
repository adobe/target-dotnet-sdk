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
    using Adobe.ExperienceCloud.Ecid;

    internal sealed class VisitorProvider
    {
        private const string AmcvPrefix = "AMCV_";
        private static VisitorProvider instance;
        private string orgId;

        private VisitorProvider(string orgId)
        {
            this.orgId = orgId;
            this.VisitorCookieName = AmcvPrefix + orgId;
        }

        internal static VisitorProvider Instance
        {
            get
            {
                if (instance == null)
                {
                    throw new ApplicationException("VisitorProvider instance is not initialized");
                }

                return instance;
            }
        }

        internal string VisitorCookieName { get; }

        internal static void Initialize(string orgId)
        {
            instance = new VisitorProvider(orgId);
        }

        internal Visitor CreateVisitor(string visitorCookie)
        {
            return new (this.orgId, visitorCookie);
        }
    }
}
