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

    internal class TldRuleParser
    {
        private readonly char[] lineBreak = new char[] { '\n', '\r' };

        public IEnumerable<TldRule> ParseRules(string data)
        {
            var lines = data.Split(this.lineBreak);
            return this.ParseRules(lines);
        }

        public IEnumerable<TldRule> ParseRules(IEnumerable<string> lines)
        {
            var items = new List<TldRule>();
            var division = TldRuleDivision.Unknown;

            foreach (var line in lines)
            {
                if (string.IsNullOrEmpty(line))
                {
                    continue;
                }

                if (line.StartsWith("//"))
                {
                    if (line.StartsWith("// ===BEGIN ICANN DOMAINS==="))
                    {
                        division = TldRuleDivision.ICANN;
                    }
                    else if (line.StartsWith("// ===END ICANN DOMAINS==="))
                    {
                        division = TldRuleDivision.Unknown;
                    }
                    else if (line.StartsWith("// ===BEGIN PRIVATE DOMAINS==="))
                    {
                        division = TldRuleDivision.Private;
                    }
                    else if (line.StartsWith("// ===END PRIVATE DOMAINS==="))
                    {
                        division = TldRuleDivision.Unknown;
                    }

                    continue;
                }

                var tldRule = new TldRule(line.Trim(), division);
                items.Add(tldRule);
            }

            return items;
        }
    }
}
#pragma warning restore SA1636
