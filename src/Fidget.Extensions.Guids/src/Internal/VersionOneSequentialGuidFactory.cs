using System;
using System.Security.Cryptography;

namespace Fidget.Extensions.Guids.Internal
{
    /// <summary>
    /// Factory for generating a sequential GUID that conforms to the structure of UUIDv1 with a node value
    /// taken from another GUID (typically random).
    /// </summary>

    class VersionOneSequentialGuidFactory : ISequentialGuidFactory
    {
        /// <summary>
        /// Constructs a factory for generating a sequential GUID that conforms to the specifications of UUIDv1.
        /// </summary>
        
        VersionOneSequentialGuidFactory() {}

        /// <summary>
        /// Gets a singleton instance of the type.
        /// </summary>

        public static ISequentialGuidFactory Instance { get; } = new VersionOneSequentialGuidFactory();

        /// <summary>
        /// Creates and returns a sequential identifier conforming to the RFC 4122 specification
        /// for a time-based UUID (https://tools.ietf.org/html/rfc4122#section-4.2)
        /// </summary>
        /// <param name="time">Number of 100 nanosecond intervals since 00:00:00.00, 15 October 1582.</param>
        /// <param name="clock">Value that represents an incremented clock sequence.</param>
        /// <param name="node">48-bit node identifier.</param>
        /// <remarks>
        /// The most-significant nibble (4 bits) of the time will be overwritten by the identifier version,
        /// causing a roll-over of values once every approximately two-thousand years.
        /// </remarks>
        
        public Guid Create( long time, int clock, byte[] node )
        {
            if ( node == null ) throw new ArgumentNullException( nameof( node ) );
            if ( node.Length != 6 ) throw new ArgumentException( $"{nameof(node)} must be a 6-byte array", nameof(node) );

            var timeBytes = BitConverter.GetBytes( time );
            var clockBytes = BitConverter.GetBytes( clock );
            
            if ( BitConverter.IsLittleEndian ) {
                Array.Reverse( clockBytes );
            }
            else {
                Array.Reverse( timeBytes );
            }

            var output = new byte[16];
            Array.Copy( timeBytes, 0, output, 0, 8 );
            Array.Copy( clockBytes, 2, output, 8, 2 );
            Array.Copy( node, 0, output, 10, 6 );
            
            // set version
            output[7] &= 0x1f;
            output[7] |= 0x10;

            // set variant
            output[8] &= 0xBF;
            output[8] |= 0x80;
            
            return new Guid( output );
        }
    }
}