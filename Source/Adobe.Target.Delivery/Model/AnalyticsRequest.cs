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
    /// Integration with Adobe Analytics (A4T)
    /// </summary>
    [DataContract(Name = "AnalyticsRequest")]
    public partial class AnalyticsRequest : IEquatable<AnalyticsRequest>, IValidatableObject
    {

        /// <summary>
        /// Gets or Sets Logging
        /// </summary>
        [DataMember(Name = "logging", EmitDefaultValue = false)]
        public LoggingType? Logging { get; set; }
        /// <summary>
        /// Initializes a new instance of the <see cref="AnalyticsRequest" /> class.
        /// </summary>
        /// <param name="supplementalDataId">Supplemental data id, used for **server side** integrations. Format &lt;16 hexadecimal digits&gt;-&lt;16 hexadecimal digits&gt; .</param>
        /// <param name="logging">logging.</param>
        /// <param name="trackingServer">tracking server domain (should not include http://).</param>
        /// <param name="trackingServerSecure">secure tracking server domain (should not include https://).</param>
        public AnalyticsRequest(string supplementalDataId = default(string), LoggingType? logging = default(LoggingType?), string trackingServer = default(string), string trackingServerSecure = default(string))
        {
            this.SupplementalDataId = supplementalDataId;
            this.Logging = logging;
            this.TrackingServer = trackingServer;
            this.TrackingServerSecure = trackingServerSecure;
        }

        /// <summary>
        /// Supplemental data id, used for **server side** integrations. Format &lt;16 hexadecimal digits&gt;-&lt;16 hexadecimal digits&gt; 
        /// </summary>
        /// <value>Supplemental data id, used for **server side** integrations. Format &lt;16 hexadecimal digits&gt;-&lt;16 hexadecimal digits&gt; </value>
        [DataMember(Name = "supplementalDataId", EmitDefaultValue = false)]
        public string SupplementalDataId { get; set; }

        /// <summary>
        /// tracking server domain (should not include http://)
        /// </summary>
        /// <value>tracking server domain (should not include http://)</value>
        [DataMember(Name = "trackingServer", EmitDefaultValue = false)]
        public string TrackingServer { get; set; }

        /// <summary>
        /// secure tracking server domain (should not include https://)
        /// </summary>
        /// <value>secure tracking server domain (should not include https://)</value>
        [DataMember(Name = "trackingServerSecure", EmitDefaultValue = false)]
        public string TrackingServerSecure { get; set; }

        /// <summary>
        /// Returns the string presentation of the object
        /// </summary>
        /// <returns>String presentation of the object</returns>
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("class AnalyticsRequest {\n");
            sb.Append("  SupplementalDataId: ").Append(SupplementalDataId).Append("\n");
            sb.Append("  Logging: ").Append(Logging).Append("\n");
            sb.Append("  TrackingServer: ").Append(TrackingServer).Append("\n");
            sb.Append("  TrackingServerSecure: ").Append(TrackingServerSecure).Append("\n");
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
            return this.Equals(input as AnalyticsRequest);
        }

        /// <summary>
        /// Returns true if AnalyticsRequest instances are equal
        /// </summary>
        /// <param name="input">Instance of AnalyticsRequest to be compared</param>
        /// <returns>Boolean</returns>
        public bool Equals(AnalyticsRequest input)
        {
            if (input == null)
                return false;

            return 
                (
                    this.SupplementalDataId == input.SupplementalDataId ||
                    (this.SupplementalDataId != null &&
                    this.SupplementalDataId.Equals(input.SupplementalDataId))
                ) && 
                (
                    this.Logging == input.Logging ||
                    this.Logging.Equals(input.Logging)
                ) && 
                (
                    this.TrackingServer == input.TrackingServer ||
                    (this.TrackingServer != null &&
                    this.TrackingServer.Equals(input.TrackingServer))
                ) && 
                (
                    this.TrackingServerSecure == input.TrackingServerSecure ||
                    (this.TrackingServerSecure != null &&
                    this.TrackingServerSecure.Equals(input.TrackingServerSecure))
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
                if (this.SupplementalDataId != null)
                    hashCode = hashCode * 59 + this.SupplementalDataId.GetHashCode();
                hashCode = hashCode * 59 + this.Logging.GetHashCode();
                if (this.TrackingServer != null)
                    hashCode = hashCode * 59 + this.TrackingServer.GetHashCode();
                if (this.TrackingServerSecure != null)
                    hashCode = hashCode * 59 + this.TrackingServerSecure.GetHashCode();
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
            // SupplementalDataId (string) maxLength
            if(this.SupplementalDataId != null && this.SupplementalDataId.Length > 33)
            {
                yield return new System.ComponentModel.DataAnnotations.ValidationResult("Invalid value for SupplementalDataId, length must be less than 33.", new [] { "SupplementalDataId" });
            }

            // SupplementalDataId (string) minLength
            if(this.SupplementalDataId != null && this.SupplementalDataId.Length < 33)
            {
                yield return new System.ComponentModel.DataAnnotations.ValidationResult("Invalid value for SupplementalDataId, length must be greater than 33.", new [] { "SupplementalDataId" });
            }

            yield break;
        }
    }

}
