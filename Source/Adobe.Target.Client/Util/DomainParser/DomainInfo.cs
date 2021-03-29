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
    using System.Linq;

    internal class DomainInfo
    {
        public DomainInfo()
        {
        }

        public DomainInfo(string domain, TldRule tldRule)
        {
            if (string.IsNullOrEmpty(domain))
            {
                return;
            }

            if (tldRule == null)
            {
                return;
            }

            var domainParts = domain.Split('.').Reverse().ToList();
            var ruleParts = tldRule.Name.Split('.').Skip(tldRule.Type == TldRuleType.WildcardException ? 1 : 0).Reverse().ToList();
            var tld = string.Join(".", domainParts.Take(ruleParts.Count).Reverse());
            var registrableDomain = string.Join(".", domainParts.Take(ruleParts.Count + 1).Reverse());

            if (domain.Equals(tld))
            {
                return;
            }

            this.TldRule = tldRule;
            this.Hostname = domain;
            this.Tld = tld;
            this.RegistrableDomain = registrableDomain;

            this.Domain = domainParts.Skip(ruleParts.Count).FirstOrDefault();
            var subDomain = string.Join(".", domainParts.Skip(ruleParts.Count + 1).Reverse());
            this.SubDomain = string.IsNullOrEmpty(subDomain) ? null : subDomain;
        }

        /// <summary>
        /// Domain Name without the TLD<para />
        /// e.g. microsoft, google
        /// </summary>
        public string Domain { get; set; }

        /// <summary>
        /// The TLD<para />
        /// e.g. com, net, de, co.uk
        /// </summary>
        public string Tld { get; set; }

        /// <summary>
        /// The Sub Domain<para />
        /// e.g. www, mail
        /// </summary>
        public string SubDomain { get; set; }

        /// <summary>
        /// The Registrable Domain<para />
        /// e.g. microsoft.com, amazon.co.uk
        /// </summary>
        public string RegistrableDomain { get; private set; }

        /// <summary>
        /// Fully qualified hostname (FQDN)
        /// </summary>
        public string Hostname { get; private set; }

        /// <summary>
        /// The matching public suffix Rule
        /// </summary>
        public TldRule TldRule { get; private set; }
    }
}
#pragma warning restore SA1636
