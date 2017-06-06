using System;

namespace Fidget.Extensions.Guids
{
    partial class TimeGuid
    {
        /// <summary>
        /// Formatter for creating standard time-based GUIDs as described in RFC 4122.
        /// </summary>
        
        internal class StandardFormatter : ITimeGuidFormatter
        {
            /// <summary>
            /// Singleton instance of the type.
            /// </summary>
            
            internal static readonly ITimeGuidFormatter Instance = new StandardFormatter();

            /// <summary>
            /// Creates and returns a time-based identifier.
            /// </summary>
            /// <param name="time">
            /// Number of 100 nanosecond intervals since 00:00:00.00, 15 October 1582.
            /// The most-significant nibble (4 bits) will be overwritten by the identifier version, 
            /// causing a roll-over of values once every approximately two-thousand years.
            /// </param>
            /// <param name="clock">
            /// Value whose least-significant 14 bits will be used as the clock sequence.
            /// According to spec, this value should be initialized to a random number for each node used.
            /// </param>
            /// <param name="node">
            /// 48-bit node identifier.
            /// If set to a random value, the mutlicast bit must be set.
            /// </param>

            public Guid Create( long time, short clock, byte[] node )
            {
                if ( node == null ) throw new ArgumentNullException( nameof( node ) );
                if ( node.Length != 6 ) throw new ArgumentException( $"{nameof( node )} must be a 6-byte array", nameof( node ) );

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
                Array.Copy( clockBytes, 0, output, 8, 2 );
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
}