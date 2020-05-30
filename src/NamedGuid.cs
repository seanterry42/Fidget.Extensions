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
        /// Computes and return a name-based GUID using the given algorithm as defined in https://tools.ietf.org/html/rfc4122#section-4.3.
        /// </summary>
        /// <param name="algorithm">Hash algorithm to use for generating the name. SHA-1 is recommended.</param>
        /// <param name="namespace">Name space identifier.</param>
        /// <param name="name">Name for which to create a GUID.</param>

        public static Guid Compute( NamedGuidAlgorithm algorithm, Guid @namespace, string name ) =>
            factory.Compute( algorithm, @namespace, name );
    }
}