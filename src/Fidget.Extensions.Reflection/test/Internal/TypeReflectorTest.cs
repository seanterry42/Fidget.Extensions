using System;
using System.Collections.Generic;
using System.Text;
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
        }
    }
}