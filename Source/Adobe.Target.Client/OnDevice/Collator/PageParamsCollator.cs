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
    using Adobe.Target.Client.Model;
    using Adobe.Target.Client.Util;
    using Adobe.Target.Client.Util.DomainParser;
    using Adobe.Target.Delivery.Model;
    using Microsoft.Extensions.Logging;

    internal sealed class PageParamsCollator : IParamsCollator
    {
        internal const string PageUrl = "url";
        internal const string PageUrlLower = "url_lc";
        internal const string PageDomain = "domain";
        internal const string PageDomainLower = "domain_lc";
        internal const string PageSubdomain = "subdomain";
        internal const string PageSubdomainLower = "subdomain_lc";
        internal const string PageTopLevelDomain = "topLevelDomain";
        internal const string PageTopLevelDomainLower = "topLevelDomain_lc";
        internal const string PagePath = "path";
        internal const string PagePathLower = "path_lc";
        internal const string PageQuery = "query";
        internal const string PageQueryLower = "query_lc";
        internal const string PageFragment = "fragment";
        internal const string PageFragmentLower = "fragment_lc";
        internal const string Www = "www";

        private readonly ILogger logger;
        private readonly DomainParser domainParser;
        private readonly bool referring;

        public PageParamsCollator(bool referring = false)
        {
            this.logger = TargetClient.Logger;
            this.domainParser = new DomainParser();
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
                var domainInfo = this.domainParser.Parse(url);
                var subdomain = GetSubDomain(domainInfo.SubDomain);

                result.Add(PageUrl, parsed.OriginalString);
                result.Add(PageUrlLower, parsed.OriginalString.ToLowerInvariant());
                result.Add(PageDomain, parsed.Host);
                result.Add(PageDomainLower, parsed.Host.ToLowerInvariant());
                result.Add(PageSubdomain, subdomain);
                result.Add(PageSubdomainLower, subdomain.ToLowerInvariant());
                result.Add(PageTopLevelDomain, domainInfo.Tld);
                result.Add(PageTopLevelDomainLower, domainInfo.Tld.ToLowerInvariant());
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

        private static string GetSubDomain(string subdomain)
        {
            if (subdomain.ToLowerInvariant().Equals(Www))
            {
                return string.Empty;
            }

            if (subdomain.ToLowerInvariant().StartsWith(Www + '.'))
            {
                return subdomain.Substring(subdomain.IndexOf('.') + 1);
            }

            return subdomain;
        }
    }
}
