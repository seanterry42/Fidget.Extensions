using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;

namespace Fidget.Extensions.Reflection.Internal
{
    /// <summary>
    /// Shared elements for type reflectors.
    /// </summary>
    
    abstract class TypeReflector
    {
        /// <summary>
        /// Defines a delegate for making a shallow copy of the given object instance.
        /// </summary>
        /// <param name="instance">Instance to be cloned.</param>
        /// <returns>A shallow copy of the given instance.</returns>

        protected delegate object CloneMethodDelegate( object instance );

        /// <summary>
        /// Unbound clone method delegate.
        /// </summary>

        protected static readonly CloneMethodDelegate CloneMethod = (CloneMethodDelegate)typeof( object )
            .GetMethod( nameof( MemberwiseClone ), BindingFlags.Instance | BindingFlags.NonPublic )
            .CreateDelegate( typeof( CloneMethodDelegate ) );
    }

    /// <summary>
    /// Fast reflection provider for a type.
    /// </summary>
    /// <typeparam name="T">Type to reflect.</typeparam>
    
    class TypeReflector<T> : TypeReflector, ITypeReflector<T>
    {
        /// <summary>
        /// Constructs a fast reflection provider for a type.
        /// </summary>
        
        TypeReflector() {}

        /// <summary>
        /// Singleton instance of the type.
        /// </summary>
        
        public static ITypeReflector<T> Instance { get; } = new TypeReflector<T>();

        /// <summary>
        /// Creates and returns a shallow copy of the source instance.
        /// </summary>
        /// <param name="source">Source instance to clone.</param>
        
        public T Clone( T source )
        {
            if ( source == null ) throw new ArgumentNullException( nameof( source ) );

            return (T)CloneMethod.Invoke( source );
        }
    }
}