using System;

namespace Fidget.Extensions.Guids
{
    /// <summary>
    /// Defines a formatter for time-based GUIDs.
    /// </summary>

    public interface ITimeGuidFormatter
    {
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
        
        Guid Create( long time, short clock, byte[] node );
    }
}