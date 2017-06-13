using System;
using System.Security.Cryptography;
using System.Text;

namespace Fidget.Extensions.Guids
{
    /// <summary>
    /// Utility methods for generating name-based GUIDs.
    /// </summary>

    public static partial class NamedGuid
    {
        /// <summary>
        /// Returns the hash algorithm implementation to use for generating the identifier.
        /// </summary>
        /// <param name="algorithm">Algorithm to return.</param>
        
        internal static HashAlgorithm GetAlgorithm( NamedGuidAlgorithm algorithm )
        {
            switch ( algorithm )
            {
                case NamedGuidAlgorithm.MD5:
                    return MD5.Create();
                case NamedGuidAlgorithm.SHA1:
                    return SHA1.Create();
                default: throw new NotImplementedException();
            }
        }

        /// <summary>
        /// Returns the GUID version for the specified algorithm.
        /// </summary>
        /// <param name="algorithm">Algorithm whose version to return.</param>
        
        internal static byte GetVersion( NamedGuidAlgorithm algorithm )
        {
            switch ( algorithm )
            {
                case NamedGuidAlgorithm.MD5:
                    return 0x30;
                case NamedGuidAlgorithm.SHA1:
                    return 0x50;
                default: throw new NotImplementedException();
            }
        }

        /// <summary>
        /// Creates and return a name-based GUID using the given algorithm as defined in https://tools.ietf.org/html/rfc4122#section-4.3.
        /// </summary>
        /// <param name="algorithm">Hash algorithm to use for generating the name. SHA-1 is recommended.</param>
        /// <param name="namespace">Name space identifier.</param>
        /// <param name="name">Name for which to create a GUID.</param>
        
        public static Guid Create( NamedGuidAlgorithm algorithm, Guid @namespace, string name )
        {
            if ( name == null ) throw new ArgumentNullException( nameof( name ) );

            var version = GetVersion( algorithm );
            var encoded = Encoding.Unicode.GetBytes( name );
            var bytes = @namespace.ToByteArray();
            Array.Resize( ref bytes, encoded.Length + 16 );
            Array.Copy( encoded, 0, bytes, 16, encoded.Length );

            using ( var hasher = GetAlgorithm( algorithm ) )
            {
                var hash = hasher.ComputeHash( bytes );
                Array.Resize( ref hash, 16 );

                // set version
                hash[7] &= 0x0f;
                hash[7] |= version;

                // set variant
                hash[8] &= 0xBF;
                hash[8] |= 0x80;

                return new Guid( hash );
            }
        }
    }
}