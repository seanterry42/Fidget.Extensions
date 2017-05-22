using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Fidget.Extensions.Guids.Internal
{
    /// <summary>
    /// Tests of the UUIDv1 Guid factory.
    /// </summary>
    
    public class VersionOneSequentialGuidFactoryTesting
    {
        /// <summary>
        /// Time argument.
        /// </summary>

        long time;

        /// <summary>
        /// Clock argument.
        /// </summary>
        
        int clock;

        /// <summary>
        /// Node argument.
        /// </summary>
        
        byte[] node = new byte[6];
        
        /// <summary>
        /// Tests of the create method.
        /// </summary>
        
        public class Create : VersionOneSequentialGuidFactoryTesting
        {
            /// <summary>
            /// Calls the create method with the configured arguments.
            /// </summary>

            Guid CallCreate() => SequentialGuid
                .GetFactory( GuidAlgorithm.UuidVersion1 )
                .Create( time, clock, node );

            [Fact]
            public void Requires_node()
            {
                node = null;
                Assert.Throws<ArgumentNullException>( "node", () => CallCreate() );
            }

            [Theory]
            [InlineData( 5 )]
            [InlineData( 7 )]
            public void Requires_node_6bytes( int length )
            {
                node = new byte[length];
                Assert.Throws<ArgumentException>( "node", () => CallCreate() );
            }

            /// <summary>
            /// The least-significant time bytes contain the time value.
            /// </summary>

            [Theory]
            [InlineData( 0 )]
            [InlineData( long.MaxValue )]
            [InlineData( long.MinValue )]
            [InlineData( 0x0102030405060708 )]
            public void Returns_bytes0through6_time( long timeValue )
            {
                time = timeValue;
                var guid = CallCreate();
                var actual = guid.ToByteArray();

                for ( var i = 0; i <= 6; i++ )
                {
                    var expected = (timeValue >> (i * 8)) & 0xff;
                    Assert.Equal( expected, actual[i] );
                }
            }

            /// <summary>
            /// The most significant nibble of the time stores the version.
            /// </summary>

            [Theory]
            [InlineData( 0 )]
            [InlineData( long.MaxValue )]
            [InlineData( long.MinValue )]
            [InlineData( 0x0102030405060708 )]
            public void Returns_byte7_timeHigh_with_version( long timeValue )
            {
                time = timeValue;
                var guid = CallCreate();
                var actual = guid.ToByteArray();
                const int version = 0x10;
                var expected = version + (((timeValue >> 56) & 0x0f));

                Assert.Equal( expected, actual[7] );
            }

            /// <summary>
            /// Byte 8 should contain the high clock sequence and the variant.
            /// </summary>

            [Theory]
            [InlineData( 0 )]
            [InlineData( int.MaxValue )]
            [InlineData( int.MinValue )]
            [InlineData( 0x1234 )]
            public void Returns_byte8_clockHigh_with_variant( int clockValue )
            {
                clock = clockValue;
                var guid = CallCreate();
                var actual = guid.ToByteArray();
                
                const int variant = 0x80;
                var expected = variant + ( ( clockValue >> 8 ) & 0x3f );
                
                Assert.Equal( expected, actual[8] );
            }
            
            /// <summary>
            /// Byte 9 should contain the lower clock value sequence.
            /// </summary>

            [Theory]
            [InlineData( 0 )]
            [InlineData( int.MaxValue )]
            [InlineData( int.MinValue )]
            [InlineData( 0x1234 )]
            public void Returns_byte9_clockLow( int clockValue )
            {
                clock = clockValue;
                var guid = CallCreate();
                var actual = guid.ToByteArray();
                var expected = clockValue & 0xff;

                Assert.Equal( expected, actual[9] );
            }

            /// <summary>
            /// Bytes 10 to 15 contain the node.
            /// </summary>
            
            [Fact]
            public void Returns_bytes10to15_node()
            {
                node = new byte[6];
                new Random().NextBytes( node );
                var guid = CallCreate();
                var actual = guid.ToByteArray();

                for ( var i = 0; i < 6; i++ )
                {
                    Assert.Equal( node[i], actual[i + 10] );
                }
            }

            /// <summary>
            /// Sanity check on byte order.
            /// </summary>
            
            [Fact]
            public void SanityCheck()
            {
                var bytes = new byte[16];

                for ( byte i = 0; i < 16; i++ )
                {
                    bytes[i] = i;
                }

                var actual = new Guid( bytes );
                Assert.Equal( "03020100-0504-0706-0809-0a0b0c0d0e0f", actual.ToString(), true );
            }
        }
    }
}