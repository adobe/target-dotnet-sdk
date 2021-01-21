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
    using System.Threading.Tasks;
    using Microsoft.Extensions.DependencyInjection;
    using Adobe.Target.Client;

    internal class Program
    {
        public static async Task Main(string[] args)
        {
            var services = new ServiceCollection();
            ConfigureServices(services);
            var serviceProvider = services.BuildServiceProvider();

            await serviceProvider.GetService<App>().RunAsync(args);
        }

        private static void ConfigureServices(IServiceCollection services)
        {
            services.AddTargetLibrary();
            services.AddLogging();
            services.AddTransient<App>();
        }
    }
}
