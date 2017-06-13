using System;
using System.Security.Cryptography;

namespace Fidget.Extensions.Guids
{
    /// <summary>
    /// Utility methods for generating time-based GUID values.
    /// </summary>

    public static partial class TimeGuid
    {
        /// <summary>
        /// Gets the start of the Gregorian calenadar, used per RFC-4122 for UUID Version 1.
        /// </summary>

        public static long Epoch { get; } = new DateTime( 1582, 10, 15 ).Ticks;

        /// <summary>
        /// Returns the formatter to use for the given layout.
        /// </summary>
        /// <param name="layout">Layout of the identifier to create.</param>
        
        internal static ITimeGuidFormatter GetFormatter( TimeGuidLayout layout )
        {
            switch ( layout )
            {
                case TimeGuidLayout.SqlServer: return SqlServerFormatter.Instance;
                case TimeGuidLayout.Standard: return StandardFormatter.Instance;
                default: throw new NotImplementedException();
            }
        }

        /// <summary>
        /// Creates and returns a time-based GUID in the given layout.
        /// </summary>
        /// <param name="layout">Layout of the identifier.</param>
        
        public static Guid Create( TimeGuidLayout layout )
        {
            var formatter = GetFormatter( layout );
            var time = DateTime.UtcNow.Ticks - Epoch;
            var clockBytes = new byte[2];
            var node = new byte[6];

            using ( var rng = RandomNumberGenerator.Create() )
            {
                rng.GetBytes( clockBytes );
                rng.GetBytes( node );
            }

            // set multicast bit on random nodes
            node[0] |= 0x01;
            var clock = BitConverter.ToInt16( clockBytes, 0 );

            return formatter.Create( time, clock, node );
        }
    }
}