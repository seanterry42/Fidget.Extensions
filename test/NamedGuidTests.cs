using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using Xunit;

namespace Identifiable
{
    /// <summary>
    /// Tests of the named GUID creator.
    /// </summary>
    
    public class NamedGuidTests
    {
        /// <summary>
        /// Tests of the GetAlgorithm method.
        /// </summary>
        
        public class GetAlgorithm
        {
            NamedGuidAlgorithm algorithm;
            HashAlgorithm CallGetAlgorithm() => NamedGuid.GetAlgorithm( algorithm );

            [Fact]
            public void Requires_valid_algorithm()
            {
                algorithm = default(NamedGuidAlgorithm);
                Assert.Throws<NotImplementedException>( ()=> CallGetAlgorithm() );
            }

            [Fact]
            public void When_MD5_returns_provider()
            {
                algorithm = NamedGuidAlgorithm.MD5;
                using ( var actual = CallGetAlgorithm() )
                {
                    Assert.IsAssignableFrom<MD5>( actual );
                }
            }

            [Fact]
            public void When_SHA1_returns_provider()
            {
                algorithm = NamedGuidAlgorithm.SHA1;
                using ( var actual = CallGetAlgorithm() )
                {
                    Assert.IsAssignableFrom<SHA1>( actual );
                }
            }
        }

        /// <summary>
        /// Tests of the GetVersion method.
        /// </summary>
        
        public class GetVersion
        {
            NamedGuidAlgorithm algorithm;
            byte CallGetVersion() => NamedGuid.GetVersion( algorithm );

            [Fact]
            public void Requires_valid_algorithm()
            {
                algorithm = default( NamedGuidAlgorithm );
                Assert.Throws<NotImplementedException>( () => CallGetVersion() );
            }

            [Fact]
            public void When_MD5_returns_3()
            {
                algorithm = NamedGuidAlgorithm.MD5;
                var actual = CallGetVersion();
                Assert.Equal( 0x30, actual );
            }

            [Fact]
            public void When_SHA1_returns_5()
            {
                algorithm = NamedGuidAlgorithm.SHA1;
                var actual = CallGetVersion();
                Assert.Equal( 0x50, actual );
            }
        }

        public class Create
        {
            NamedGuidAlgorithm algorithm;
            Guid @namespace;
            string name = string.Empty;
            Guid CallCreate() => NamedGuid.Create( algorithm, @namespace, name );

            [Fact]
            public void Requires_valid_algorithm()
            {
                algorithm = default( NamedGuidAlgorithm );
                Assert.Throws<NotImplementedException>( () => CallCreate() );
            }

            [Theory]
            [InlineData( NamedGuidAlgorithm.MD5 )]
            [InlineData( NamedGuidAlgorithm.SHA1 )]
            public void Requires_name( NamedGuidAlgorithm alg )
            {
                algorithm = alg;
                name = null;
                Assert.Throws<ArgumentNullException>( "name", ()=> CallCreate() );
            }

            /// <summary>
            /// Created GUID should report as variant 1.
            /// </summary>
            
            [Theory]
            [InlineData( NamedGuidAlgorithm.MD5 )]
            [InlineData( NamedGuidAlgorithm.SHA1 )]
            public void Returns_variant_1( NamedGuidAlgorithm alg )
            {
                algorithm = alg;
                var guid = CallCreate();
                var actual = guid.ToByteArray();

                // first bit of byte 8 should be set
                Assert.NotEqual( 0, actual[8] & 0x80 );
                // second bit of byte 8 should not be set
                Assert.Equal( 0, actual[8] & 0x40 );
            }

            [Fact]
            public void When_MD5_returns_version_3()
            {
                algorithm = NamedGuidAlgorithm.MD5;
                var guid = CallCreate();
                var actual = guid.ToByteArray();

                Assert.Equal( 0x30, actual[7] & 0xF0 );
            }

            [Fact]
            public void When_SHA1_returns_version_5()
            {
                algorithm = NamedGuidAlgorithm.SHA1;
                var guid = CallCreate();
                var actual = guid.ToByteArray();

                Assert.Equal( 0x50, actual[7] & 0xF0 );
            }

            [Fact]
            public void When_MD5_returns_deterministic()
            {
                algorithm = NamedGuidAlgorithm.MD5;
                @namespace = new Guid( "bb205f8f-f3c6-4204-bfdf-7ceeaca6d593" );
                name = "Hello World";
                var expected = new Guid( "77d1b2af-f19e-34e8-a2a8-dba40d8596ba" );
                var actual = CallCreate();

                Assert.Equal( expected, actual );

                // changing the text at all should result in a different identifier
                name = "Hello World!";
                actual = CallCreate();
                Assert.NotEqual( expected, actual );
            }

            [Fact]
            public void When_SHA1_returns_deterministic()
            {
                algorithm = NamedGuidAlgorithm.SHA1;
                @namespace = new Guid( "bb205f8f-f3c6-4204-bfdf-7ceeaca6d593" );
                name = "Hello World";
                var expected = new Guid( "d18fd408-dc10-52a9-9a61-d1eb6d6a3088" );
                var actual = CallCreate();

                Assert.Equal( expected, actual );

                // changing the text at all should result in a different identifier
                name = "Hello World!";
                actual = CallCreate();
                Assert.NotEqual( expected, actual );
            }
        }
    }
}