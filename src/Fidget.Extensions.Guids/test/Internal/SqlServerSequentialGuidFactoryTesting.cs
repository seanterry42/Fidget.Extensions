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
        /// Source GUID value.
        /// </summary>
        
        Guid source = Guid.NewGuid();

        /// <summary>
        /// Sequence argument.
        /// </summary>
        
        long sequence;

        /// <summary>
        /// Calls the create method with the configured arguments.
        /// </summary>
        
        Guid CallCreate() => source.Sequentialize( GuidFormat.SqlServer, sequence );

        [Fact]
        public void Create_requires_positive_sequence()
        {
            sequence = new Random().Next( int.MinValue, 0 );
            Assert.Throws<ArgumentOutOfRangeException>( "sequence", () => CallCreate() );
        }

        [Fact]
        public void Create_returns_variant_zero()
        {
            sequence = long.MaxValue;
            var actual = CallCreate().ToByteArray();
            
            // first bit of byte 8 should always be zero
            Assert.Equal( 0x00, actual[8] & 0x80 );
        }

        /// <summary>
        /// The highest order sequence bits should be shifted by one place and in the heaviest
        /// weighted fields (10-15).
        /// </summary>
        
        [Fact]
        public void Create_returns_highOrderSequenceInCorrectOrder()
        {
            sequence = 0x0123456789ABCDEF;

            // high order sequence is shifted
            var bytes = BitConverter.GetBytes( sequence << 1 );
            var actual = CallCreate().ToByteArray();
            
            // highest order should be in 10-15
            Assert.Equal( bytes[7], actual[10] );
            Assert.Equal( bytes[6], actual[11] );
            Assert.Equal( bytes[5], actual[12] );
            Assert.Equal( bytes[4], actual[13] );
            Assert.Equal( bytes[3], actual[14] );
            Assert.Equal( bytes[2], actual[15] );
        }

        [Fact]
        public void Create_returns_lowOrderSequenceInCorrectOrder()
        {
            sequence = 0x0123456789ABCDEF;
            var bytes = BitConverter.GetBytes( sequence );
            var actual = CallCreate().ToByteArray();

            // first bit cleared for variant
            Assert.Equal( bytes[1] & 0x7f, actual[8] );
            Assert.Equal( bytes[0], actual[9] );
        }

        /// <summary>
        /// The source Guid should hold the bottom 64 bits.
        /// </summary>
        
        [Fact]
        public void Create_returns_bottomOrderSourceGuid()
        {
            sequence = 0x0123456789ABCDEF;
            var expected = source.ToByteArray();
            var actual = CallCreate().ToByteArray();

            for ( var i = 0; i < 8; i++ )
            {
                Assert.Equal( expected[i], actual[i] );
            }
        }
    }
}