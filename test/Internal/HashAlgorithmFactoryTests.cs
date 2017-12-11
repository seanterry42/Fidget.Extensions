using System;
using System.Security.Cryptography;
using Xunit;

namespace Identifiable.Internal
{
    public class HashAlgorithmFactoryTests
    {
        IHashAlgorithmFactory instance => new HashAlgorithmFactory();
        
        public class Create : HashAlgorithmFactoryTests
        {
            NamedGuidAlgorithm algorithm;
            HashAlgorithm invoke() => instance.Create( algorithm );

            [Fact]
            public void when_algorithm_unknown_throws()
            {
                algorithm = default(NamedGuidAlgorithm);
                Assert.Throws<NotImplementedException>( ()=> invoke() );
            }

            [Theory]
            [InlineData( NamedGuidAlgorithm.MD5, typeof(MD5) )]
            [InlineData( NamedGuidAlgorithm.SHA1, typeof(SHA1) )]
            public void returns_known_algorithm( NamedGuidAlgorithm algorithm, Type expectedType )
            {
                this.algorithm = algorithm;
            
                using ( var actual = invoke() )
                {
                    Assert.IsAssignableFrom( expectedType, actual );
                }
            }
        }
    }
}