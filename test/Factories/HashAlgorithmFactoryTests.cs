using System;
using System.Security.Cryptography;
using Xunit;

namespace Identifiable.Factories
{
    public class HashAlgorithmFactoryTests
    {
        IHashAlgorithmFactory instance => new HashAlgorithmFactory();

        public class Create : HashAlgorithmFactoryTests
        {
            NamedGuidAlgorithm algorithm;
            HashAlgorithm invoke( out byte version ) => instance.Create( algorithm, out version );

            [Fact]
            public void when_algorithm_unknown_throws()
            {
                algorithm = default( NamedGuidAlgorithm );
                Assert.Throws<NotImplementedException>( () => invoke( out byte version ) );
            }

            [Theory]
            [InlineData( NamedGuidAlgorithm.MD5, typeof( MD5 ), StandardGuidVersion.UUIDv3 )]
            [InlineData( NamedGuidAlgorithm.SHA1, typeof( SHA1 ), StandardGuidVersion.UUIDv5 )]
            public void returns_known_algorithm( NamedGuidAlgorithm algorithm, Type expectedType, byte expectedVersion )
            {
                this.algorithm = algorithm;

                using ( var actualAlgorithm = invoke( out byte version ) )
                {
                    Assert.IsAssignableFrom( expectedType, actualAlgorithm );
                    Assert.Equal( expectedVersion, version );
                }
            }
        }
    }
}