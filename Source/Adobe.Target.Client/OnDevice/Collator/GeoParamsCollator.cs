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
    using Adobe.Target.Client.Model;
    using Adobe.Target.Delivery.Model;

    internal sealed class GeoParamsCollator : IParamsCollator
    {
        internal const string GeoLatitude = "latitude";
        internal const string GeoLongitude = "longitude";
        internal const string GeoCity = "city";
        internal const string GeoRegion = "region";
        internal const string GeoCountry = "country";

        public Dictionary<string, object> CollateParams(TargetDeliveryRequest deliveryRequest = default, RequestDetails requestDetails = default)
        {
            var result = new Dictionary<string, object>();
            var geo = deliveryRequest?.DeliveryRequest?.Context?.Geo;
            if (geo == null)
            {
                return result;
            }

            if (geo.Latitude != 0)
            {
                result.Add(GeoLatitude, geo.Latitude);
            }

            if (geo.Longitude != 0)
            {
                result.Add(GeoLongitude, geo.Longitude);
            }

            if (!string.IsNullOrEmpty(geo.City))
            {
                result.Add(GeoCity, geo.City.Replace(" ", string.Empty).ToUpperInvariant());
            }

            if (!string.IsNullOrEmpty(geo.StateCode))
            {
                result.Add(GeoRegion, geo.StateCode.ToUpperInvariant());
            }

            if (!string.IsNullOrEmpty(geo.CountryCode))
            {
                result.Add(GeoCountry, geo.CountryCode.ToUpperInvariant());
            }

            return result;
        }
    }
}
