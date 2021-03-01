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
    using System;
    using System.Collections.Generic;
    using Adobe.Target.Client.Extension;
    using Adobe.Target.Client.Model;
    using Adobe.Target.Client.Util;
    using Adobe.Target.Delivery.Model;
    using Microsoft.Extensions.Logging;

    internal sealed class PageParamsCollator : IParamsCollator
    {
        private const string PageUrl = "url";
        private const string PageUrlLower = "url_lc";
        private const string PageDomain = "domain";
        private const string PageDomainLower = "domain_lc";
        private const string PageSubdomain = "subdomain";
        private const string PageSubdomainLower = "subdomain_lc";
        private const string PageTopLevelDomain = "topLevelDomain";
        private const string PageTopLevelDomainLower = "topLevelDomain_lc";
        private const string PagePath = "path";
        private const string PagePathLower = "path_lc";
        private const string PageQuery = "query";
        private const string PageQueryLower = "query_lc";
        private const string PageFragment = "fragment";
        private const string PageFragmentLower = "fragment_lc";
        private const string Www = "www";

        private readonly ILogger logger;
        private readonly bool referring;

        public PageParamsCollator(bool referring = false)
        {
            this.logger = TargetClient.Logger;
            this.referring = referring;
        }

        public Dictionary<string, object> CollateParams(TargetDeliveryRequest deliveryRequest = default, RequestDetails requestDetails = default)
        {
            var result = new Dictionary<string, object>();
            var address = requestDetails?.Address ?? deliveryRequest?.DeliveryRequest?.Context?.Address;
            var url = this.referring ? address?.ReferringUrl : address?.Url;
            if (string.IsNullOrEmpty(url))
            {
                return result;
            }

            try
            {
                var parsed = new Uri(url);
                result.Add(PageUrl, parsed.OriginalString);
                result.Add(PageUrlLower, parsed.OriginalString.ToLowerInvariant());
                result.Add(PageDomain, parsed.Host);
                result.Add(PageDomainLower, parsed.Host.ToLowerInvariant());
                result.AddAll(GetDomainParts(parsed));
                result.Add(PagePath, parsed.AbsolutePath);
                result.Add(PagePathLower, parsed.AbsolutePath.ToLowerInvariant());
                result.Add(PageQuery, parsed.Query);
                result.Add(PageQueryLower, parsed.Query.ToLowerInvariant());
                result.Add(PageFragment, parsed.Fragment);
                result.Add(PageFragmentLower, parsed.Fragment.ToLowerInvariant());
            }
            catch (UriFormatException)
            {
                this.logger?.LogWarning(Messages.MalformedAddressUrl + url);
            }

            return result;
        }

        private static IDictionary<string, object> GetDomainParts(Uri uri)
        {
            var result = new Dictionary<string, object>();
            var parts = uri.Host.Split('.');
            switch (parts.Length)
            {
                case 2:
                    result.Add(PageSubdomain, string.Empty);
                    result.Add(PageSubdomainLower, string.Empty);
                    result.Add(PageTopLevelDomain, parts[1]);
                    result.Add(PageTopLevelDomainLower, parts[1].ToLowerInvariant());
                    break;
                case 3:
                    result.Add(PageSubdomain, parts[0].ToLowerInvariant() == Www ? string.Empty : parts[0]);
                    result.Add(PageSubdomainLower, parts[0].ToLowerInvariant() == Www ? string.Empty : parts[0].ToLowerInvariant());
                    result.Add(PageTopLevelDomain, parts[2]);
                    result.Add(PageTopLevelDomainLower, parts[2].ToLowerInvariant());
                    break;
                case 4:
                    result.Add(PageSubdomain, parts[0].ToLowerInvariant() == Www ? string.Empty : parts[0]);
                    result.Add(PageSubdomainLower, parts[0].ToLowerInvariant() == Www ? string.Empty : parts[0].ToLowerInvariant());
                    result.Add(PageTopLevelDomain, $"{parts[2]}.{parts[3]}");
                    result.Add(PageTopLevelDomainLower, $"{parts[2].ToLowerInvariant()}.{parts[3].ToLowerInvariant()}");
                    break;
                default:
                    result.Add(PageSubdomain, string.Empty);
                    result.Add(PageSubdomainLower, string.Empty);
                    result.Add(PageTopLevelDomain, string.Empty);
                    result.Add(PageTopLevelDomainLower, string.Empty);
                    break;
            }

            return result;
        }
    }
}
