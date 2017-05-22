using Fidget.Extensions.Guids.Internal;
using System;
using System.Security.Cryptography;
using System.Threading;

namespace Fidget.Extensions.Guids
{
    /// <summary>
    /// Extension methods for creating sequential GUIDs.
    /// These are similar to COMB Guids created by Jimmy Nilsson, but using a higher-fidelity time value.
    /// </summary>

    public static class SequentialGuid
    {
        /// <summary>
        /// Gets the start of the Gregorian calenadar, used per RFC-4122 for UUID Version 1.
        /// </summary>

        public static long Epoch { get; } = new DateTime( 1582, 10, 15 ).Ticks;

        /// <summary>
        /// Internal clock sequence, initialized to a random value.
        /// </summary>
        
        static int Clock = new Random().Next();
        
        /// <summary>
        /// Random number generator for node values.
        /// </summary>
        
        static readonly RandomNumberGenerator Randomizer = RandomNumberGenerator.Create();
        
        /// <summary>
        /// Returns the factory for the specified format.
        /// </summary>
        /// <param name="format">System for which to bias the Guid.</param>
        
        public static ISequentialGuidFactory GetFactory( this GuidAlgorithm format )
        {
            switch ( format )
            {
                case GuidAlgorithm.SqlServer:
                    return SqlServerSequentialGuidFactory.Instance;
                case GuidAlgorithm.UuidVersion1:
                    return VersionOneSequentialGuidFactory.Instance;
                default:
                    throw new NotImplementedException();
            }
        }
        
        /// <summary>
        /// Creates and returns a sequential (Version 1) Guid.
        /// </summary>
        /// <param name="format">Format of the resulting identifier.</param>
        
        public static Guid Create( GuidAlgorithm format )
        {
            var time = DateTime.UtcNow.Ticks;
            var clock = Interlocked.Increment( ref Clock );
            var node = new byte[6];

            // node value is random with multicast bit set
            // per https://tools.ietf.org/html/rfc4122#section-4.5
            Randomizer.GetBytes( node );
            node[0] |= 0x01;

            return GetFactory( format )
                .Create( time, clock, node );
        }
    }
}