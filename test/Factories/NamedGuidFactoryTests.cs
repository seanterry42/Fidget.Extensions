using System;
using Xunit;

namespace Identifiable.Factories
{
    public class NamedGuidFactoryTests
    {
        IHashAlgorithmFactory algorithmFactory = new HashAlgorithmFactory();
        INamedGuidFactory instance => new NamedGuidFactory( algorithmFactory );

        public class Constructor : NamedGuidFactoryTests
        {
            [Fact]
            public void requires_algorithmFactory()
            {
                algorithmFactory = null;
                Assert.Throws<ArgumentNullException>( nameof( algorithmFactory ), () => instance );
            }
        }

        public class Compute : NamedGuidFactoryTests
        {
            NamedGuidAlgorithm algorithm;
            Guid @namespace = Guid.NewGuid();
            string name = string.Empty;

            Guid invoke() => instance.Compute( algorithm, @namespace, name );

            [Fact]
            public void requires_name()
            {
                name = null;
                Assert.Throws<ArgumentNullException>( nameof( name ), () => invoke() );
            }

            [Fact]
            public void requires_valid_algorithm()
            {
                algorithm = default( NamedGuidAlgorithm );
                Assert.Throws<NotImplementedException>( () => invoke() );
            }

            [Theory]
            [InlineData( NamedGuidAlgorithm.MD5 )]
            [InlineData( NamedGuidAlgorithm.SHA1 )]
            public void returns_variant_1( NamedGuidAlgorithm algorithm )
            {
                name = $"{Guid.NewGuid()}";
                this.algorithm = algorithm;
                var actual = invoke();

                // isolate the first two bits
                var variant = actual.ToByteArray()[8] & 0b11000000;

                // ensure the first bit is on, the second bit is off
                Assert.Equal( 0b10000000, variant );
            }

            [Theory]
            [InlineData( NamedGuidAlgorithm.MD5 )]
            [InlineData( NamedGuidAlgorithm.SHA1 )]
            public void returns_version_for_algorithm( NamedGuidAlgorithm algorithm )
            {
                name = $"{Guid.NewGuid()}";
                this.algorithm = algorithm;
                var actual = invoke();

                // isolate the first nibble
                var version = actual.ToByteArray()[7] & 0b11110000;

                switch ( algorithm )
                {
                    case NamedGuidAlgorithm.MD5:
                        Assert.Equal( StandardGuidVersion.UUIDv3, version );
                        break;
                    case NamedGuidAlgorithm.SHA1:
                        Assert.Equal( StandardGuidVersion.UUIDv5, version );
                        break;
                    default:
                        throw new InvalidOperationException();
                }
            }

            public class DeterministicResults : TheoryData<NamedGuidAlgorithm, Guid, string, Guid>
            {
                public DeterministicResults()
                {
                    // example from https://tools.ietf.org/html/rfc4122#appendix-B
                    // note the result comes from the errata: http://www.rfc-editor.org/errata_search.php?rfc=4122&eid=1352
                    Add( NamedGuidAlgorithm.MD5, new Guid( "6ba7b810-9dad-11d1-80b4-00c04fd430c8" ), "www.widgets.com", new Guid( "3d813cbb-47fb-32ba-91df-831e1593ac29" ) );
                }
            }

            [Theory]
            [ClassData( typeof( DeterministicResults ) )]
            public void returns_deterministic( NamedGuidAlgorithm algorithm, Guid @namespace, string name, Guid expected )
            {
                this.@namespace = @namespace;
                this.algorithm = algorithm;
                this.name = name;

                var actual = invoke();
                Assert.Equal( expected, actual );

                // ensure changing the text yields a different result
                this.name += "!";
                actual = invoke();
                Assert.NotEqual( expected, actual );
            }
        }
    }
}