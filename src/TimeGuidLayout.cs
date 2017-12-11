namespace Identifiable
{
    /// <summary>
    /// Layouts for time-based GUIDs.
    /// </summary>

    public enum TimeGuidLayout
    {
        /// <summary>
        /// GUID layout as described in RFC 4122 Section 4.1.2.
        /// https://tools.ietf.org/html/rfc4122#section-4.1.2
        /// </summary>
        
        Standard = 1,

        /// <summary>
        /// Layout that is optimized for SQL Server clustered indexes.
        /// Identifiers in this layout can be transposed to/from those in the standard layout.
        /// </summary>
        /// <remarks>
        /// Identifiers in this layout will identify as Variant 0 due to transposition.
        /// </remarks>
        
        SqlServer,
    }
}