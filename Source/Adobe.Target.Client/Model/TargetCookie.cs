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
namespace Adobe.Target.Client.Model
{
    using System;

    /// <summary>
    /// TargetCookie
    /// </summary>
    public sealed class TargetCookie : IEquatable<TargetCookie>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TargetCookie"/> class.
        /// </summary>
        /// <param name="name">Name</param>
        /// <param name="value">Value</param>
        /// <param name="maxAge">Max-Age</param>
        public TargetCookie(string name, string value, int maxAge)
        {
            this.Name = name;
            this.Value = value;
            this.MaxAge = maxAge;
        }

        /// <summary>
        /// Name
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Value
        /// </summary>
        public string Value { get; private set; }

        /// <summary>
        /// Max-Age
        /// </summary>
        public int MaxAge { get; private set; }

        /// <inheritdoc />
        public bool Equals(TargetCookie other)
        {
            if (ReferenceEquals(null, other))
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return true;
            }

            return this.Name == other.Name && this.Value == other.Value && this.MaxAge == other.MaxAge;
        }

        /// <inheritdoc />
        public override bool Equals(object obj) => ReferenceEquals(this, obj) || (obj is TargetCookie other && this.Equals(other));

        /// <inheritdoc />
        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = this.Name != null ? this.Name.GetHashCode() : 0;
                hashCode = (hashCode * 397) ^ (this.Value != null ? this.Value.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ this.MaxAge;
                return hashCode;
            }
        }
    }
}
