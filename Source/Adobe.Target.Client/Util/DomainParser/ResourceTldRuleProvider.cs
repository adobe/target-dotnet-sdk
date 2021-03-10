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
namespace Adobe.Target.Client.Util.DomainParser
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Reflection;
    using System.Threading.Tasks;

    internal class ResourceTldRuleProvider : ITldRuleProvider
    {
        private const string PublicSuffixListResource = "Adobe.Target.Client.Util.DomainParser.public_suffix_list.dat";

        public async Task<IEnumerable<TldRule>> BuildAsync()
        {
            var ruleData = await this.LoadFromResourceAsync().ConfigureAwait(false);

            var ruleParser = new TldRuleParser();
            var rules = ruleParser.ParseRules(ruleData);
            return rules;
        }

        private async Task<string> LoadFromResourceAsync()
        {
            var assembly = Assembly.GetExecutingAssembly();

            try
            {
                using (Stream stream = assembly.GetManifestResourceStream(PublicSuffixListResource))
                using (StreamReader reader = new StreamReader(stream ?? throw new InvalidOperationException()))
                {
                    return await reader.ReadToEndAsync().ConfigureAwait(false);
                }
            }
            catch (Exception e)
            {
                throw new ApplicationException("Unable to read public suffix resource", e);
            }
        }
    }
}
