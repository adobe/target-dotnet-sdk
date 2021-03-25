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
    using System.Net;
    using Adobe.Target.Client.Util;
    using Adobe.Target.Delivery.Model;
    using Newtonsoft.Json;

    /// <summary>
    /// Target Delivery Response
    /// </summary>
    public sealed class TargetDeliveryResponse
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TargetDeliveryResponse"/> class.
        /// </summary>
        /// <param name="request">Request</param>
        /// <param name="response">Response</param>
        /// <param name="status">Status</param>
        /// <param name="message">Message</param>
        /// <param name="locations">Locations</param>
        public TargetDeliveryResponse(
            TargetDeliveryRequest request,
            DeliveryResponse response,
            HttpStatusCode status,
            string message = "",
            Locations locations = default)
        {
            this.Request = request;
            this.Response = response;
            this.Status = status;
            this.Message = message;
            this.Locations = locations;
        }

        /// <summary>
        /// Target Delivery request
        /// </summary>
        public TargetDeliveryRequest Request { get; }

        /// <summary>
        /// Delivery response
        /// </summary>
        public DeliveryResponse Response { get; }

        /// <summary>
        /// Status
        /// </summary>
        public HttpStatusCode Status { get; }

        /// <summary>
        /// Message
        /// </summary>
        public string Message { get; }

        /// <summary>
        /// Locations, i.e. mboxes and views
        /// </summary>
        public Locations Locations { get; }

        /// <summary>
        /// Gets Target cookies
        /// </summary>
        /// <returns>Target cookies</returns>
        public Dictionary<string, TargetCookie> GetCookies()
        {
            var cookies = new Dictionary<string, TargetCookie>();

            if (this.Response == null || this.Response.Status < (int)HttpStatusCode.OK ||
                this.Response.Status >= (int)HttpStatusCode.Ambiguous)
            {
                return cookies;
            }

            var targetCookie = CookieUtils.CreateTargetCookie(this.Request.SessionId, this.Response.Id.TntId);
            if (targetCookie != null)
            {
                cookies.Add(TargetConstants.MboxCookieName, targetCookie);
            }

            var clusterCookie = CookieUtils.CreateClusterCookie(this.Response.Id.TntId);
            if (clusterCookie != null)
            {
                cookies.Add(TargetConstants.ClusterCookieName, clusterCookie);
            }

            return cookies;
        }

        /// <inheritdoc />
        public override string ToString() => JsonConvert.SerializeObject(this, Newtonsoft.Json.Formatting.Indented);
    }
}
