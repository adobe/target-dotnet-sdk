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
namespace Adobe.Target.Client.OnDevice.Collator
{
    using System.Collections.Generic;
    using Adobe.Target.Client.Extension;
    using Adobe.Target.Client.Model;
    using Adobe.Target.Delivery.Model;

    internal sealed class GeoParamsCollator : IParamsCollator
    {
        internal const string GeoLatitude = "latitude";
        internal const string GeoLongitude = "longitude";
        internal const string GeoCity = "city";
        internal const string GeoRegion = "region";
        internal const string GeoCountry = "country";

        internal static readonly IDictionary<string, object> DefaultGeoParams = new Dictionary<string, object>
        {
            { GeoLatitude, 0f },
            { GeoLongitude, 0f },
            { GeoCity, string.Empty },
            { GeoRegion, string.Empty },
            { GeoCountry, string.Empty },
        };

        public Dictionary<string, object> CollateParams(TargetDeliveryRequest deliveryRequest = default, RequestDetails requestDetails = default)
        {
            var result = new Dictionary<string, object>();
            var geo = deliveryRequest?.DeliveryRequest?.Context?.Geo;
            if (geo == null)
            {
                result.AddAll(DefaultGeoParams);
                return result;
            }

            result.Add(GeoLatitude, geo.Latitude);
            result.Add(GeoLongitude, geo.Longitude);
            result.Add(GeoCity, string.IsNullOrEmpty(geo.City) ? string.Empty : geo.City.Replace(" ", string.Empty).ToUpperInvariant());
            result.Add(GeoRegion, string.IsNullOrEmpty(geo.StateCode) ? string.Empty : geo.StateCode.ToUpperInvariant());
            result.Add(GeoCountry, string.IsNullOrEmpty(geo.CountryCode) ? string.Empty : geo.CountryCode.ToUpperInvariant());

            return result;
        }
    }
}
