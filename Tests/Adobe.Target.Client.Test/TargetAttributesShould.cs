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
    using System.IO;
    using System.Net;
    using Delivery.Model;
    using Model;
    using Newtonsoft.Json;
    using Xunit;

    public class TargetAttributesShould
    {
        private const string GlobalMbox = "target-global-mbox";

        [Fact]
        public void Constructor_InitializeAttributes()
        {
            var response = new TargetDeliveryResponse(null, GetTestDeliveryResponse(), HttpStatusCode.OK);
            var attributes = new TargetAttributes(response);
            Assert.Equal(response, attributes.Response);

            var dict = attributes.ToDictionary();
            Assert.NotNull(dict);
            Assert.Equal(3, dict.Count);

            var mboxDict = attributes.ToMboxDictionary(GlobalMbox);
            Assert.Equal(2, mboxDict.Count);
            Assert.Equal("executePageLoad", mboxDict["executePageLoad"]);
            Assert.Equal("prefetchPageLoad", mboxDict["prefetchPageLoad"]);

            mboxDict = attributes.ToMboxDictionary("mbox1");
            Assert.Equal(10, mboxDict.Count);
            Assert.Equal("strValLast", attributes.GetString("mbox1", "strVal1"));
            Assert.False(attributes.GetBoolean("mbox1", "boolVal1"));
            Assert.Equal(121, attributes.GetInteger("mbox1", "intVal1"));
            Assert.Equal(451.123, attributes.GetDouble("mbox1", "doubleVal1"));
            Assert.Equal(9223372036854775801, attributes.GetValue<long>("mbox1", "longVal1"));

            mboxDict = attributes.ToMboxDictionary("mbox2");
            Assert.Equal(10, mboxDict.Count);
            Assert.Equal("Welcome Message2", attributes.GetString("mbox2", "strVal2"));
            Assert.True(attributes.GetBoolean("mbox2", "boolVal2"));
            Assert.Equal(122, attributes.GetInteger("mbox2", "intVal2"));
            Assert.Equal(452.123, attributes.GetDouble("mbox1", "doubleVal2"));
            Assert.Equal(9223372036854775802, attributes.GetValue<long>("mbox2", "longVal2"));
        }

        private static DeliveryResponse GetTestDeliveryResponse()
        {
            return JsonConvert.DeserializeObject<DeliveryResponse>(File.ReadAllText("TestDeliveryResponse.json"));
        }
    }
}
