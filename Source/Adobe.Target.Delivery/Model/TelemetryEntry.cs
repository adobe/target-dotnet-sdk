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
    /// Telemetry Entry.
    /// </summary>
    [DataContract(Name = "TelemetryEntry")]
    public partial class TelemetryEntry : IEquatable<TelemetryEntry>, IValidatableObject
    {

        /// <summary>
        /// Gets or Sets Mode
        /// </summary>
        [DataMember(Name = "mode", EmitDefaultValue = false)]
        public ExecutionMode? Mode { get; set; }
        /// <summary>
        /// Initializes a new instance of the <see cref="TelemetryEntry" /> class.
        /// </summary>
        /// <param name="requestId">Request Id.</param>
        /// <param name="timestamp">Timestamp of the entry, in milliseconds elapsed since UNIX epoch..</param>
        /// <param name="mode">mode.</param>
        /// <param name="execution">Execution time in milliseconds..</param>
        /// <param name="parsing">Response parsing time, in milliseconds elapsed since UNIX epoch..</param>
        /// <param name="features">features.</param>
        /// <param name="request">request.</param>
        /// <param name="telemetryServerToken">Encoded data with telemetry collected from previous request to Delivery API.</param>
        public TelemetryEntry(string requestId = default(string), long? timestamp = default(long?), ExecutionMode? mode = default(ExecutionMode?), double? execution = default(double?), double? parsing = default(double?), TelemetryFeatures features = default(TelemetryFeatures), TelemetryRequest request = default(TelemetryRequest), string telemetryServerToken = default(string))
        {
            this.RequestId = requestId;
            this.Timestamp = timestamp;
            this.Mode = mode;
            this.Execution = execution;
            this.Parsing = parsing;
            this.Features = features;
            this.Request = request;
            this.TelemetryServerToken = telemetryServerToken;
        }

        /// <summary>
        /// Request Id
        /// </summary>
        /// <value>Request Id</value>
        [DataMember(Name = "requestId", EmitDefaultValue = false)]
        public string RequestId { get; set; }

        /// <summary>
        /// Timestamp of the entry, in milliseconds elapsed since UNIX epoch.
        /// </summary>
        /// <value>Timestamp of the entry, in milliseconds elapsed since UNIX epoch.</value>
        [DataMember(Name = "timestamp", EmitDefaultValue = false)]
        public long? Timestamp { get; set; }

        /// <summary>
        /// Execution time in milliseconds.
        /// </summary>
        /// <value>Execution time in milliseconds.</value>
        [DataMember(Name = "execution", EmitDefaultValue = false)]
        public double? Execution { get; set; }

        /// <summary>
        /// Response parsing time, in milliseconds elapsed since UNIX epoch.
        /// </summary>
        /// <value>Response parsing time, in milliseconds elapsed since UNIX epoch.</value>
        [DataMember(Name = "parsing", EmitDefaultValue = false)]
        public double? Parsing { get; set; }

        /// <summary>
        /// Gets or Sets Features
        /// </summary>
        [DataMember(Name = "features", EmitDefaultValue = false)]
        public TelemetryFeatures Features { get; set; }

        /// <summary>
        /// Gets or Sets Request
        /// </summary>
        [DataMember(Name = "request", EmitDefaultValue = false)]
        public TelemetryRequest Request { get; set; }

        /// <summary>
        /// Encoded data with telemetry collected from previous request to Delivery API
        /// </summary>
        /// <value>Encoded data with telemetry collected from previous request to Delivery API</value>
        [DataMember(Name = "telemetryServerToken", EmitDefaultValue = false)]
        public string TelemetryServerToken { get; set; }

        /// <summary>
        /// Returns the string presentation of the object
        /// </summary>
        /// <returns>String presentation of the object</returns>
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("class TelemetryEntry {\n");
            sb.Append("  RequestId: ").Append(RequestId).Append("\n");
            sb.Append("  Timestamp: ").Append(Timestamp).Append("\n");
            sb.Append("  Mode: ").Append(Mode).Append("\n");
            sb.Append("  Execution: ").Append(Execution).Append("\n");
            sb.Append("  Parsing: ").Append(Parsing).Append("\n");
            sb.Append("  Features: ").Append(Features).Append("\n");
            sb.Append("  Request: ").Append(Request).Append("\n");
            sb.Append("  TelemetryServerToken: ").Append(TelemetryServerToken).Append("\n");
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
            return this.Equals(input as TelemetryEntry);
        }

        /// <summary>
        /// Returns true if TelemetryEntry instances are equal
        /// </summary>
        /// <param name="input">Instance of TelemetryEntry to be compared</param>
        /// <returns>Boolean</returns>
        public bool Equals(TelemetryEntry input)
        {
            if (input == null)
                return false;

            return 
                (
                    this.RequestId == input.RequestId ||
                    (this.RequestId != null &&
                    this.RequestId.Equals(input.RequestId))
                ) && 
                (
                    this.Timestamp == input.Timestamp ||
                    this.Timestamp.Equals(input.Timestamp)
                ) && 
                (
                    this.Mode == input.Mode ||
                    this.Mode.Equals(input.Mode)
                ) && 
                (
                    this.Execution == input.Execution ||
                    this.Execution.Equals(input.Execution)
                ) && 
                (
                    this.Parsing == input.Parsing ||
                    this.Parsing.Equals(input.Parsing)
                ) && 
                (
                    this.Features == input.Features ||
                    (this.Features != null &&
                    this.Features.Equals(input.Features))
                ) && 
                (
                    this.Request == input.Request ||
                    (this.Request != null &&
                    this.Request.Equals(input.Request))
                ) && 
                (
                    this.TelemetryServerToken == input.TelemetryServerToken ||
                    (this.TelemetryServerToken != null &&
                    this.TelemetryServerToken.Equals(input.TelemetryServerToken))
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
                if (this.RequestId != null)
                    hashCode = hashCode * 59 + this.RequestId.GetHashCode();
                hashCode = hashCode * 59 + this.Timestamp.GetHashCode();
                hashCode = hashCode * 59 + this.Mode.GetHashCode();
                hashCode = hashCode * 59 + this.Execution.GetHashCode();
                hashCode = hashCode * 59 + this.Parsing.GetHashCode();
                if (this.Features != null)
                    hashCode = hashCode * 59 + this.Features.GetHashCode();
                if (this.Request != null)
                    hashCode = hashCode * 59 + this.Request.GetHashCode();
                if (this.TelemetryServerToken != null)
                    hashCode = hashCode * 59 + this.TelemetryServerToken.GetHashCode();
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
