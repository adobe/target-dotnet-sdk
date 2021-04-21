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
namespace Adobe.Target.Client.OnDevice
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text.RegularExpressions;
    using Adobe.Target.Client.Model.OnDevice;
    using Adobe.Target.Delivery.Model;
    using Newtonsoft.Json.Linq;
    using Action = Adobe.Target.Delivery.Model.Action;

    internal sealed class CampaignMacroReplacer
    {
        private const string Name = "name";
        private const string Index = "index";
        private const string MacroPatternRegexString = @"\$\{([a-zA-Z0-9_.]*?)\}";

        private static readonly IReadOnlyDictionary<string, string> MacroNameReplacements =
            new Dictionary<string, string>
            {
                { "campaign", "activity" },
                { "recipe", "experience" },
            };

        private static readonly IReadOnlyList<string> MacroNameRemovals = new List<string> { "mbox" };

        private static readonly Regex MacroPatternRegex = new (
            MacroPatternRegexString,
            RegexOptions.Compiled | RegexOptions.Multiline | RegexOptions.IgnoreCase);

        private readonly OnDeviceDecisioningRule rule;
        private readonly IDictionary<string, object> consequence;
        private readonly IDictionary<string, string> requestParameters;
        private IDictionary<string, object> mboxDetails = new Dictionary<string, object>();

        public CampaignMacroReplacer(OnDeviceDecisioningRule rule, IDictionary<string, object> consequence, RequestDetailsUnion details)
        {
            this.rule = rule;
            this.consequence = consequence;
            this.requestParameters = details.GetRequestDetails().Parameters ?? new Dictionary<string, string>();
            _ = details.Match<object>(
                _ => _,
                request =>
                {
                    this.mboxDetails.Add(Name, request.Name);
                    this.mboxDetails.Add(Index, request.Index);
                    return null;
                },
                _ => _);
        }

        internal IList<Option> GetOptions()
        {
            var result = new List<Option>();
            var options = (IList<Option>)this.consequence[DecisioningDetailsExecutor.Options];
            foreach (var option in options)
            {
                var content = option.Content;
                if (option.Type == OptionType.Html && content is string stringContent)
                {
                    content = this.AddCampaignMacroValues(stringContent);
                }

                if (option.Type == OptionType.Actions && this.GetActionsContent(content, out var actions))
                {
                    content = actions;
                }

                result.Add(new Option(option.Type, content, option.EventToken, option.ResponseTokens));
            }

            return result;
        }

        private static string SanitizedMacroKey(string macroKey)
        {
            if (macroKey == "mbox.name")
            {
                macroKey = "location.name";
            }

            foreach (var replacement in MacroNameReplacements)
            {
                macroKey = macroKey.Replace(replacement.Key, replacement.Value);
            }

            var keySegments = macroKey.Split('.');
            var length = keySegments.Length;
            if (length > 2)
            {
                keySegments = new[] { keySegments[length - 2], keySegments[length - 1] };
            }

            keySegments = keySegments
                .Where(part => !MacroNameRemovals.Contains(part))
                .ToArray();

            return string.Join(".", keySegments);
        }

        private bool GetActionsContent(object actionsObject, out IList<Action> result)
        {
            if (actionsObject is not JArray actionsContent)
            {
                result = null;
                return false;
            }

            var actions = actionsContent.ToObject<IList<Action>>();
            if (actions == null)
            {
                result = null;
                return false;
            }

            foreach (var action in actions)
            {
                if (action.Content is string actionContent)
                {
                    action.Content = this.AddCampaignMacroValues(actionContent);
                }
            }

            result = actions;
            return true;
        }

        private string AddCampaignMacroValues(string content)
        {
            return MacroPatternRegex.Replace(content, match =>
            {
                var matchValue = match.Groups[1].Value;
                var macroKey = SanitizedMacroKey(matchValue);
                return this.GetMacroValue(macroKey, "${" + matchValue + "}");
            });
        }

        private string GetMacroValue(string key, string defaultValue)
        {
            var meta = this.rule.Meta;
            if (meta.ContainsKey(key))
            {
                return (string)Convert.ChangeType(meta[key], TypeCode.String);
            }

            if (this.mboxDetails.ContainsKey(key))
            {
                return (string)Convert.ChangeType(this.mboxDetails[key], TypeCode.String);
            }

            return this.requestParameters.ContainsKey(key) ? this.requestParameters[key] : defaultValue;
        }
    }
}
