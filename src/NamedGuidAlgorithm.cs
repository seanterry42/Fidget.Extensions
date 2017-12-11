namespace Identifiable
{
    /// <summary>
    /// Enumeration of named GUID algorithms.
    /// </summary>

    public enum NamedGuidAlgorithm
    {
        /// <summary>
        /// Generates a name-based GUID using the MD5 algorithm.
        /// </summary>
        
        MD5 = 3,

        /// <summary>
        /// Generates a name-based GUID using the SHA-1 algorithm (preferred).
        /// </summary>
        
        SHA1 = 5,
    }
}