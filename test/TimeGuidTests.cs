using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace Identifiable
{
    /// <summary>
    /// Tests of time-based GUID generation.
    /// </summary>
    
    public class TimeGuidTests
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
                var actual = TimeGuid.Epoch;

                Assert.Equal( expected, actual );
            }
        }

        /// <summary>
        /// Tests of the GetFormatter method.
        /// </summary>
        
        public class GetFormatter
        {
            [Fact]
            public void Requires_expected_layout()
            {
                var layout = default(TimeGuidLayout);
                Assert.Throws<NotImplementedException>( ()=> TimeGuid.GetFormatter( layout ) );
            }

            [Fact]
            public void When_Standard_ReturnsFormatter()
            {
                var actual = TimeGuid.GetFormatter( TimeGuidLayout.Standard );
                Assert.IsType<TimeGuid.StandardFormatter>( actual );
            }

            [Fact]
            public void When_SqlServer_ReturnsFormatter()
            {
                var actual = TimeGuid.GetFormatter( TimeGuidLayout.SqlServer );
                Assert.IsType<TimeGuid.SqlServerFormatter>( actual );
            }
        }

        /// <summary>
        /// Tests of the standard formatter.
        /// </summary>
        
        public class Standard
        {
            long time = 0;
            short clock = 0;
            byte[] node = new byte[6];

            Guid CallCreate() => TimeGuid.StandardFormatter.Instance.Create( time, clock, node );

            [Fact]
            public void Requires_node()
            {
                node = null;
                Assert.Throws<ArgumentNullException>( "node", () => CallCreate() );
            }

            [Theory]
            [InlineData(5)]
            [InlineData(7)]
            public void Requires_node_6_bytes( int size )
            {
                node = new byte[size];
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
            [InlineData( short.MaxValue )]
            [InlineData( short.MinValue )]
            [InlineData( 0x1234 )]
            public void Returns_byte8_clockHigh_with_variant( short clockValue )
            {
                clock = clockValue;
                var guid = CallCreate();
                var actual = guid.ToByteArray();

                const int variant = 0x80;
                var expected = variant + ((clockValue >> 8) & 0x3f);

                Assert.Equal( expected, actual[8] );
            }

            /// <summary>
            /// Byte 9 should contain the lower clock value sequence.
            /// </summary>

            [Theory]
            [InlineData( 0 )]
            [InlineData( short.MaxValue )]
            [InlineData( short.MinValue )]
            [InlineData( 0x1234 )]
            public void Returns_byte9_clockLow( short clockValue )
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
        }

        /// <summary>
        /// Tests of the SQL Server formatter.
        /// </summary>
        
        public class SqlServer
        {
            long time = 0;
            short clock = 0;
            byte[] node = new byte[6];

            Guid CallCreate() => TimeGuid.SqlServerFormatter.Instance.Create( time, clock, node );

            [Fact]
            public void Requires_node()
            {
                node = null;
                Assert.Throws<ArgumentNullException>( "node", () => CallCreate() );
            }

            [Theory]
            [InlineData( 5 )]
            [InlineData( 7 )]
            public void Requires_node_6_bytes( int size )
            {
                node = new byte[size];
                Assert.Throws<ArgumentException>( "node", () => CallCreate() );
            }

            /// <summary>
            /// Most significant bytes should contain the high time value.
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
                var expected = 0x10 + ((timeValue >> 8) & 0x0F);
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
            [InlineData( short.MinValue )]
            [InlineData( short.MaxValue )]
            [InlineData( 0 )]
            [InlineData( 0x1234 )]

            public void Returns_bytes6_clockHigh_with_variant( short clockValue )
            {
                clock = clockValue;
                var guid = CallCreate();
                var actual = guid.ToByteArray();
                const int variant = 0x80;
                var expected = variant + ((clockValue >> 8) & 0x3F);

                Assert.Equal( expected, actual[6] );
            }

            [Theory]
            [InlineData( short.MinValue )]
            [InlineData( short.MaxValue )]
            [InlineData( 0 )]
            [InlineData( 0x1234 )]

            public void Returns_byte7_clockLow( short clockValue )
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

        /// <summary>
        /// Tests of the create method.
        /// </summary>
        
        public class Create
        {
            TimeGuidLayout layout = default(TimeGuidLayout);
            Guid CallCreate() => TimeGuid.Create( layout );

            /// <summary>
            /// Standard layout should identify as variant one.
            /// </summary>

            [Fact]
            public void Standard_returns_variant_one()
            {
                layout = TimeGuidLayout.Standard;
                var actual = CallCreate().ToByteArray();

                // first bit of byte 8 should be set
                Assert.NotEqual( 0, actual[8] & 0x80 );
                // second bit of byte 8 should not be set
                Assert.Equal( 0, actual[8] & 0x40 );
            }

            /// <summary>
            /// Standard layout should identify as version 1 uuid.
            /// </summary>

            [Fact]
            public void Standard_returns_version_one()
            {
                layout = TimeGuidLayout.Standard;
                var actual = CallCreate().ToByteArray();
                Assert.Equal( 0x10, actual[7] & 0xF0 );
            }

            /// <summary>
            /// Multicast bit of the node must be set when randomly generated.
            /// </summary>

            [Fact]
            public void Standard_returns_node_with_multicast_bit()
            {
                layout = TimeGuidLayout.Standard;
                var actual = CallCreate().ToByteArray();
                Assert.Equal( 1, actual[10] & 0x01 );
            }

            /// <summary>
            /// SQL Server layout should identify as variant zero to avoid collisions with real Guids.
            /// </summary>

            [Fact]
            public void SqlServer_returns_variant_zero()
            {
                layout = TimeGuidLayout.SqlServer;
                var actual = CallCreate().ToByteArray();

                // first bit of byte 8 should be zero
                Assert.Equal( 0, actual[8] & 0x80 );
            }

            /// <summary>
            /// The version has been moved to the same field as the variant in SQL Server layout.
            /// </summary>

            [Fact]
            public void SqlServer_returns_version_one()
            {
                layout = TimeGuidLayout.SqlServer;
                var actual = CallCreate().ToByteArray();

                // first nibble of byte 8 should be one
                Assert.Equal( 1, actual[8] >> 4 );
            }

            /// <summary>
            /// The originating variant (for UUIDv1 transposability) should be one.
            /// </summary>

            [Fact]
            public void SqlServer_returns_original_variant_one()
            {
                layout = TimeGuidLayout.SqlServer;
                var actual = CallCreate().ToByteArray();
                Assert.NotEqual( 0, actual[6] & 0x80 );
                Assert.Equal( 0, actual[6] & 0x40 );
            }

            /// <summary>
            /// Multicast bit of the node must be set when randomly generated.
            /// </summary>

            [Fact]
            public void SqlServer_returns_node_with_multicast_bit()
            {
                layout = TimeGuidLayout.SqlServer;
                var guid = CallCreate();
                var actual = guid.ToByteArray();
                Assert.Equal( 1, actual[0] & 0x01 );
            }

            /// <summary>
            /// All hex characters in SQL server and Standard layouts should be transposable.
            /// This just checks that all of the characters are present, not that they are in the correct order.
            /// </summary>
            
            [Fact]
            public void SqlServer_returns_transposed_standard()
            {
                var time = DateTime.UtcNow.Ticks - TimeGuid.Epoch;
                short clock = 0x6677;
                var node = new byte[] { 0x00, 0x11, 0x22, 0x33, 0x44, 0x55 };

                var sql = TimeGuid.GetFormatter( TimeGuidLayout.SqlServer ).Create( time, clock, node ).ToString().OrderBy( _=> _ );
                var standard = TimeGuid.GetFormatter( TimeGuidLayout.Standard ).Create( time, clock, node ).ToString().OrderBy( _=> _ );

                Assert.True( sql.SequenceEqual( standard ) );
            }
        }
    }
}