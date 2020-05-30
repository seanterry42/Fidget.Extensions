using System;

namespace Identifiable
{
    /// <summary>
    /// Utility methods for generating time-based GUID values.
    /// </summary>

    public static class TimeGuid
    {
        /// <summary>
        /// Creates and returns a time-based GUID in the given layout.
        /// </summary>
        /// <param name="layout">Layout of the identifier.</param>

        public static Guid Create( TimeGuidLayout layout ) =>
            Factories.TimeGuidFactory.Instance.Create( layout );
    }
}