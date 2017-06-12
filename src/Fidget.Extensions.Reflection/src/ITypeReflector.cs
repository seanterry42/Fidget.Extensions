using System;
using System.Collections.Generic;
using System.Text;

namespace Fidget.Extensions.Reflection
{
    /// <summary>
    /// Defines a fast reflection provider for a type.
    /// </summary>
    /// <typeparam name="T">Type to reflect.</typeparam>
    
    public interface ITypeReflector<T>
    {
        /// <summary>
        /// Creates and returns a shallow copy of the source instance.
        /// </summary>
        /// <param name="source">Source instance to clone.</param>

        T Clone( T source );
    }
}