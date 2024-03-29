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
    /// Client hints data.  Used in place of userAgent if provided. 
    /// </summary>
    [DataContract(Name = "ClientHints")]
    public partial class ClientHints : IEquatable<ClientHints>, IValidatableObject
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ClientHints" /> class.
        /// </summary>
        /// <param name="mobile">Sec-CH-UA-Mobile (low entropy).</param>
        /// <param name="model">Sec-CH-UA-Model.</param>
        /// <param name="platform">Sec-CH-UA-Platform (low entropy).</param>
        /// <param name="platformVersion">Sec-CH-UA-Platform-Version.</param>
        /// <param name="browserUAWithMajorVersion">Sec-CH-UA (low entropy).</param>
        /// <param name="browserUAWithFullVersion">Sec-CH-UA-Full-Version-List.</param>
        /// <param name="architecture">Sec-CH-UA-Arch.</param>
        /// <param name="bitness">Sec-CH-UA-Bitness.</param>
        public ClientHints(bool mobile = default(bool), string model = default(string), string platform = default(string), string platformVersion = default(string), string browserUAWithMajorVersion = default(string), string browserUAWithFullVersion = default(string), string architecture = default(string), string bitness = default(string))
        {
            this.Mobile = mobile;
            this.Model = model;
            this.Platform = platform;
            this.PlatformVersion = platformVersion;
            this.BrowserUAWithMajorVersion = browserUAWithMajorVersion;
            this.BrowserUAWithFullVersion = browserUAWithFullVersion;
            this.Architecture = architecture;
            this.Bitness = bitness;
        }

        /// <summary>
        /// Sec-CH-UA-Mobile (low entropy)
        /// </summary>
        /// <value>Sec-CH-UA-Mobile (low entropy)</value>
        [DataMember(Name = "mobile", EmitDefaultValue = true)]
        public bool Mobile { get; set; }

        /// <summary>
        /// Sec-CH-UA-Model
        /// </summary>
        /// <value>Sec-CH-UA-Model</value>
        [DataMember(Name = "model", EmitDefaultValue = false)]
        public string Model { get; set; }

        /// <summary>
        /// Sec-CH-UA-Platform (low entropy)
        /// </summary>
        /// <value>Sec-CH-UA-Platform (low entropy)</value>
        [DataMember(Name = "platform", EmitDefaultValue = false)]
        public string Platform { get; set; }

        /// <summary>
        /// Sec-CH-UA-Platform-Version
        /// </summary>
        /// <value>Sec-CH-UA-Platform-Version</value>
        [DataMember(Name = "platformVersion", EmitDefaultValue = false)]
        public string PlatformVersion { get; set; }

        /// <summary>
        /// Sec-CH-UA (low entropy)
        /// </summary>
        /// <value>Sec-CH-UA (low entropy)</value>
        [DataMember(Name = "browserUAWithMajorVersion", EmitDefaultValue = false)]
        public string BrowserUAWithMajorVersion { get; set; }

        /// <summary>
        /// Sec-CH-UA-Full-Version-List
        /// </summary>
        /// <value>Sec-CH-UA-Full-Version-List</value>
        [DataMember(Name = "browserUAWithFullVersion", EmitDefaultValue = false)]
        public string BrowserUAWithFullVersion { get; set; }

        /// <summary>
        /// Sec-CH-UA-Arch
        /// </summary>
        /// <value>Sec-CH-UA-Arch</value>
        [DataMember(Name = "architecture", EmitDefaultValue = false)]
        public string Architecture { get; set; }

        /// <summary>
        /// Sec-CH-UA-Bitness
        /// </summary>
        /// <value>Sec-CH-UA-Bitness</value>
        [DataMember(Name = "bitness", EmitDefaultValue = false)]
        public string Bitness { get; set; }

        /// <summary>
        /// Returns the string presentation of the object
        /// </summary>
        /// <returns>String presentation of the object</returns>
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("class ClientHints {\n");
            sb.Append("  Mobile: ").Append(Mobile).Append("\n");
            sb.Append("  Model: ").Append(Model).Append("\n");
            sb.Append("  Platform: ").Append(Platform).Append("\n");
            sb.Append("  PlatformVersion: ").Append(PlatformVersion).Append("\n");
            sb.Append("  BrowserUAWithMajorVersion: ").Append(BrowserUAWithMajorVersion).Append("\n");
            sb.Append("  BrowserUAWithFullVersion: ").Append(BrowserUAWithFullVersion).Append("\n");
            sb.Append("  Architecture: ").Append(Architecture).Append("\n");
            sb.Append("  Bitness: ").Append(Bitness).Append("\n");
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
            return this.Equals(input as ClientHints);
        }

        /// <summary>
        /// Returns true if ClientHints instances are equal
        /// </summary>
        /// <param name="input">Instance of ClientHints to be compared</param>
        /// <returns>Boolean</returns>
        public bool Equals(ClientHints input)
        {
            if (input == null)
                return false;

            return 
                (
                    this.Mobile == input.Mobile ||
                    this.Mobile.Equals(input.Mobile)
                ) && 
                (
                    this.Model == input.Model ||
                    (this.Model != null &&
                    this.Model.Equals(input.Model))
                ) && 
                (
                    this.Platform == input.Platform ||
                    (this.Platform != null &&
                    this.Platform.Equals(input.Platform))
                ) && 
                (
                    this.PlatformVersion == input.PlatformVersion ||
                    (this.PlatformVersion != null &&
                    this.PlatformVersion.Equals(input.PlatformVersion))
                ) && 
                (
                    this.BrowserUAWithMajorVersion == input.BrowserUAWithMajorVersion ||
                    (this.BrowserUAWithMajorVersion != null &&
                    this.BrowserUAWithMajorVersion.Equals(input.BrowserUAWithMajorVersion))
                ) && 
                (
                    this.BrowserUAWithFullVersion == input.BrowserUAWithFullVersion ||
                    (this.BrowserUAWithFullVersion != null &&
                    this.BrowserUAWithFullVersion.Equals(input.BrowserUAWithFullVersion))
                ) && 
                (
                    this.Architecture == input.Architecture ||
                    (this.Architecture != null &&
                    this.Architecture.Equals(input.Architecture))
                ) && 
                (
                    this.Bitness == input.Bitness ||
                    (this.Bitness != null &&
                    this.Bitness.Equals(input.Bitness))
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
                hashCode = hashCode * 59 + this.Mobile.GetHashCode();
                if (this.Model != null)
                    hashCode = hashCode * 59 + this.Model.GetHashCode();
                if (this.Platform != null)
                    hashCode = hashCode * 59 + this.Platform.GetHashCode();
                if (this.PlatformVersion != null)
                    hashCode = hashCode * 59 + this.PlatformVersion.GetHashCode();
                if (this.BrowserUAWithMajorVersion != null)
                    hashCode = hashCode * 59 + this.BrowserUAWithMajorVersion.GetHashCode();
                if (this.BrowserUAWithFullVersion != null)
                    hashCode = hashCode * 59 + this.BrowserUAWithFullVersion.GetHashCode();
                if (this.Architecture != null)
                    hashCode = hashCode * 59 + this.Architecture.GetHashCode();
                if (this.Bitness != null)
                    hashCode = hashCode * 59 + this.Bitness.GetHashCode();
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
