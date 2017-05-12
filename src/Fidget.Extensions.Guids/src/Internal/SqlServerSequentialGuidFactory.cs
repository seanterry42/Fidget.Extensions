using System;

namespace Fidget.Extensions.Guids.Internal
{
    /// <summary>
    /// Provider for creating sequential GUIDs for SQL Server.
    /// </summary>

    class SqlServerSequentialGuidFactory : ISequentialGuidFactory
    {
        /// <summary>
        /// Constructs a provider for creating sequential GUIDs for SQL Server.
        /// </summary>
        
        SqlServerSequentialGuidFactory() {}

        /// <summary>
        /// Gets a singleton instance of the type.
        /// </summary>
        
        public static ISequentialGuidFactory Instance { get; } = new SqlServerSequentialGuidFactory();

        /// <summary>
        /// Writes the given sequence value into the highest-order bytes of the given source Guid.
        /// </summary>
        /// <param name="source">
        /// Source Guid to sequentialize.
        /// Version 3, 4, or 5 Guids are recommended to ensure sufficient entropy to avoid collisions.</param>
        /// <param name="sequence">Sequence number to write into the highest-order bytes.</param>
        /// <returns>
        /// The source Guid with the sequence value written into the highest-order bytes.
        /// The version of the Guid will not be changed, but the variant of the new value will be
        /// set to zero to remove these generated values from the same collision domain.</returns>
        
        public Guid Create( Guid source, long sequence )
        {
            // ensure a positive value since the sign bit will be lost
            if ( sequence < 0 ) throw new ArgumentOutOfRangeException( nameof(sequence), $"{nameof(sequence)} must be a positive value" );

            // shift the high order bit of the sequence to apply the zero variant
            const long mask = 0x7fff;
            sequence = unchecked( ( (sequence & ~mask ) << 1 ) + ( sequence & mask ) );
            
            // correct for endianness
            var bytes = BitConverter.GetBytes( sequence );
            Array.Reverse( bytes, 0, 2 );
            Array.Reverse( bytes, 2, 6 );

            var output = source.ToByteArray();
            Array.Copy( bytes, 0, output, 8, 8 );

            return new Guid( output );
        }
    }
}