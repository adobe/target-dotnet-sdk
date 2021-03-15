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
    using Adobe.Target.Delivery.Model;

    internal sealed class RequestDetailsUnion
    {
        private readonly RequestDetails requestDetails;
        private readonly MboxRequest mboxRequest;
        private readonly ViewRequest viewRequest;
        private readonly int tag;

        internal RequestDetailsUnion(RequestDetails requestDetails)
        {
            this.requestDetails = requestDetails;
            this.tag = 0;
        }

        internal RequestDetailsUnion(MboxRequest mboxRequest)
        {
            this.mboxRequest = mboxRequest;
            this.tag = 1;
        }

        internal RequestDetailsUnion(ViewRequest viewRequest)
        {
            this.viewRequest = viewRequest;
            this.tag = 2;
        }

        internal RequestDetails GetRequestDetails()
        {
            return this.tag switch
            {
                0 => this.requestDetails,
                1 => new RequestDetails(
                    this.mboxRequest.Address,
                    this.mboxRequest.Parameters,
                    this.mboxRequest.ProfileParameters,
                    this.mboxRequest.Order,
                    this.mboxRequest.Product),
                2 => new RequestDetails(
                    this.viewRequest.Address,
                    this.viewRequest.Parameters,
                    this.viewRequest.ProfileParameters,
                    this.viewRequest.Order,
                    this.viewRequest.Product),
                _ => throw new ApplicationException("Unrecognized tag value: " + this.tag)
            };
        }

        internal T Match<T>(Func<RequestDetails, T> pageLoadFunc, Func<MboxRequest, T> mboxFunc, Func<ViewRequest, T> viewFunc)
        {
            return this.tag switch
            {
                0 => pageLoadFunc(this.requestDetails),
                1 => mboxFunc(this.mboxRequest),
                2 => viewFunc(this.viewRequest),
                _ => throw new ApplicationException("Unrecognized tag value: " + this.tag)
            };
        }
    }
}
