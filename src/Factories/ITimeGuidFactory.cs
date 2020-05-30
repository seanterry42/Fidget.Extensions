using System;

namespace Identifiable.Factories
{
    /// <summary>
    /// Defines a factory for generating time-based GUIDs.
    /// </summary>

    public interface ITimeGuidFactory
    {
        /// <summary>
        /// Creates and returns a time-based GUID in the given layout.
        /// </summary>
        /// <param name="layout">Layout of the identifier.</param>

        Guid Create( TimeGuidLayout layout );
    }
}