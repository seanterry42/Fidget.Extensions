using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Fidget.Extensions.Guids.Internal
{
    /// <summary>
    /// Tests of the SQL Server sequential Guid factory.
    /// </summary>
    
    public class SqlServerSequentialGuidFactoryTesting
    {
        /// <summary>
        /// Time argument.
        /// </summary>
        
        long time;

        /// <summary>
        /// Clock arguments.
        /// </summary>

        int clock;

        /// <summary>
        /// Node argument.
        /// </summary>
        
        byte[] node = new byte[6];
        
        /// <summary>
        /// Tests of the create method.
        /// </summary>
        
        public class Create : SqlServerSequentialGuidFactoryTesting
        {
            /// <summary>
            /// Calls the create method.
            /// </summary>
            
            Guid CallCreate() => SequentialGuid
                .GetFactory( GuidAlgorithm.SqlServer )
                .Create( time, clock, node );

            /// <summary>
            /// Next most significant bytes should contain the middle time value.
            /// </summary>

            [Theory]
            [InlineData( long.MinValue )]
            [InlineData( long.MaxValue )]
            [InlineData( 0 )]
            [InlineData( 0x01234567890ABCDEF )]

            public void Returns_bytes10to15_timeHigh( long timeValue )
            {
                time = timeValue;
                var guid = CallCreate();
                var actual = guid.ToByteArray();
                
                // skip the low sequence
                timeValue >>= 12;

                for ( var i = 15; i >= 10; i-- )
                {
                    var expected = timeValue & 0xFF;
                    Assert.Equal( expected, actual[i] );
                    timeValue >>= 8;
                }
            }

            [Theory]
            [InlineData( long.MinValue )]
            [InlineData( long.MaxValue )]
            [InlineData( 0 )]
            [InlineData( 0x0123456789ABCDEF )]

            public void Returns_byte8_timeLow_with_variantVersion( long timeValue )
            {
                time = timeValue;
                var guid = CallCreate();
                var actual = guid.ToByteArray();
                var expected = 0x10 + ( ( timeValue >> 8 ) & 0x0F );
                Assert.Equal( expected, actual[8] );
            }

            [Theory]
            [InlineData( long.MinValue )]
            [InlineData( long.MaxValue )]
            [InlineData( 0 )]
            [InlineData( 0x01234567890ABCDEF )]

            public void Returns_bytes9_timeLow( long timeValue )
            {
                time = timeValue;
                var guid = CallCreate();
                var actual = guid.ToByteArray();
                var expected = time & 0xFF;
                Assert.Equal( expected, actual[9] );
            }

            [Theory]
            [InlineData( int.MinValue )]
            [InlineData( int.MaxValue )]
            [InlineData( 0 )]
            [InlineData( 0x1234 )]
            
            public void Returns_bytes6_clockHigh_with_variant( int clockValue )
            {
                clock = clockValue;
                var guid = CallCreate();
                var actual = guid.ToByteArray();
                const int variant = 0x80;
                var expected = variant + ( ( clockValue >> 8 ) & 0x3F );

                Assert.Equal( expected, actual[6] );
            }

            [Theory]
            [InlineData( int.MinValue )]
            [InlineData( int.MaxValue )]
            [InlineData( 0 )]
            [InlineData( 0x1234 )]

            public void Returns_byte7_clockLow( int clockValue )
            {
                clock = clockValue;
                var guid = CallCreate();
                var actual = guid.ToByteArray();
                var expected = clockValue & 0xFF;

                Assert.Equal( expected, actual[7] );
            }

            /// <summary>
            /// Bytes 0 to 5 contain the node.
            /// </summary>

            [Fact]
            public void Returns_bytes0to5_node()
            {
                node = new byte[6];
                new Random().NextBytes( node );
                var guid = CallCreate();
                var actual = guid.ToByteArray();

                for ( var i = 0; i < 6; i++ )
                {
                    Assert.Equal( node[i], actual[i] );
                }
            }
        }
    }
}