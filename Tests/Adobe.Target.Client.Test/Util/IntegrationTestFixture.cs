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
namespace Adobe.Target.Client.Test.Util
{
    using System.Collections.Generic;
    using System.Dynamic;
    using System.IO;
    using Newtonsoft.Json;

    public class IntegrationTestFixture
    {
        private const string ArtifactsDir = "Artifacts";
        private const string TestsDir = "Models";
        private const string JsonFiles = "*.json";

        public IntegrationTestFixture()
        {
            this.Artifacts = LoadArtifacts();
            this.Tests = LoadTests();
        }

        public IDictionary<string, string> Artifacts { get; }

        public IDictionary<string, ExpandoObject> Tests { get; }

        private static IDictionary<string, string> LoadArtifacts()
        {
            var result = new Dictionary<string, string>();
            var directoryInfo = new DirectoryInfo(ArtifactsDir);
            foreach (var file in directoryInfo.GetFiles(JsonFiles))
            {
                var contents = File.ReadAllText($"{ArtifactsDir}/{file.Name}");
                result.Add(file.Name.Substring(0, file.Name.IndexOf('.')), contents);
            }

            return result;
        }

        private static IDictionary<string, ExpandoObject> LoadTests()
        {
            var result = new Dictionary<string, ExpandoObject>();
            var directoryInfo = new DirectoryInfo(TestsDir);
            foreach (var file in directoryInfo.GetFiles(JsonFiles))
            {
                var contents = File.ReadAllText($"{TestsDir}/{file.Name}");
                dynamic deserialized = JsonConvert.DeserializeObject<ExpandoObject>(contents);
                result.Add(file.Name.Substring(0, file.Name.IndexOf('.')), deserialized);
            }

            return result;
        }
    }
}
