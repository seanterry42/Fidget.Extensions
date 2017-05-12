using System;

namespace Fidget.Extensions.Guids
{
    /// <summary>
    /// Defines a factory for generating sequential Guid values.
    /// </summary>

    public interface ISequentialGuidFactory
    {
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
        
        Guid Create( Guid source, long sequence );
    }
}