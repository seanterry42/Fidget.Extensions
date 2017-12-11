/*  Copyright 2017 Sean Terry

    Licensed under the Apache License, Version 2.0 (the "License");
    you may not use this file except in compliance with the License.
    You may obtain a copy of the License at

        http://www.apache.org/licenses/LICENSE-2.0

    Unless required by applicable law or agreed to in writing, software
    distributed under the License is distributed on an "AS IS" BASIS,
    WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
    See the License for the specific language governing permissions and
    limitations under the License.
*/

using Identifiable.Factories;
using System;

namespace Identifiable
{
    /// <summary>
    /// Creates GUIDs based on a namespace and name.
    /// </summary>

    public static class NamedGuid
    {
        /// <summary>
        /// Factory to use for generating named identifiers.
        /// </summary>
        
        static readonly INamedGuidFactory factory = NamedGuidFactory.Instance;

        /// <summary>
        /// Creates and return a name-based GUID using the given algorithm as defined in https://tools.ietf.org/html/rfc4122#section-4.3.
        /// </summary>
        /// <param name="algorithm">Hash algorithm to use for generating the name. SHA-1 is recommended.</param>
        /// <param name="namespace">Name space identifier.</param>
        /// <param name="name">Name for which to create a GUID.</param>
        
        public static Guid Create( NamedGuidAlgorithm algorithm, Guid @namespace, string name ) => 
            factory.Create( algorithm, @namespace, name );
    }
}