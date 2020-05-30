﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Identifiable.Factories
{
    /// <summary>
    /// Factory for creating named GUIDs.
    /// </summary>

    public class NamedGuidFactory : INamedGuidFactory
    {
        readonly IHashAlgorithmFactory algorithmFactory;

        /// <summary>
        /// Constructs a factory for creating named GUIDs.
        /// </summary>
        /// <param name="algorithmFactory">Hash algorithm factory.</param>

        public NamedGuidFactory( IHashAlgorithmFactory algorithmFactory )
        {
            this.algorithmFactory = algorithmFactory ?? throw new ArgumentNullException( nameof( algorithmFactory ) );
        }

        /// <summary>
        /// Gets the default instance of the type.
        /// </summary>

        internal static INamedGuidFactory Instance { get; } = new NamedGuidFactory( HashAlgorithmFactory.Instance );

        /// <summary>
        /// Collection of named GUID versions indexed by algorithm.
        /// </summary>

        readonly IReadOnlyDictionary<NamedGuidAlgorithm, byte> versions = new Dictionary<NamedGuidAlgorithm, byte>
        {
            { NamedGuidAlgorithm.MD5, 0x30 },
            { NamedGuidAlgorithm.SHA1, 0x50 },
        };

        /// <summary>
        /// Computes and return a name-based GUID using the given algorithm as defined in https://tools.ietf.org/html/rfc4122#section-4.3.
        /// </summary>
        /// <param name="algorithm">Hash algorithm to use for generating the name. SHA-1 is recommended.</param>
        /// <param name="namespace">Name space identifier.</param>
        /// <param name="name">Name for which to create a GUID.</param>

        public Guid Compute( in NamedGuidAlgorithm algorithm, in Guid @namespace, string name )
        {
            if ( name == null ) throw new ArgumentNullException( nameof( name ) );

            // function to put guid bytes into network order
            static void correctEndianness( byte[] data )
            {
                /* from https://msdn.microsoft.com/en-us/library/windows/desktop/aa373931(v=vs.85).aspx
                    typedef struct _GUID {
                      DWORD Data1;
                      WORD  Data2;
                      WORD  Data3;
                      BYTE  Data4[8]; // already big-endian
                    } GUID;
                */

                if ( BitConverter.IsLittleEndian )
                {
                    Array.Reverse( data, 0, 4 );
                    Array.Reverse( data, 4, 2 );
                    Array.Reverse( data, 6, 2 );
                }
            }

            var nameBytes = Encoding.UTF8.GetBytes( name );
            var namespaceBytes = @namespace.ToByteArray();
            correctEndianness( namespaceBytes );

            using ( var hasher = algorithmFactory.Create( algorithm, out byte version ) )
            {
                hasher.TransformBlock( namespaceBytes, 0, namespaceBytes.Length, null, 0 );
                hasher.TransformFinalBlock( nameBytes, 0, nameBytes.Length );
                var hash = hasher.Hash;

                Array.Resize( ref hash, 16 );
                correctEndianness( hash );

                // set version
                hash[7] &= 0b00001111;
                hash[7] |= version;

                // set variant - turn on first bit, turn off second bit
                hash[8] |= 0b10000000;
                hash[8] &= 0b10111111;

                return new Guid( hash );
            }
        }
    }
}