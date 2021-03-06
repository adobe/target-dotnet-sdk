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
    /// Browser object may be specified only when the Channel is Web.
    /// </summary>
    [DataContract(Name = "Browser")]
    public partial class Browser : IEquatable<Browser>, IValidatableObject
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Browser" /> class.
        /// </summary>
        /// <param name="host">Current web page host.</param>
        /// <param name="language">Language in Accept-Language header format, see RFC 7231 sec. 5.3.5.</param>
        /// <param name="webGLRenderer">This is an optional field, added to help with device detection using device atlas .</param>
        public Browser(string host = default(string), string language = default(string), string webGLRenderer = default(string))
        {
            this.Host = host;
            this.Language = language;
            this.WebGLRenderer = webGLRenderer;
        }

        /// <summary>
        /// Current web page host
        /// </summary>
        /// <value>Current web page host</value>
        [DataMember(Name = "host", EmitDefaultValue = false)]
        public string Host { get; set; }

        /// <summary>
        /// Language in Accept-Language header format, see RFC 7231 sec. 5.3.5
        /// </summary>
        /// <value>Language in Accept-Language header format, see RFC 7231 sec. 5.3.5</value>
        [DataMember(Name = "language", EmitDefaultValue = false)]
        public string Language { get; set; }

        /// <summary>
        /// This is an optional field, added to help with device detection using device atlas 
        /// </summary>
        /// <value>This is an optional field, added to help with device detection using device atlas </value>
        [DataMember(Name = "webGLRenderer", EmitDefaultValue = false)]
        public string WebGLRenderer { get; set; }

        /// <summary>
        /// Returns the string presentation of the object
        /// </summary>
        /// <returns>String presentation of the object</returns>
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("class Browser {\n");
            sb.Append("  Host: ").Append(Host).Append("\n");
            sb.Append("  Language: ").Append(Language).Append("\n");
            sb.Append("  WebGLRenderer: ").Append(WebGLRenderer).Append("\n");
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
            return this.Equals(input as Browser);
        }

        /// <summary>
        /// Returns true if Browser instances are equal
        /// </summary>
        /// <param name="input">Instance of Browser to be compared</param>
        /// <returns>Boolean</returns>
        public bool Equals(Browser input)
        {
            if (input == null)
                return false;

            return 
                (
                    this.Host == input.Host ||
                    (this.Host != null &&
                    this.Host.Equals(input.Host))
                ) && 
                (
                    this.Language == input.Language ||
                    (this.Language != null &&
                    this.Language.Equals(input.Language))
                ) && 
                (
                    this.WebGLRenderer == input.WebGLRenderer ||
                    (this.WebGLRenderer != null &&
                    this.WebGLRenderer.Equals(input.WebGLRenderer))
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
                if (this.Host != null)
                    hashCode = hashCode * 59 + this.Host.GetHashCode();
                if (this.Language != null)
                    hashCode = hashCode * 59 + this.Language.GetHashCode();
                if (this.WebGLRenderer != null)
                    hashCode = hashCode * 59 + this.WebGLRenderer.GetHashCode();
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
