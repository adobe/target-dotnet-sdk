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
namespace Adobe.Target.Client.OnDevice
{
    using System;
    using System.Threading.Tasks;
    using Adobe.Target.Delivery.Model;
    using RestSharp;

    internal sealed class GeoClient : IGeoClient
    {
        private const string GeoPath = "/v1/geo";
        private const string GeoIpHeader = "x-forwarded-for";

        private const string GeoHeaderCity = "x-geo-city";
        private const string GeoHeaderRegion = "x-geo-region-code";
        private const string GeoHeaderCountry = "x-geo-country-code";
        private const string GeoHeaderLatitude = "x-geo-latitude";
        private const string GeoHeaderLongitude = "x-geo-longitude";

        private readonly RestClient restClient;

        internal GeoClient(TargetClientConfig clientConfig)
        {
            this.restClient = new RestClient(this.GetGeoUrl(clientConfig))
            {
                Timeout = clientConfig.Timeout, Proxy = clientConfig.Proxy,
            };
        }

        public async Task<Geo> LookupGeoAsync(Geo geo)
        {
            if (!this.ValidateGeo(geo))
            {
                return geo;
            }

            var request = new RestRequest().AddHeader(GeoIpHeader, geo.IpAddress);
            var response = await this.restClient.ExecuteGetAsync(request).ConfigureAwait(false);
            return this.HeadersToGeo(geo, response);
        }

        private bool ValidateGeo(Geo geo)
        {
            return geo != null && !string.IsNullOrEmpty(geo.IpAddress) && string.IsNullOrEmpty(geo.City)
                   && string.IsNullOrEmpty(geo.StateCode) && string.IsNullOrEmpty(geo.CountryCode)
                   && geo.Latitude == 0 && geo.Longitude == 0;
        }

        private Geo HeadersToGeo(Geo originalGeo, IRestResponse response)
        {
            if (!response.IsSuccessful)
            {
                return originalGeo;
            }

            var geo = new Geo(originalGeo.IpAddress);
            foreach (var header in response.Headers)
            {
                switch (header.Name?.ToLowerInvariant())
                {
                    case GeoHeaderCity:
                        geo.City = (string)header.Value;
                        break;
                    case GeoHeaderRegion:
                        geo.StateCode = (string)header.Value;
                        break;
                    case GeoHeaderCountry:
                        geo.CountryCode = (string)header.Value;
                        break;
                    case GeoHeaderLatitude:
                        geo.Latitude = float.TryParse((string)header.Value, out var parsedLat) ? parsedLat : 0;
                        break;
                    case GeoHeaderLongitude:
                        geo.Longitude = float.TryParse((string)header.Value, out var parsedLong) ? parsedLong : 0;
                        break;
                }
            }

            return geo;
        }

        private string GetGeoUrl(TargetClientConfig clientConfig)
        {
            return Uri.UriSchemeHttps + "://" + clientConfig.OnDeviceConfigHostname + GeoPath;
        }
    }
}
