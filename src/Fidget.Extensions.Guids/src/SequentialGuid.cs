using Fidget.Extensions.Guids.Internal;
using System;
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
        /// Current sequence value.
        /// </summary>
        
        static long Sequence = 0;

        /// <summary>
        /// Returns the next sequence number based on the current system time.
        /// </summary>
        
        public static long GetNextSequence()
        {
            long comparand, next;

            do
            {
                // the next sequence value is the greater of the current system clock
                // or the next increment if the system clock has not advanced
                comparand = Sequence;
                next = Math.Max( comparand + 1, DateTime.UtcNow.Ticks - Epoch );
            }
            while ( comparand != Interlocked.CompareExchange( ref Sequence, next, comparand ) );

            return next;
        }

        /// <summary>
        /// Returns the factory for the specified format.
        /// </summary>
        /// <param name="format">System for which to bias the Guid.</param>
        
        static ISequentialGuidFactory GetFactory( this GuidFormat format )
        {
            switch ( format )
            {
                case GuidFormat.SqlServer:
                    return SqlServerSequentialGuidFactory.Instance;
                default:
                    throw new NotImplementedException();
            }
        }

        /// <summary>
        /// Embeds the given sequence value into the highest-order bytes of the given source GUID.
        /// </summary>
        /// <param name="source">
        /// Source GUID whose high-order bits to overwrite.
        /// Version 3, 4, or 5 GUIDs are recommended to ensure sufficient entropy in the lower-order bits.</param>
        /// <param name="sequence">Sequence value to write into the highest-order bits. Defaults to the current system clock.</param>
        /// <param name="format">System for which to bias the Guid.</param>
        /// <returns>
        /// The source Guid with the sequence value written into the highest-order bytes.
        /// The version of the Guid will not be changed, but the variant of the new value will be
        /// set to zero to remove these generated values from the same collision domain.</returns>
        
        public static Guid Sequentialize( this Guid source, GuidFormat format, long? sequence = null ) => format
            .GetFactory()
            .Create( source, sequence ?? GetNextSequence() );
    }
}