#pragma warning disable SA1636
/*
 * The MIT License (MIT)
 * Copyright (c) 2016 nager.at
 *
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 *
 * The above copyright notice and this permission notice shall be included in all
 * copies or substantial portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
 * SOFTWARE.
 */
namespace Adobe.Target.Client.Util.DomainParser
{
    using System.Collections.Generic;
    using System.Linq;

    internal static class DomainDataStructureExtensions
    {
        /// <summary>
        /// Add all the rules in <paramref name="tldRules"/> to <paramref name="structure"/>.
        /// </summary>
        /// <param name="structure">The structure to appened the rule.</param>
        /// <param name="tldRules">The rules to append.</param>
        public static void AddRules(this DomainDataStructure structure, IEnumerable<TldRule> tldRules)
        {
            foreach (var tldRule in tldRules)
            {
                structure.AddRule(tldRule);
            }
        }

        /// <summary>
        /// Add <paramref name="tldRule"/> to <paramref name="structure"/>.
        /// </summary>
        /// <param name="structure">The structure to appened the rule.</param>
        /// <param name="tldRule">The rule to append.</param>
        public static void AddRule(this DomainDataStructure structure, TldRule tldRule)
        {
            var parts = tldRule.Name.Split('.').Reverse().ToList();
            for (var i = 0; i < parts.Count; i++)
            {
                var domainPart = parts[i];
                if (parts.Count - 1 > i)
                {
                    if (!structure.Nested.ContainsKey(domainPart))
                    {
                        structure.Nested.Add(domainPart, new DomainDataStructure(domainPart));
                    }

                    structure = structure.Nested[domainPart];
                    continue;
                }

                if (structure.Nested.ContainsKey(domainPart))
                {
                    structure.Nested[domainPart].TldRule = tldRule;
                    continue;
                }

                structure.Nested.Add(domainPart, new DomainDataStructure(domainPart, tldRule));
            }
        }
    }
}
#pragma warning restore SA1636
