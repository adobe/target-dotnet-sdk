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
    using System;
    using System.Linq;

    internal class TldRule
    {
        public TldRule(string ruleData, TldRuleDivision division = TldRuleDivision.Unknown)
        {
            if (string.IsNullOrEmpty(ruleData))
            {
                throw new ArgumentException("RuleData is empty");
            }

            this.Division = division;

            var parts = ruleData.Split('.').Select(x => x.Trim()).ToList();
            foreach (var part in parts)
            {
                if (string.IsNullOrEmpty(part))
                {
                    throw new FormatException("Rule contains empty part");
                }

                if (part.Contains("*") && part != "*")
                {
                    throw new FormatException("Wildcard syntax not correct");
                }
            }

            if (ruleData.StartsWith("!", StringComparison.InvariantCultureIgnoreCase))
            {
                this.Type = TldRuleType.WildcardException;
                this.Name = ruleData.Substring(1).ToLower();
                this.LabelCount = parts.Count - 1;
            }
            else if (ruleData.Contains("*"))
            {
                this.Type = TldRuleType.Wildcard;
                this.Name = ruleData.ToLower();
                this.LabelCount = parts.Count;
            }
            else
            {
                this.Type = TldRuleType.Normal;
                this.Name = ruleData.ToLower();
                this.LabelCount = parts.Count;
            }
        }

        public string Name { get; private set; }

        public TldRuleType Type { get; private set; }

        public int LabelCount { get; private set; }

        public TldRuleDivision Division { get; private set; }

        public override string ToString()
        {
            return this.Name;
        }
    }
}
#pragma warning restore SA1636
