using System;
using System.Collections.Generic;
using System.Text;
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
        /// Tests of the GetNextSequence method.
        /// </summary>
        
        public class GetNextSequence
        {
            [Fact]
            public void Returns_GreaterThanPrevious()
            {
                var current = SequentialGuid.GetNextSequence();

                for ( var i = 0; i < 100; i++ )
                {
                    var next = SequentialGuid.GetNextSequence();
                    Assert.True( next > current );

                    current = next;
                }
            }

            [Fact]
            public void Returns_SystemTime_TicksFromEpoch()
            {
                var now = DateTime.UtcNow.Ticks - SequentialGuid.Epoch;
                var actual = SequentialGuid.GetNextSequence();
                var high = DateTime.UtcNow.Ticks + TimeSpan.TicksPerMillisecond;

                // this proves that our seqence was created after the system clock
                // was originally checked, but within a millisecond of our expected time.
                // the sequence can advance a ways do to other concurrent tests.
                Assert.InRange( actual, now, high );
            }
        }
    }
}