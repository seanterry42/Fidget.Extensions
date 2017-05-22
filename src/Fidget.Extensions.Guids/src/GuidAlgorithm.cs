namespace Fidget.Extensions.Guids
{
    /// <summary>
    /// Format for GUID byte order.
    /// </summary>

    public enum GuidAlgorithm
    {
        /// <summary>
        /// Time-based (Version 1) UUID conforming to RFC 4122.
        /// </summary>
        /// <see cref="https://tools.ietf.org/html/rfc4122#section-4.2"/>
        /// <remarks>
        /// Values created with this algorithm will consist of a 60-bit value representing the current
        /// system time, a 14-bit sequential clock, and a 47-bit random node identifier.
        /// </remarks>

        UuidVersion1 = 1,

        /// <summary>
        /// Time-based sequential identifier optimized for use in SQL Server clustered indexes that
        /// can be transposed to UuidVersion1.
        /// </summary>

        SqlServer,
    }
}