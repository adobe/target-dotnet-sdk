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
    using System.Collections.Generic;

    internal static class CollectionUtils
    {
        internal static TValue GetOrCreate<TKey, TValue>(IDictionary<TKey, TValue> dict, TKey key)
            where TValue : new()
        {
            if (dict.TryGetValue(key, out var val))
            {
                return val;
            }

            val = new TValue();
            dict.Add(key, val);

            return val;
        }

        internal static void AddAll<TKey, TValue>(IDictionary<TKey, TValue> to, IDictionary<TKey, TValue> from)
        {
            foreach (var entry in from)
            {
                if (to.ContainsKey(entry.Key))
                {
                    continue;
                }

                to.Add(entry.Key, entry.Value);
            }
        }
    }
}
