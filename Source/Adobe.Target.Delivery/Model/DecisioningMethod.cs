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
    /// Defines DecisioningMethod
    /// </summary>
    [JsonConverter(typeof(StringEnumConverter))]
    public enum DecisioningMethod
    {
        /// <summary>
        /// Enum ServerSide for value: server-side
        /// </summary>
        [EnumMember(Value = "server-side")]
        ServerSide = 1,

        /// <summary>
        /// Enum OnDevice for value: on-device
        /// </summary>
        [EnumMember(Value = "on-device")]
        OnDevice = 2,

        /// <summary>
        /// Enum Hybrid for value: hybrid
        /// </summary>
        [EnumMember(Value = "hybrid")]
        Hybrid = 3

    }

}
