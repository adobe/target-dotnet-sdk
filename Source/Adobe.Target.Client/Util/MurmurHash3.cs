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
namespace Adobe.Target.Client.Util
{
    internal static class MurmurHash3
    {
        private const int Seed = 0;
        private const int CharBytes = 2;
        private const uint C1 = 0xcc9e2d51;
        private const uint C2 = 0x1b873593;
        private const int R1 = 15;
        private const int R2 = 13;
        private const int M = 5;
        private const uint N = 0xe6546b64;

        /// <summary>
        /// hashUnencodedChars() implementation ported from Guava
        /// </summary>
        /// <param name="input">Input</param>
        /// <returns>Hashed output</returns>
        public static int HashUnencodedChars(string input)
        {
            return unchecked((int)HashStringUnsigned(input));
        }

        private static uint HashStringUnsigned(string input)
        {
            uint h1 = Seed;

            for (int i = 1; i < input.Length; i += 2)
            {
                uint k1 = unchecked((uint)input[i - 1] | (((uint)input[i]) << 16));
                k1 = MixK1(k1);
                h1 = MixH1(h1, k1);
            }

            if ((input.Length & 1) == 1)
            {
                uint k1 = input[input.Length - 1];
                k1 = MixK1(k1);
                h1 ^= k1;
            }

            return Fmix(h1, CharBytes * input.Length);
        }

        private static uint MixK1(uint k1)
        {
            k1 *= C1;
            k1 = RotateLeft(k1, R1);
            k1 *= C2;
            return k1;
        }

        private static uint MixH1(uint h1, uint k1)
        {
            h1 ^= k1;
            h1 = RotateLeft(h1, R2);
            h1 = (h1 * M) + N;
            return h1;
        }

        private static uint Fmix(uint h1, int length)
        {
            h1 ^= (uint)length;
            h1 ^= h1 >> 16;
            h1 *= 0x85ebca6b;
            h1 ^= h1 >> 13;
            h1 *= 0xc2b2ae35;
            h1 ^= h1 >> 16;
            return h1;
        }

        private static uint RotateLeft(uint i, int distance)
        {
            return (i << distance) | (i >> (32 - distance));
        }
    }
}
