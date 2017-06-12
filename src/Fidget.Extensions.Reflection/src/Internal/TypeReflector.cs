﻿using System;
using System.Collections.Generic;
using System.Linq;
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
        /// Type being reflected.
        /// </summary>
        
        readonly Type Type = typeof(T);

        /// <summary>
        /// Collection of properties indexed by name.
        /// </summary>

        readonly IReadOnlyDictionary<string, IPropertyReflector<T>> Index;

        /// <summary>
        /// Collection of properties on the type that are arrays.
        /// </summary>

        readonly IEnumerable<IPropertyReflector<T>> Arrays;

        /// <summary>
        /// Constructs a fast reflection provider for a type.
        /// </summary>

        TypeReflector() 
        {
            // creates and returns a reflector for the given property
            IPropertyReflector<T> createReflector( PropertyInfo propertyInfo )
            {
                var type = typeof( PropertyReflector<,> )
                    .MakeGenericType( propertyInfo.DeclaringType, propertyInfo.PropertyType );

                return (IPropertyReflector<T>)Activator
                    .CreateInstance( type, propertyInfo );
            }
            
            Index = Type
                .GetProperties()
                .Select( _ => createReflector( _ ) )
                .ToDictionary( _ => _.PropertyInfo.Name );

            Arrays = Index
                .Values
                .Where( _=> _.IsArray )
                .Where( _=> !_.IsReadOnly )
                .ToArray();
        }

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

            var clone = (T)CloneMethod.Invoke( source );

            // perform a structural copy of any array properties
            foreach ( var property in Arrays )
            {
                if ( property.IsArray && !property.IsReadOnly )
                {
                    property.Copy( source, clone );
                }
            }

            return clone;
        }
    }
}