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
namespace Adobe.Target.Client.Model
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Adobe.Target.Client.Util;
    using Adobe.Target.Delivery.Model;
    using Newtonsoft.Json.Linq;

    /// <summary>
    /// Target Attributes
    /// </summary>
    public sealed class TargetAttributes
    {
        private const string GlobalMbox = "target-global-mbox";
        private readonly IReadOnlyDictionary<string, IReadOnlyDictionary<string, object>> content;

        /// <summary>
        /// Initializes a new instance of the <see cref="TargetAttributes"/> class.
        /// </summary>
        /// <param name="response">Target Delivery response</param>
        public TargetAttributes(TargetDeliveryResponse response)
        {
            this.Response = response;
            this.content = ToDictionary(response);
        }

        /// <summary>
        /// Target Delivery response
        /// </summary>
        public TargetDeliveryResponse Response { get; }

        /// <summary>
        /// Gets Attributes Dictionary
        /// </summary>
        /// <returns>Attributes Dictionary</returns>
        public IReadOnlyDictionary<string, IReadOnlyDictionary<string, object>> ToDictionary()
        {
            return this.content;
        }

        /// <summary>
        /// Gets Mbox Dictionary
        /// </summary>
        /// <param name="mbox">Mbox name</param>
        /// <returns>Mbox Dictionary</returns>
        public IReadOnlyDictionary<string, object> ToMboxDictionary(string mbox)
        {
            return this.content.TryGetValue(mbox, out var dict) ? dict : null;
        }

        /// <summary>
        /// Gets a Boolean Attribute
        /// </summary>
        /// <param name="mbox">Mbox name</param>
        /// <param name="key">Attribute name</param>
        /// <param name="defaultValue">Default attribute value</param>
        /// <returns>Boolean attribute or default value</returns>
        public bool GetBoolean(string mbox, string key, bool defaultValue = default)
        {
            var mboxDict = this.ToMboxDictionary(mbox);
            if (mboxDict == null)
            {
                return defaultValue;
            }

            return mboxDict.TryGetValue(key, out var value) && value is bool b ? b : defaultValue;
        }

        /// <summary>
        /// Gets a String Attribute
        /// </summary>
        /// <param name="mbox">Mbox name</param>
        /// <param name="key">Attribute name</param>
        /// <param name="defaultValue">Default attribute value</param>
        /// <returns>String attribute or default value</returns>
        public string GetString(string mbox, string key, string defaultValue = default)
        {
            var mboxDict = this.ToMboxDictionary(mbox);
            if (mboxDict == null)
            {
                return defaultValue;
            }

            return mboxDict.TryGetValue(key, out var value) && value != null ? value.ToString() : defaultValue;
        }

        /// <summary>
        /// Gets an Integer Attribute
        /// </summary>
        /// <param name="mbox">Mbox name</param>
        /// <param name="key">Attribute name</param>
        /// <param name="defaultValue">Default attribute value</param>
        /// <returns>Integer attribute or default value</returns>
        public int GetInteger(string mbox, string key, int defaultValue = default)
        {
            return this.GetValue(mbox, key, defaultValue);
        }

        /// <summary>
        /// Gets a Double Attribute
        /// </summary>
        /// <param name="mbox">Mbox name</param>
        /// <param name="key">Attribute name</param>
        /// <param name="defaultValue">Default attribute value</param>
        /// <returns>Double attribute or default value</returns>
        public double GetDouble(string mbox, string key, double defaultValue = default)
        {
            return this.GetValue(mbox, key, defaultValue);
        }

        /// <summary>
        /// Gets an Attribute value
        /// </summary>
        /// <typeparam name="T">Generic type parameter</typeparam>
        /// <param name="mbox">Mbox name</param>
        /// <param name="key">Attribute name</param>
        /// <param name="defaultValue">Default attribute value</param>
        /// <returns>Attribute or default value</returns>
        public T GetValue<T>(string mbox, string key, T defaultValue = default)
        {
            var mboxDict = this.ToMboxDictionary(mbox);
            if (mboxDict == null)
            {
                return defaultValue;
            }

            try
            {
                return mboxDict.TryGetValue(key, out var value) && value != null
                    ? (T)Convert.ChangeType(value, typeof(T)) : defaultValue;
            }
            catch (Exception)
            {
                return defaultValue;
            }
        }

        private static IReadOnlyDictionary<string, IReadOnlyDictionary<string, object>> ToDictionary(TargetDeliveryResponse deliveryResponse)
        {
            if (deliveryResponse?.Response == null)
            {
                return null;
            }

            var result = new Dictionary<string, Dictionary<string, object>>();
            var prefetchResponse = deliveryResponse.Response.Prefetch;
            var executeResponse = deliveryResponse.Response.Execute;

            ProcessResponse(result, prefetchResponse?.PageLoad, prefetchResponse?.Mboxes);
            ProcessResponse(result, executeResponse?.PageLoad, executeResponse?.Mboxes);

            return result.ToDictionary(pair => pair.Key, pair => (IReadOnlyDictionary<string, object>)pair.Value);
        }

        private static void ProcessResponse(IDictionary<string, Dictionary<string, object>> accumulator, PageLoadResponse pageLoad, List<PrefetchMboxResponse> mboxes)
        {
            if (pageLoad != null)
            {
                AddOptions(accumulator, pageLoad.Options, GlobalMbox);
            }

            if (mboxes == null || mboxes.Count == 0)
            {
                return;
            }

            for (var i = mboxes.Count - 1; i >= 0; i--)
            {
                var mbox = mboxes[i];
                if (mbox == null)
                {
                    continue;
                }

                AddOptions(accumulator, mbox.Options, mbox.Name);
            }
        }

        private static void ProcessResponse(IDictionary<string, Dictionary<string, object>> accumulator, PageLoadResponse pageLoad, List<MboxResponse> mboxes)
        {
            if (pageLoad != null)
            {
                AddOptions(accumulator, pageLoad.Options, GlobalMbox);
            }

            if (mboxes == null || mboxes.Count == 0)
            {
                return;
            }

            for (var i = mboxes.Count - 1; i >= 0; i--)
            {
                var mbox = mboxes[i];
                if (mbox == null)
                {
                    continue;
                }

                AddOptions(accumulator, mbox.Options, mbox.Name);
            }
        }

        private static void AddOptions(IDictionary<string, Dictionary<string, object>> accumulator, IReadOnlyList<Option> options, string mbox)
        {
            var mboxContent = CollectionUtils.GetOrCreate(accumulator, mbox);
            for (var i = options.Count - 1; i >= 0; i--)
            {
                var option = options[i];
                if (option.Type != OptionType.Json || option.Content is not JObject)
                {
                    continue;
                }

                var contentDict = ((JObject)option.Content).ToObject<Dictionary<string, object>>();
                if (contentDict == null)
                {
                    continue;
                }

                CollectionUtils.AddAll(mboxContent, contentDict);
            }
        }
    }
}
