using System.Numerics;
using Xunit;

namespace Elgamal.Lib.UnitTest
{
    public class BabyStepGiantStep
    {
        [Fact]
        public void EqualsTest()
        {
            var p = BigInteger.Pow(10, 9) + 7;

            Assert.Equal(
                new BigInteger(-323529414),
                BabyStepGiantStepImplementation.Gcd(34, p).Item2);

            Assert.Equal(
                new BigInteger(676470593),
                BabyStepGiantStepImplementation.Inv(34, p));

            Assert.Equal(
                new BigInteger(316464245),
                BabyStepGiantStepImplementation.LogMod(34, 245, p));

            Assert.Equal(
                245,
                BabyStepGiantStepImplementation.SquareExp(34, 316464245, p)
                );
        }

        [Fact]
        public void ManyEqualsTest()
        {
            var @params = ElgamalParameters.Generate(32);

            var keys = ElgamalKey.GenerateKeyPair(@params);

            Assert.Equal(
                (keys.PrivateKey as ElgamalKey)!.Key,
                BabyStepGiantStepImplementation.LogMod(
                    @params.G,
                    (keys.PublicKey as ElgamalKey)!.Key,
                    @params.P));
        }
    }
}