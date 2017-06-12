using System;
using Xunit;

namespace Fidget.Extensions.Reflection.Internal
{
    /// <summary>
    /// Tests of the type reflector.
    /// </summary>

    public class TypeReflectorTest
    {
        /// <summary>
        /// Tests of the Clone method.
        /// </summary>
        
        public class Clone
        {
            /// <summary>
            /// Model class for testing.
            /// </summary>
            
            class Model
            {
                public Guid? Value { get; set; }
                public byte[] Array { get; set; }
                public string String { get; set; }
                public object Reference { get; set; }
            }

            /// <summary>
            /// Method instance argument.
            /// </summary>
            
            Model source = new Model();

            /// <summary>
            /// Invokes the method under test with the supplied argument(s).
            /// </summary>
                        
            Model Invoke() => TypeReflector<Model>.Instance.Clone( source );

            [Fact]
            public void Requires_source()
            {
                source = null;
                Assert.Throws<ArgumentNullException>( nameof(source), () => Invoke() );
            }

            [Fact]
            public void Returns_cloned_type()
            {
                var actual = Invoke();
                Assert.IsType<Model>( actual );
            }

            [Fact]
            public void Returns_different_instance()
            {
                var expected = source;
                var actual = Invoke();
                Assert.NotEqual( expected, actual );
            }

            [Theory]
            [InlineData( true )]
            [InlineData( false )]
            public void Returns_property_whenValueType_equal( bool useNull )
            {
                source.Value = useNull ? (Guid?)null : Guid.NewGuid();
                var expected = source.Value;
                var actual = Invoke().Value;

                Assert.Equal( expected, actual );
            }

            [Theory]
            [InlineData( true )]
            [InlineData( false )]
            public void Returns_property_whenStringType_equal( bool useNull )
            {
                var expected = source.String = useNull ? (string)null : Guid.NewGuid().ToString();
                var actual = Invoke().String;

                Assert.Equal( expected, actual );
            }

            [Theory]
            [InlineData( true )]
            [InlineData( false )]
            public void Returns_property_whenReferenceType_refernceEqual( bool useNull )
            {
                var expected = source.Reference = useNull ? null : new object();
                var actual = Invoke().Reference;

                Assert.True( object.ReferenceEquals( expected, actual ) );
            }

            [Theory]
            [InlineData( true )]
            [InlineData( false )]
            public void Returns_property_whenArrayType_equal( bool useNull )
            {
                var expected = source.Array = useNull ? null : Guid.NewGuid().ToByteArray();
                var actual = Invoke().Array;

                Assert.Equal( expected, actual );
            }

            /// <summary>
            /// Array types should be cloned, rather than reference the same instance.
            /// </summary>
            
            [Fact]
            public void Returns_property_whenArrayType_notReferenceEqual()
            {
                var expected = source.Array = Guid.NewGuid().ToByteArray();
                var actual = Invoke().Array;

                Assert.False( object.ReferenceEquals( expected, actual ) );
            }
        }
    }
}