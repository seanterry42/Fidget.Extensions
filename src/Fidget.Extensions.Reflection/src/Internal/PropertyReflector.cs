using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Fidget.Extensions.Reflection.Internal
{
    /// <summary>
    /// Defines a fast reflection provider for a property.
    /// </summary>
    /// <typeparam name="T">Declaring type of the property.</typeparam>
    
    abstract class PropertyReflector<T> : IPropertyReflector<T>
    {
        /// <summary>
        /// Gets the property being reflected.
        /// </summary>
        
        public PropertyInfo PropertyInfo { get; }

        /// <summary>
        /// Gets whether the property is an array type.
        /// </summary>

        public bool IsArray { get; }

        /// <summary>
        /// Gets whether the property is read-only.
        /// </summary>

        public bool IsReadOnly { get; }

        /// <summary>
        /// Initializes a fast reflection provider for a property.
        /// </summary>
        /// <param name="propertyInfo">Property being reflected.</param>

        protected PropertyReflector( PropertyInfo propertyInfo )
        {
            PropertyInfo = propertyInfo ?? throw new ArgumentNullException( nameof(propertyInfo) );

            IsArray = PropertyInfo.PropertyType.IsArray;
        }

        /// <summary>
        /// Copies the value of the property from the source instance to the target.
        /// </summary>
        /// <param name="source">Source instance containing the property value to copy.</param>
        /// <param name="target">Target instance into which to copy the property value.</param>

        public abstract void Copy( T source, T target );
    }

    /// <summary>
    /// Fast reflection provider for a property.
    /// </summary>
    /// <typeparam name="T">Declaring type of the property.</typeparam>
    /// <typeparam name="V">Type of the property value.</typeparam>

    class PropertyReflector<T, V> : PropertyReflector<T>
    {
        /// <summary>
        /// Defines an unbound property accessor.
        /// </summary>
        /// <param name="instance">Instance whose property value to access.</param>

        delegate V AccessorDelegate( T instance );

        /// <summary>
        /// Unbound property accessor.
        /// </summary>

        readonly AccessorDelegate Accessor;

        /// <summary>
        /// Defines an unbound property mutator.
        /// </summary>
        /// <param name="instance">Instance whose property value to set.</param>
        /// <param name="value">New value of the property.</param>

        delegate void MutatorDelegate( T instance, V value );

        /// <summary>
        /// Unbound property mutator.
        /// </summary>

        readonly MutatorDelegate Mutator;
        
        /// <summary>
        /// Constructs a fast reflection provider for a property.
        /// </summary>
        /// <param name="propertyInfo">Property to reflect.</param>
        
        public PropertyReflector( PropertyInfo propertyInfo ) : base( propertyInfo )
        {
            Accessor = (AccessorDelegate)PropertyInfo
                .GetMethod
                .CreateDelegate( typeof( AccessorDelegate ) );

            Mutator = (MutatorDelegate)PropertyInfo
                .SetMethod?
                .CreateDelegate( typeof( MutatorDelegate ) );
        }

        /// <summary>
        /// Copies the value of the property from the source instance to the target.
        /// </summary>
        /// <param name="source">Source instance containing the property value to copy.</param>
        /// <param name="target">Target instance into which to copy the property value.</param>

        public override void Copy( T source, T target )
        {
            if ( source == null ) throw new ArgumentNullException( nameof( source ) );
            if ( target == null ) throw new ArgumentNullException( nameof( target ) );
            if ( IsReadOnly ) throw new InvalidOperationException( "Property is read-only" );

            var value = Accessor.Invoke( source );

            // for arrays, we want to copy the contents rather than the structure itself
            if ( IsArray && (object)value is Array array )
            {
                value = (V)array.Clone();
            }

            Mutator.Invoke( target, value );
        }
    }
}