using System;

namespace Fidget.Extensions.Guids
{
    /// <summary>
    /// Defines a factory for generating sequential Guid values.
    /// </summary>

    public interface ISequentialGuidFactory
    {
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

        Guid Create( long time, int clock, byte[] node );
    }
}