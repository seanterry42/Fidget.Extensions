using Identifiable.Formatters;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;

namespace Identifiable.Factories
{
    /// <summary>
    /// Factory for generating time-based GUIDs.
    /// </summary>

    public class TimeGuidFactory : ITimeGuidFactory
    {
        /// <summary>
        /// Gets the default instance of the type.
        /// </summary>

        public static ITimeGuidFactory Instance { get; } = new TimeGuidFactory();

        /// <summary>
        /// Gets the start of the Gregorian calenadar, used per RFC-4122 for UUID Version 1.
        /// </summary>

        public static long Epoch { get; } = new DateTime( 1582, 10, 15 ).Ticks;

        /// <summary>
        /// Collection of formatters, indexed by layout option.
        /// </summary>

        readonly IReadOnlyDictionary<TimeGuidLayout, ITimeGuidFormatter> formatters = new Dictionary<TimeGuidLayout, ITimeGuidFormatter>
        {
            { TimeGuidLayout.Standard, StandardFormatter.Instance },
            { TimeGuidLayout.SqlServer, SqlServerFormatter.Instance },
        };

        /// <summary>
        /// Creates and returns a time-based GUID in the given layout.
        /// </summary>
        /// <param name="layout">Layout of the identifier.</param>

        public Guid Create( TimeGuidLayout layout )
        {
            if ( !formatters.TryGetValue( layout, out ITimeGuidFormatter formatter ) )
                throw new NotImplementedException();

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