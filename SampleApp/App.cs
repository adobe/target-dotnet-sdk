/*
 * Copyright 2020 Adobe. All rights reserved.
 * This file is licensed to you under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License. You may obtain a copy
 * of the License at http://www.apache.org/licenses/LICENSE-2.0
 * Unless required by applicable law or agreed to in writing, software distributed under
 * the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR REPRESENTATIONS
 * OF ANY KIND, either express or implied. See the License for the specific language
 * governing permissions and limitations under the License.
 */
namespace SampleApp
{
    using System;
    using System.Threading.Tasks;
    using Adobe.Target.Client;
    using Microsoft.Extensions.Logging;

    public class App
    {
        private readonly ITargetClient targetClient;
        private readonly ILogger<App> logger;

        public App(ITargetClient targetClient, ILogger<App> logger)
        {
            this.targetClient = targetClient;
            this.logger = logger;
        }

        public async Task RunAsync(string[] args)
        {
            logger.LogInformation("Starting ...");

            Console.WriteLine("Target init");
            targetClient.Initialize();
            Console.WriteLine("Test: " + targetClient.TestMe());

            logger.LogInformation("Done.");

            await Task.CompletedTask;
        }
    }
}
