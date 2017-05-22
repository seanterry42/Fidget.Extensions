using System;
using Xunit;

namespace Fidget.Extensions.Guids.Test
{
    /// <summary>
    /// Tests of the SequentialGuid extension methods.
    /// </summary>

    public class SequentialGuidTesting
    {
        /// <summary>
        /// Tests of the Epoch property.
        /// </summary>
        
        public class Epoch
        {
            [Fact]
            public void Equals_GregorianReformDate_October_15_1582()
            {
                var expected = new DateTime( 1582, 10, 15 ).Ticks;
                var actual = SequentialGuid.Epoch;

                Assert.Equal( expected, actual );
            }
        }

        /// <summary>
        /// Tests of the create method for SQL Server types.
        /// </summary>
        
        public class Create_SqlServer
        {
            /// <summary>
            /// Calls the create method.
            /// </summary>
            
            Guid CallCreate() => SequentialGuid.Create( GuidAlgorithm.SqlServer );

            /// <summary>
            /// The variant should be zero to avoid collisions with real Guids.
            /// </summary>
            
            [Fact]
            public void Returns_variant_zero()
            {
                var actual = CallCreate().ToByteArray();
                
                // first bit of byte 8 should be zero
                Assert.Equal( 0, actual[8] & 0x80 );
            }
            
            /// <summary>
            /// The version has been moved to the same field as the variant.
            /// </summary>
            
            [Fact]
            public void Returns_version_one()
            {
                var actual = CallCreate().ToByteArray();

                // first nibble of byte 8 should be one
                Assert.Equal( 1, actual[8] >> 4 );
            }

            /// <summary>
            /// The originating variant (for UUIDv1 transposability) should be one.
            /// </summary>
            
            [Fact]
            public void Returns_original_variant_one()
            {
                var actual = CallCreate().ToByteArray();
                Assert.NotEqual( 0, actual[6] & 0x80 );
                Assert.Equal( 0, actual[6] & 0x40 );
            }
        }
    }
}