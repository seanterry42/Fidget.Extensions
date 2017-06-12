using Fidget.Extensions.Reflection.Internal;
using Xunit;

namespace Fidget.Extensions.Reflection
{
    /// <summary>
    /// Tests of the type extensions methods.
    /// </summary>

    public class TypeExtensionsTest
    {
        /// <summary>
        /// Tests of the reflect method.
        /// </summary>
        
        public class Reflect
        {
            /// <summary>
            /// Model type to reflect.
            /// </summary>
            
            class Model {}

            /// <summary>
            /// Ensures that a reflector is returned when no model instance is specified.
            /// </summary>
            
            [Fact]
            public void WhenNoInstance_returns_reflector()
            {
                var actual = TypeExtensions.Reflect<Model>();
                Assert.IsType<TypeReflector<Model>>( actual );
                Assert.IsAssignableFrom<ITypeReflector<Model>>( actual );
            }

            /// <summary>
            /// Ensures that a reflector is returned when a null model instance is given.
            /// </summary>

            [Fact]
            public void WhenNullInstance_returns_reflector()
            {
                Model instance = null;
                var actual = TypeExtensions.Reflect( instance );
                Assert.IsType<TypeReflector<Model>>( actual );
                Assert.IsAssignableFrom<ITypeReflector<Model>>( actual );
            }

            /// <summary>
            /// Ensures that a reflector is returned when a model instance is given.
            /// </summary>
            
            [Fact]
            public void WhenInstance_returns_reflector()
            {
                var instance = new Model();
                var actual = TypeExtensions.Reflect( instance );
                Assert.IsType<TypeReflector<Model>>( actual );
                Assert.IsAssignableFrom<ITypeReflector<Model>>( actual );
            }

            /// <summary>
            /// Ensures that a singleton instance is used.
            /// </summary>
            
            [Fact]
            public void Returns_same_instance()
            {
                var expected = TypeExtensions.Reflect<Model>();
                
                for ( var i = 0; i < 3; i++ )
                {
                    var actual = TypeExtensions.Reflect<Model>();
                    Assert.Equal( expected, actual );

                    actual = TypeExtensions.Reflect( (Model)null );
                    Assert.Equal( expected, actual );

                    actual = TypeExtensions.Reflect( new Model() );
                    Assert.Equal( expected, actual );
                }
            }
        }
    }
}