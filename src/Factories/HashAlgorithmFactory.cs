using System;
using System.Security.Cryptography;

namespace Identifiable.Factories
{
    /// <summary>
    /// Factory for generating hash algorithms for name-based identifiers.
    /// </summary>

    public class HashAlgorithmFactory : IHashAlgorithmFactory
    {
        /// <summary>
        /// Default instance of the type.
        /// </summary>

        internal static IHashAlgorithmFactory Instance { get; } = new HashAlgorithmFactory();

        /// <summary>
        /// Returns the hash algorithm implementation to use for generating the identifier.
        /// </summary>
        /// <param name="algorithm">Algorithm to return.</param>
        /// <param name="version">GUID version field.</param>

        public HashAlgorithm Create( in NamedGuidAlgorithm algorithm, out byte version )
        {
            switch ( algorithm )
            {
                case NamedGuidAlgorithm.MD5:
                    version = StandardGuidVersion.UUIDv3;
                    return MD5.Create();
                case NamedGuidAlgorithm.SHA1:
                    version = StandardGuidVersion.UUIDv5;
                    return SHA1.Create();
                default: throw new NotImplementedException();
            }
        }
    }
}