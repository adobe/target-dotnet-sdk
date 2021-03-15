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
    using Util;
    using Xunit;

    public class AllocationProviderShould
    {
        [Fact]
        public void MurmurHash3_HashUnencodedChars()
        {
            var hashed = MurmurHash3.HashUnencodedChars("someClientId.123456.tntId123.salty");
            Assert.Equal(-1846592194, hashed);

            hashed = MurmurHash3.HashUnencodedChars("targettesting.125880.4c038b35f1b1453d80a3e7da8208c617.campaign");
            Assert.Equal(-683299703, hashed);
        }

        [Fact]
        public void AllocationUtils_CalculateAllocation()
        {
            var result = AllocationUtils.CalculateAllocation(
                "someClientId",
                "123456",
                "ecid123",
                "salty");
            Assert.Equal(29.06, result);

            result = AllocationUtils.CalculateAllocation(
                "someClientId",
                "123456",
                "tntId123",
                "salty");
            Assert.Equal(21.94, result);

            result = AllocationUtils.CalculateAllocation(
                "someClientId",
                "123456",
                "tntId123.28_0",
                "salty");
            Assert.Equal(21.94, result);
        }
    }
}
