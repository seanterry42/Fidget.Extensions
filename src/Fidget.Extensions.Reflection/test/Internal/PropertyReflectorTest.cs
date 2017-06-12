using System;
using System.Reflection;
using Xunit;

namespace Fidget.Extensions.Reflection.Internal
{
    /// <summary>
    /// Tests of the PropertyReflector type.
    /// </summary>

    public class PropertyReflectorTest
    {
        /// <summary>
        /// Tests of the PropertyInfo property.
        /// </summary>
        
        public class PropertyInfo
        {
            /// <summary>
            /// Model type for testing.
            /// </summary>
            
            class Model
            {
                public Guid? Value { get; set; }
            }

            [Fact]
            public void Returns_propertyInfo_fromReflection()
            {
                var expected = typeof(Model).GetProperty( nameof(Model.Value) );
                var actual = TypeReflector<Model>.Instance[nameof(Model.Value)].PropertyInfo;

                Assert.Equal( expected, actual );
            }
        }

        /// <summary>
        /// Tests of the IsArray property.
        /// </summary>
        
        public class IsArray
        {
            /// <summary>
            /// Model type for testing.
            /// </summary>
            
            class Model
            {
                public Guid? Value { get; set; }
                public string String { get; set; }
                public object Reference { get; set; }
                public byte[] Array { get; set; }
            }

            [Theory]
            [InlineData(nameof(Model.Value))]
            [InlineData(nameof(Model.String))]
            [InlineData(nameof(Model.Reference))]
            public void Returns_false_whenNonArray( string propertyName )
            {
                var actual = TypeReflector<Model>.Instance[propertyName];
                Assert.False( actual.IsArray );
            }

            [Theory]
            [InlineData(nameof(Model.Array))]
            public void Returns_true_whenArray( string propertyName )
            {
                var actual = TypeReflector<Model>.Instance[propertyName];
                Assert.True( actual.IsArray );
            }
        }

        /// <summary>
        /// Tests of the IsReadOnly property.
        /// </summary>
        
        public class IsReadOnly
        {
            /// <summary>
            /// Model type for testing.
            /// </summary>
            
            class Model
            {
                public Guid? Value { get; set; }
                public Guid? ReadOnlyValue { get; }
            }

            [Fact]
            public void Returns_false_whenWriteableProperty()
            {
                var actual = TypeReflector<Model>.Instance[nameof(Model.Value)];
                Assert.False( actual.IsReadOnly );
            }

            [Fact]
            public void Returns_true_whenReadOnlyProperty()
            {
                var actual = TypeReflector<Model>.Instance[nameof( Model.ReadOnlyValue )];
                Assert.True( actual.IsReadOnly );
            }
        }

        /// <summary>
        /// Tests of the copy method.
        /// </summary>
        
        public class Copy
        {
            /// <summary>
            /// Model type for testing.
            /// </summary>
            
            class Model
            {
                public Guid? ReadOnlyValue { get; }
                public byte[] ReadOnlyArray { get; }
                public object ReadOnlyReference { get; }

                public Guid? Value { get; set; }
                public byte[] Array { get; set; }
                public object Reference { get; set; }
            }

            Model source = new Model();
            Model target = new Model
            {
                Value = Guid.Empty,
                Array = new byte[0],
                Reference = new object(),
            };

            void Invoke( string propertyName ) => TypeReflector<Model>.Instance[propertyName].Copy( source, target ); 
            
            [Fact]
            public void Requires_source()
            {
                source = null;
                Assert.Throws<ArgumentNullException>( nameof(source), () => Invoke( nameof(Model.Value) ) );
            }

            [Fact]
            public void Requires_target()
            {
                target = null;
                Assert.Throws<ArgumentNullException>( nameof( target ), () => Invoke( nameof( Model.Value ) ) );
            }

            [Theory]
            [InlineData(nameof(Model.ReadOnlyValue))]
            [InlineData( nameof( Model.ReadOnlyReference ) )]
            [InlineData( nameof( Model.ReadOnlyArray ) )]
            public void Throws_invalidOperation_whenReadOnly( string propertyName )
            {
                Assert.Throws<InvalidOperationException>( () => Invoke( propertyName ) );
            }

            [Theory]
            [InlineData( false )]
            [InlineData( true )]
            public void Copies_valueType_byValue( bool useNull )
            {
                var expected = source.Value = useNull ? (Guid?)null : Guid.NewGuid();
                Invoke( nameof(Model.Value) );
                var actual = target.Value;

                Assert.Equal( expected, actual );
            }

            [Theory]
            [InlineData( false )]
            [InlineData( true )]
            public void Copies_referenceType_byReference( bool useNull )
            {
                var expected = source.Reference = useNull ? null : new object();
                Invoke( nameof( Model.Reference ) );
                var actual = target.Reference;

                Assert.Equal( expected, actual );
            }

            [Theory]
            [InlineData( false )]
            [InlineData( true )]
            public void Copies_arrayType_byValue( bool useNull )
            {
                var expected = source.Array = useNull ? null : Guid.NewGuid().ToByteArray();
                Invoke( nameof(Model.Array) );
                var actual = target.Array;

                Assert.Equal( expected, actual );

                // ensure non-null values are not the same instances
                if ( expected != null ) Assert.NotSame( expected, actual );
            }
        }
    }
}