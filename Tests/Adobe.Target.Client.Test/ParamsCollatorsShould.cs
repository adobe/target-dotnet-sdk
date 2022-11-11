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
namespace Adobe.Target.Client.Test
{
    using System;
    using System.Collections.Generic;
    using Delivery.Model;
    using Model;
    using Moq;
    using OnDevice.Collator;
    using Client.Util;
    using Newtonsoft.Json;
    using Xunit;

    [Collection("Datetime-mocked Collection")]
    public class ParamsCollatorsShould
    {
        private const string testClientId = "testClientId";
        private const string testOrgId = "testOrgId";

        public ParamsCollatorsShould()
        {
            var targetClientConfig = new TargetClientConfig.Builder(testClientId, testOrgId).Build();
            _ = TargetClient.Create(targetClientConfig);
        }

        [Fact]
        public void CollateParams_ReturnPageParams()
        {
            const string url = "https://WwW.JoRA.TeSt.cO.Uk:80/Home/Index.html?q1=v1&Q2=V2#FragmentName";
            const string urlLower = "https://www.jora.test.co.uk:80/home/index.html?q1=v1&q2=v2#fragmentname";
            const string domain = "www.jora.test.co.uk";
            const string domainLower = "www.jora.test.co.uk";
            const string subdomain = "jora";
            const string subdomainLower = "jora";
            const string topdomain = "co.uk";
            const string topdomainLower = "co.uk";
            const string path = "/Home/Index.html";
            const string pathLower = "/home/index.html";
            const string query = "?q1=v1&Q2=V2";
            const string queryLower = "?q1=v1&q2=v2";
            const string fragment = "#FragmentName";
            const string fragmentLower = "#fragmentname";

            var request = new TargetDeliveryRequest.Builder()
                .SetContext(new Context(ChannelType.Web, address: new Address(url)))
                .Build();
            var result = new PageParamsCollator().CollateParams(request);
            Assert.Equal(url, result[PageParamsCollator.PageUrl]);
            Assert.Equal(urlLower, result[PageParamsCollator.PageUrlLower]);
            Assert.Equal(domain, result[PageParamsCollator.PageDomain]);
            Assert.Equal(domainLower, result[PageParamsCollator.PageDomainLower]);
            Assert.Equal(subdomain, result[PageParamsCollator.PageSubdomain]);
            Assert.Equal(subdomainLower, result[PageParamsCollator.PageSubdomainLower]);
            Assert.Equal(topdomain, result[PageParamsCollator.PageTopLevelDomain]);
            Assert.Equal(topdomainLower, result[PageParamsCollator.PageTopLevelDomainLower]);
            Assert.Equal(path, result[PageParamsCollator.PagePath]);
            Assert.Equal(pathLower, result[PageParamsCollator.PagePathLower]);
            Assert.Equal(query, result[PageParamsCollator.PageQuery]);
            Assert.Equal(queryLower, result[PageParamsCollator.PageQueryLower]);
            Assert.Equal(fragment, result[PageParamsCollator.PageFragment]);
            Assert.Equal(fragmentLower, result[PageParamsCollator.PageFragmentLower]);
        }

        [Fact]
        public void CollateParams_ReturnGeoParams()
        {
            const float LAT = 47.345f;
            const float LONG = -27.872f;
            const string COUNTRY = "France";
            const string STATE = "Normandy";
            const string CITY = "Le Havre";
            const string CITY_OUT = "LEHAVRE";

            var request = new TargetDeliveryRequest.Builder()
                .SetContext(new Context(ChannelType.Web, geo: new Geo(null,
                    LAT, LONG, COUNTRY, STATE, CITY)))
                .Build();
            var result = new GeoParamsCollator().CollateParams(request);

            Assert.Equal(LAT, result[GeoParamsCollator.GeoLatitude]);
            Assert.Equal(LONG, result[GeoParamsCollator.GeoLongitude]);
            Assert.Equal(COUNTRY.ToUpperInvariant(), result[GeoParamsCollator.GeoCountry]);
            Assert.Equal(STATE.ToUpperInvariant(), result[GeoParamsCollator.GeoRegion]);
            Assert.Equal(CITY_OUT, result[GeoParamsCollator.GeoCity]);
        }

        [Fact]
        public void CollateParams_ReturnCustomParams()
        {
            var paramDict = new Dictionary<string, string>
            {
                {"foo", "bar"},
                {"BAZ", "BUZ"},
                {"dot.notation", "isConfusing"},
                {"first.second.third", "value"},
                {"first.second.wonky", "DONKEY"},
                {"this..should..be", "ignored"},
                {".something", "aaa"},
                {"=cranky .chicken.", "bbb"}
            };
            var details = new RequestDetails(parameters: paramDict);
            var result = new CustomParamsCollator().CollateParams(requestDetails: details);

            var dot = (Dictionary<String, Object>) result["dot"];
            Assert.Equal("isConfusing", dot["notation"]);
            Assert.Equal("isconfusing", dot["notation_lc"]);

            var first = (Dictionary<String, Object>) result["first"];
            var second = (Dictionary<String, Object>) first["second"];

            Assert.Equal("value", second["third"]);
            Assert.Equal("value", second["third_lc"]);
            Assert.Equal("DONKEY", second["wonky"]);
            Assert.Equal("donkey", second["wonky_lc"]);

            Assert.Equal("aaa", result[".something"]);
            Assert.Equal("bbb", result["=cranky .chicken."]);
        }


        [Fact]
        public void CollateParams_ReturnTimeParams()
        {
            var mockDateTime = DateTime.SpecifyKind(
                new DateTime(2021, 3, 8, 4, 20, 1), DateTimeKind.Utc);
            var timeMock = new Mock<TimeProvider>();
            timeMock
                .SetupGet(tp => tp.UtcNow)
                .Returns(mockDateTime.ToUniversalTime());
            TimeProvider.Current = timeMock.Object;
            var result = new TimeParamsCollator().CollateParams();

            Assert.Equal(1615177201000, result[TimeParamsCollator.CurrentTimestamp]);
            Assert.Equal(1, result[TimeParamsCollator.CurrentDay]);
            Assert.Equal("0420", result[TimeParamsCollator.CurrentTime]);
            TimeProvider.ResetToDefault();
        }

        [Fact]
        public void CollateParams_ReturnUserParams()
        {
            const string IE11_COMPAT_WIN = "Mozilla/4.0 (compatible; MSIE 7.0; Windows NT 10.0; WOW64; Trident/8.0; .NET4.0C; .NET4.0E)";
            const string EDGE_WIN = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/79.0.3945.74 " +
                                    "Safari/537.36 Edg/79.0.309.43";
            const string FIREFOX_MAC = "Mozilla/5.0 (Macintosh; Intel Mac OS X 10.15; rv:78.0) Gecko/20100101 Firefox/78.0";
            const string FIREFOX_LINUX = "Mozilla/5.0 (X11; Linux Mint/18.2; Linux x86_64; rv:58.0) Gecko/20100101 Firefox/58.0";
            const string CHROME_MAC = "Mozilla/5.0 (Macintosh; Intel Mac OS X 10_9_2) AppleWebKit/537.36 (KHTML, like Gecko) " +
                                      "Chrome/36.0.1944.0 Safari/537.36";
            const string SAFARI_MAC = "Mozilla/5.0 (Macintosh; Intel Mac OS X 10_9_3) AppleWebKit/537.75.14 " +
                                      "(KHTML, like Gecko) Version/7.0.3 Safari/7046A194A";
            const string OPERA_WIN = "Opera/9.80 (Windows NT 6.1; U; fi) Presto/2.2.15 Version/10.00";
            const string IPAD = "Mozilla/5.0 (iPad; U; CPU OS 11_2 like Mac OS X; zh-CN; iPad5,3) AppleWebKit/534.46 (KHTML, like Gecko) UCBrowser/3.0.1.776 U3/ Mobile/10A403 Safari/7543.48.3";
            const string IPHONE = "Mozilla/5.0 (iPhone; U; CPU iPhone OS 4_1 like Mac OS X; en-us) AppleWebKit/532.9 " +
                                  "(KHTML, like Gecko) Version/4.0.5 Mobile/8B117 Safari/6531.22.7";

            var request = new TargetDeliveryRequest.Builder()
                .SetContext(new Context(ChannelType.Web, userAgent: IE11_COMPAT_WIN))
                .Build();
            var result = new UserParamsCollator().CollateParams(request);

            Assert.Equal("ie", result[UserParamsCollator.UserBrowserType]);
            Assert.Equal("11", result[UserParamsCollator.UserBrowserVersion]);
            Assert.Equal("windows", result[UserParamsCollator.UserPlatform]);

            request = new TargetDeliveryRequest.Builder()
                .SetContext(new Context(ChannelType.Web, userAgent: EDGE_WIN))
                .Build();
            result = new UserParamsCollator().CollateParams(request);

            Assert.Equal("edge", result[UserParamsCollator.UserBrowserType]);
            Assert.Equal("79", result[UserParamsCollator.UserBrowserVersion]);
            Assert.Equal("windows", result[UserParamsCollator.UserPlatform]);

            request = new TargetDeliveryRequest.Builder()
                .SetContext(new Context(ChannelType.Web, userAgent: FIREFOX_MAC))
                .Build();
            result = new UserParamsCollator().CollateParams(request);

            Assert.Equal("firefox", result[UserParamsCollator.UserBrowserType]);
            Assert.Equal("78", result[UserParamsCollator.UserBrowserVersion]);
            Assert.Equal("mac", result[UserParamsCollator.UserPlatform]);

            request = new TargetDeliveryRequest.Builder()
                .SetContext(new Context(ChannelType.Web, userAgent: FIREFOX_LINUX))
                .Build();
            result = new UserParamsCollator().CollateParams(request);

            Assert.Equal("firefox", result[UserParamsCollator.UserBrowserType]);
            Assert.Equal("58", result[UserParamsCollator.UserBrowserVersion]);
            Assert.Equal("linux", result[UserParamsCollator.UserPlatform]);

            request = new TargetDeliveryRequest.Builder()
                .SetContext(new Context(ChannelType.Web, userAgent: CHROME_MAC))
                .Build();
            result = new UserParamsCollator().CollateParams(request);

            Assert.Equal("chrome", result[UserParamsCollator.UserBrowserType]);
            Assert.Equal("36", result[UserParamsCollator.UserBrowserVersion]);
            Assert.Equal("mac", result[UserParamsCollator.UserPlatform]);

            request = new TargetDeliveryRequest.Builder()
                .SetContext(new Context(ChannelType.Web, userAgent: SAFARI_MAC))
                .Build();
            result = new UserParamsCollator().CollateParams(request);

            Assert.Equal("safari", result[UserParamsCollator.UserBrowserType]);
            Assert.Equal("7", result[UserParamsCollator.UserBrowserVersion]);
            Assert.Equal("mac", result[UserParamsCollator.UserPlatform]);

            request = new TargetDeliveryRequest.Builder()
                .SetContext(new Context(ChannelType.Web, userAgent: OPERA_WIN))
                .Build();
            result = new UserParamsCollator().CollateParams(request);

            Assert.Equal("opera", result[UserParamsCollator.UserBrowserType]);
            Assert.Equal("10", result[UserParamsCollator.UserBrowserVersion]);
            Assert.Equal("windows", result[UserParamsCollator.UserPlatform]);

            request = new TargetDeliveryRequest.Builder()
                .SetContext(new Context(ChannelType.Web, userAgent: IPAD))
                .Build();
            result = new UserParamsCollator().CollateParams(request);

            Assert.Equal("ipad", result[UserParamsCollator.UserBrowserType]);
            Assert.Equal("3", result[UserParamsCollator.UserBrowserVersion]);
            Assert.Equal("mac", result[UserParamsCollator.UserPlatform]);

            request = new TargetDeliveryRequest.Builder()
                .SetContext(new Context(ChannelType.Web, userAgent: IPHONE))
                .Build();
            result = new UserParamsCollator().CollateParams(request);

            Assert.Equal("iphone", result[UserParamsCollator.UserBrowserType]);
            Assert.Equal("4", result[UserParamsCollator.UserBrowserVersion]);
            Assert.Equal("mac", result[UserParamsCollator.UserPlatform]);
        }
    }
}
