// <auto-generated/>
/*
 * Copyright 2021 Adobe. All rights reserved.
 * This file is licensed to you under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License. You may obtain a copy
 * of the License at http://www.apache.org/licenses/LICENSE-2.0
 * Unless required by applicable law or agreed to in writing, software distributed under
 * the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR REPRESENTATIONS
 * OF ANY KIND, either express or implied. See the License for the specific language
 * governing permissions and limitations under the License.
 * Generated by: https://github.com/openapitools/openapi-generator.git
 */

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.IO;
using System.Runtime.Serialization;
using System.Text;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using System.ComponentModel.DataAnnotations;
using OpenAPIDateConverter = Adobe.Target.Delivery.Client.OpenAPIDateConverter;

namespace Adobe.Target.Delivery.Model
{
    /// <summary>
    /// Delivery response. Returned content will be based upon the request and client&#39;s active activities.
    /// </summary>
    [DataContract(Name = "DeliveryResponse")]
    public partial class DeliveryResponse : IEquatable<DeliveryResponse>, IValidatableObject
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DeliveryResponse" /> class.
        /// </summary>
        /// <param name="status">status.</param>
        /// <param name="requestId">ID of the processed request. If it&#39;s not sent in the request, a random ID (UUID) is generated and returned with the response. .</param>
        /// <param name="id">id.</param>
        /// <param name="_client">Client&#39;s code. The one which was sent in the request&#39;s path..</param>
        /// <param name="edgeHost">Cluster host name that served the response. Ideally, all subsequent requests should be made to that host..</param>
        /// <param name="execute">execute.</param>
        /// <param name="prefetch">prefetch.</param>
        /// <param name="notifications">notifications.</param>
        public DeliveryResponse(int? status = default(int?), string requestId = default(string), VisitorId id = default(VisitorId), string _client = default(string), string edgeHost = default(string), ExecuteResponse execute = default(ExecuteResponse), PrefetchResponse prefetch = default(PrefetchResponse), NotificationResponse notifications = default(NotificationResponse))
        {
            this.Status = status;
            this.RequestId = requestId;
            this.Id = id;
            this._Client = _client;
            this.EdgeHost = edgeHost;
            this.Execute = execute;
            this.Prefetch = prefetch;
            this.Notifications = notifications;
        }

        /// <summary>
        /// Gets or Sets Status
        /// </summary>
        [DataMember(Name = "status", EmitDefaultValue = false)]
        public int? Status { get; set; }

        /// <summary>
        /// ID of the processed request. If it&#39;s not sent in the request, a random ID (UUID) is generated and returned with the response. 
        /// </summary>
        /// <value>ID of the processed request. If it&#39;s not sent in the request, a random ID (UUID) is generated and returned with the response. </value>
        [DataMember(Name = "requestId", EmitDefaultValue = false)]
        public string RequestId { get; set; }

        /// <summary>
        /// Gets or Sets Id
        /// </summary>
        [DataMember(Name = "id", EmitDefaultValue = false)]
        public VisitorId Id { get; set; }

        /// <summary>
        /// Client&#39;s code. The one which was sent in the request&#39;s path.
        /// </summary>
        /// <value>Client&#39;s code. The one which was sent in the request&#39;s path.</value>
        [DataMember(Name = "client", EmitDefaultValue = false)]
        public string _Client { get; set; }

        /// <summary>
        /// Cluster host name that served the response. Ideally, all subsequent requests should be made to that host.
        /// </summary>
        /// <value>Cluster host name that served the response. Ideally, all subsequent requests should be made to that host.</value>
        [DataMember(Name = "edgeHost", EmitDefaultValue = false)]
        public string EdgeHost { get; set; }

        /// <summary>
        /// Gets or Sets Execute
        /// </summary>
        [DataMember(Name = "execute", EmitDefaultValue = false)]
        public ExecuteResponse Execute { get; set; }

        /// <summary>
        /// Gets or Sets Prefetch
        /// </summary>
        [DataMember(Name = "prefetch", EmitDefaultValue = false)]
        public PrefetchResponse Prefetch { get; set; }

        /// <summary>
        /// Gets or Sets Notifications
        /// </summary>
        [DataMember(Name = "notifications", EmitDefaultValue = false)]
        public NotificationResponse Notifications { get; set; }

        /// <summary>
        /// Returns the string presentation of the object
        /// </summary>
        /// <returns>String presentation of the object</returns>
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("class DeliveryResponse {\n");
            sb.Append("  Status: ").Append(Status).Append("\n");
            sb.Append("  RequestId: ").Append(RequestId).Append("\n");
            sb.Append("  Id: ").Append(Id).Append("\n");
            sb.Append("  _Client: ").Append(_Client).Append("\n");
            sb.Append("  EdgeHost: ").Append(EdgeHost).Append("\n");
            sb.Append("  Execute: ").Append(Execute).Append("\n");
            sb.Append("  Prefetch: ").Append(Prefetch).Append("\n");
            sb.Append("  Notifications: ").Append(Notifications).Append("\n");
            sb.Append("}\n");
            return sb.ToString();
        }

        /// <summary>
        /// Returns the JSON string presentation of the object
        /// </summary>
        /// <returns>JSON string presentation of the object</returns>
        public virtual string ToJson()
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(this, Newtonsoft.Json.Formatting.Indented);
        }

        /// <summary>
        /// Returns true if objects are equal
        /// </summary>
        /// <param name="input">Object to be compared</param>
        /// <returns>Boolean</returns>
        public override bool Equals(object input)
        {
            return this.Equals(input as DeliveryResponse);
        }

        /// <summary>
        /// Returns true if DeliveryResponse instances are equal
        /// </summary>
        /// <param name="input">Instance of DeliveryResponse to be compared</param>
        /// <returns>Boolean</returns>
        public bool Equals(DeliveryResponse input)
        {
            if (input == null)
                return false;

            return 
                (
                    this.Status == input.Status ||
                    this.Status.Equals(input.Status)
                ) && 
                (
                    this.RequestId == input.RequestId ||
                    (this.RequestId != null &&
                    this.RequestId.Equals(input.RequestId))
                ) && 
                (
                    this.Id == input.Id ||
                    (this.Id != null &&
                    this.Id.Equals(input.Id))
                ) && 
                (
                    this._Client == input._Client ||
                    (this._Client != null &&
                    this._Client.Equals(input._Client))
                ) && 
                (
                    this.EdgeHost == input.EdgeHost ||
                    (this.EdgeHost != null &&
                    this.EdgeHost.Equals(input.EdgeHost))
                ) && 
                (
                    this.Execute == input.Execute ||
                    (this.Execute != null &&
                    this.Execute.Equals(input.Execute))
                ) && 
                (
                    this.Prefetch == input.Prefetch ||
                    (this.Prefetch != null &&
                    this.Prefetch.Equals(input.Prefetch))
                ) && 
                (
                    this.Notifications == input.Notifications ||
                    (this.Notifications != null &&
                    this.Notifications.Equals(input.Notifications))
                );
        }

        /// <summary>
        /// Gets the hash code
        /// </summary>
        /// <returns>Hash code</returns>
        public override int GetHashCode()
        {
            unchecked // Overflow is fine, just wrap
            {
                int hashCode = 41;
                hashCode = hashCode * 59 + this.Status.GetHashCode();
                if (this.RequestId != null)
                    hashCode = hashCode * 59 + this.RequestId.GetHashCode();
                if (this.Id != null)
                    hashCode = hashCode * 59 + this.Id.GetHashCode();
                if (this._Client != null)
                    hashCode = hashCode * 59 + this._Client.GetHashCode();
                if (this.EdgeHost != null)
                    hashCode = hashCode * 59 + this.EdgeHost.GetHashCode();
                if (this.Execute != null)
                    hashCode = hashCode * 59 + this.Execute.GetHashCode();
                if (this.Prefetch != null)
                    hashCode = hashCode * 59 + this.Prefetch.GetHashCode();
                if (this.Notifications != null)
                    hashCode = hashCode * 59 + this.Notifications.GetHashCode();
                return hashCode;
            }
        }

        /// <summary>
        /// To validate all properties of the instance
        /// </summary>
        /// <param name="validationContext">Validation context</param>
        /// <returns>Validation Result</returns>
        IEnumerable<System.ComponentModel.DataAnnotations.ValidationResult> IValidatableObject.Validate(ValidationContext validationContext)
        {
            yield break;
        }
    }

}
