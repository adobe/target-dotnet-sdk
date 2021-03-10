#pragma warning disable SA1636
#pragma warning disable VSTHRD002
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
    using System.Collections.Generic;
    using System.Linq;

    internal class DomainParser
    {
        private readonly UriNormalizer domainNormalizer;
        private readonly TldRule rootTldRule = new ("*");
        private DomainDataStructure domainDataStructure;

        public DomainParser()
            : this(new ResourceTldRuleProvider())
        {
        }

        public DomainParser(ITldRuleProvider ruleProvider)
        {
            this.domainNormalizer = new UriNormalizer();
            var rules = ruleProvider.BuildAsync().GetAwaiter().GetResult();
            this.AddRules(rules);
        }

        public DomainInfo Parse(Uri domain)
        {
            var partlyNormalizedDomain = domain.Host;
            var normalizedHost = domain.GetComponents(UriComponents.NormalizedHost, UriFormat.UriEscaped);

            var parts = normalizedHost
                .Split('.')
                .Reverse()
                .ToList();

            return this.GetDomainFromParts(partlyNormalizedDomain, parts);
        }

        public DomainInfo Parse(string domain)
        {
            var parts = this.domainNormalizer.PartlyNormalizeDomainAndExtractFullyNormalizedParts(domain, out string partlyNormalizedDomain);
            return this.GetDomainFromParts(partlyNormalizedDomain, parts);
        }

        public bool IsValidDomain(string domain)
        {
            if (string.IsNullOrEmpty(domain))
            {
                return false;
            }

            if (Uri.TryCreate(domain, UriKind.Absolute, out _))
            {
                return false;
            }

            if (!Uri.TryCreate($"http://{domain}", UriKind.Absolute, out var uri))
            {
                return false;
            }

            if (!uri.DnsSafeHost.Equals(domain, StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }

            if (domain[0] == '*')
            {
                return false;
            }

            try
            {
                var parts = this.domainNormalizer.PartlyNormalizeDomainAndExtractFullyNormalizedParts(domain, out string partlyNormalizedDomain);

                var domainName = this.GetDomainFromParts(partlyNormalizedDomain, parts);
                if (domainName == null)
                {
                    return false;
                }

                return !domainName.TldRule.Equals(this.rootTldRule);
            }
            catch (ParseException)
            {
                return false;
            }
        }

        private void AddRules(IEnumerable<TldRule> tldRules)
        {
            this.domainDataStructure = this.domainDataStructure ?? new DomainDataStructure("*", this.rootTldRule);

            this.domainDataStructure.AddRules(tldRules);
        }

        private DomainInfo GetDomainFromParts(string domain, List<string> parts)
        {
            if (parts == null || parts.Count == 0 || parts.Any(x => x.Equals(string.Empty)))
            {
                throw new ParseException("Invalid domain part detected");
            }

            var structure = this.domainDataStructure;
            var matches = new List<TldRule>();
            this.FindMatches(parts, structure, matches);

            var sortedMatches = matches.OrderByDescending(x => x.Type == TldRuleType.WildcardException ? 1 : 0)
                .ThenByDescending(x => x.LabelCount)
                .ThenByDescending(x => x.Name);

            var winningRule = sortedMatches.FirstOrDefault();

            if (parts.Count == winningRule.LabelCount)
            {
                parts.Reverse();
                var tld = string.Join(".", parts);

                if (winningRule.Type == TldRuleType.Wildcard)
                {
                    if (tld.EndsWith(winningRule.Name.Substring(1)))
                    {
                        throw new ParseException("Domain is a TLD according publicsuffix", winningRule);
                    }
                }
                else
                {
                    if (tld.Equals(winningRule.Name))
                    {
                        throw new ParseException("Domain is a TLD according publicsuffix", winningRule);
                    }
                }

                throw new ParseException($"Unknown domain {domain}");
            }

            return new DomainInfo(domain, winningRule);
        }

        private void FindMatches(IEnumerable<string> parts, DomainDataStructure structure, List<TldRule> matches)
        {
            if (structure.TldRule != null)
            {
                matches.Add(structure.TldRule);
            }

            var part = parts.FirstOrDefault();
            if (string.IsNullOrEmpty(part))
            {
                return;
            }

            if (structure.Nested.TryGetValue(part, out DomainDataStructure foundStructure))
            {
                this.FindMatches(parts.Skip(1), foundStructure, matches);
            }

            if (structure.Nested.TryGetValue("*", out foundStructure))
            {
                this.FindMatches(parts.Skip(1), foundStructure, matches);
            }
        }
    }
}
#pragma warning restore VSTHRD002
#pragma warning restore SA1636
