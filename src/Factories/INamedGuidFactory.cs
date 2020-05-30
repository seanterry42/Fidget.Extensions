using System;

namespace Identifiable.Factories
{
    /// <summary>
    /// Defines a factory for creating named GUIDs.
    /// </summary>

    public interface INamedGuidFactory
    {
        /// <summary>
        /// Creates and return a name-based GUID that is not compatible with RFC 4122.
        /// </summary>
        /// <param name="algorithm">Hash algorithm to use for generating the name. SHA-1 is recommended.</param>
        /// <param name="namespace">Name space identifier.</param>
        /// <param name="name">Name for which to create a GUID.</param>

        [Obsolete( "Consider using Compute method instead." )]
        Guid Create( in NamedGuidAlgorithm algorithm, in Guid @namespace, string name );

        /// <summary>
        /// Computes and return a name-based GUID using the given algorithm as defined in https://tools.ietf.org/html/rfc4122#section-4.3.
        /// </summary>
        /// <param name="algorithm">Hash algorithm to use for generating the name. SHA-1 is recommended.</param>
        /// <param name="namespace">Name space identifier.</param>
        /// <param name="name">Name for which to create a GUID.</param>

        Guid Compute( in NamedGuidAlgorithm algorithm, in Guid @namespace, string name );
    }
}
