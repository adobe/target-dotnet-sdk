/*
Copyright 2020 Adobe. All rights reserved.
This file is licensed to you under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License. You may obtain a copy
of the License at http://www.apache.org/licenses/LICENSE-2.0
Unless required by applicable law or agreed to in writing, software distributed under
the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR REPRESENTATIONS
OF ANY KIND, either express or implied. See the License for the specific language
governing permissions and limitations under the License.
*/
namespace Adobe.Target.Client
{
    using System;

    /// <summary>
    /// The main TargetClient class.
    /// Contains methods for creating and using TargetClient SDK.
    /// </summary>
    public class TargetClient
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TargetClient"/> class.
        /// </summary>
        public TargetClient()
        {
            Console.WriteLine("Target Client");
        }

        /// <summary>
        /// Test method.
        /// </summary>
        /// <returns>
        /// A test value.
        /// </returns>
        public int TestMe()
        {
            return 1;
        }
    }
}
