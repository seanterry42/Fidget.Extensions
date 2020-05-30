using System.Security.Cryptography;

namespace Identifiable.Factories
{
    /// <summary>
    /// Defines a factory for generating hash algorithms for name-based identifiers.
    /// </summary>

    public interface IHashAlgorithmFactory
    {
        /// <summary>
        /// Returns the hash algorithm implementation to use for generating the identifier.
        /// </summary>
        /// <param name="algorithm">Algorithm to return.</param>
        /// <param name="version">GUID version field.</param>

        HashAlgorithm Create( in NamedGuidAlgorithm algorithm, out byte version );
    }
}