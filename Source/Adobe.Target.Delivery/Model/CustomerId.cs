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
    /// CustomerId
    /// </summary>
    [DataContract(Name = "CustomerId")]
    public partial class CustomerId : IEquatable<CustomerId>, IValidatableObject
    {

        /// <summary>
        /// Gets or Sets AuthenticatedState
        /// </summary>
        [DataMember(Name = "authenticatedState", IsRequired = true, EmitDefaultValue = false)]
        public AuthenticatedState AuthenticatedState { get; set; }
        /// <summary>
        /// Initializes a new instance of the <see cref="CustomerId" /> class.
        /// </summary>
        [JsonConstructorAttribute]
        protected CustomerId() { }
        /// <summary>
        /// Initializes a new instance of the <see cref="CustomerId" /> class.
        /// </summary>
        /// <param name="id">id (required).</param>
        /// <param name="integrationCode">This is the **alias** used when setting up a CRS datasource in the Marketing Cloud UI. (required).</param>
        /// <param name="authenticatedState">authenticatedState (required).</param>
        public CustomerId(string id = default(string), string integrationCode = default(string), AuthenticatedState authenticatedState = default(AuthenticatedState))
        {
            // to ensure "id" is required (not null)
            this.Id = id ?? throw new ArgumentNullException("id is a required property for CustomerId and cannot be null");
            // to ensure "integrationCode" is required (not null)
            this.IntegrationCode = integrationCode ?? throw new ArgumentNullException("integrationCode is a required property for CustomerId and cannot be null");
            this.AuthenticatedState = authenticatedState;
        }

        /// <summary>
        /// Gets or Sets Id
        /// </summary>
        [DataMember(Name = "id", IsRequired = true, EmitDefaultValue = false)]
        public string Id { get; set; }

        /// <summary>
        /// This is the **alias** used when setting up a CRS datasource in the Marketing Cloud UI.
        /// </summary>
        /// <value>This is the **alias** used when setting up a CRS datasource in the Marketing Cloud UI.</value>
        [DataMember(Name = "integrationCode", IsRequired = true, EmitDefaultValue = false)]
        public string IntegrationCode { get; set; }

        /// <summary>
        /// Returns the string presentation of the object
        /// </summary>
        /// <returns>String presentation of the object</returns>
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("class CustomerId {\n");
            sb.Append("  Id: ").Append(Id).Append("\n");
            sb.Append("  IntegrationCode: ").Append(IntegrationCode).Append("\n");
            sb.Append("  AuthenticatedState: ").Append(AuthenticatedState).Append("\n");
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
            return this.Equals(input as CustomerId);
        }

        /// <summary>
        /// Returns true if CustomerId instances are equal
        /// </summary>
        /// <param name="input">Instance of CustomerId to be compared</param>
        /// <returns>Boolean</returns>
        public bool Equals(CustomerId input)
        {
            if (input == null)
                return false;

            return 
                (
                    this.Id == input.Id ||
                    (this.Id != null &&
                    this.Id.Equals(input.Id))
                ) && 
                (
                    this.IntegrationCode == input.IntegrationCode ||
                    (this.IntegrationCode != null &&
                    this.IntegrationCode.Equals(input.IntegrationCode))
                ) && 
                (
                    this.AuthenticatedState == input.AuthenticatedState ||
                    this.AuthenticatedState.Equals(input.AuthenticatedState)
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
                if (this.Id != null)
                    hashCode = hashCode * 59 + this.Id.GetHashCode();
                if (this.IntegrationCode != null)
                    hashCode = hashCode * 59 + this.IntegrationCode.GetHashCode();
                hashCode = hashCode * 59 + this.AuthenticatedState.GetHashCode();
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
            // Id (string) maxLength
            if(this.Id != null && this.Id.Length > 128)
            {
                yield return new System.ComponentModel.DataAnnotations.ValidationResult("Invalid value for Id, length must be less than 128.", new [] { "Id" });
            }

            // IntegrationCode (string) maxLength
            if(this.IntegrationCode != null && this.IntegrationCode.Length > 50)
            {
                yield return new System.ComponentModel.DataAnnotations.ValidationResult("Invalid value for IntegrationCode, length must be less than 50.", new [] { "IntegrationCode" });
            }

            yield break;
        }
    }

}
