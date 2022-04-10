using System.Numerics;

namespace Elgamal.Lib
{
    public static class BabyStepGiantStepImplementation
    {
        public static (BigInteger, BigInteger, BigInteger) Gcd(BigInteger a, BigInteger b)
            => Gcd(a, b, BigInteger.Zero, BigInteger.One, BigInteger.One, BigInteger.Zero);

        private static (BigInteger, BigInteger, BigInteger) Gcd(BigInteger a, BigInteger b, BigInteger alpha1, BigInteger alpha2, BigInteger beta1, BigInteger beta2)
        {
            while (true)
            {
                if (b == BigInteger.Zero)
                    return (a, alpha2, beta2);

                var q = a / b;

                var a1 = a;
                var alpha3 = alpha1;
                var beta3 = beta1;
                a = b;
                b = a1 % b;
                alpha1 = alpha2 - alpha1 * q;
                alpha2 = alpha3;
                beta1 = beta2 - beta1 * q;
                beta2 = beta3;
            }
        }

        public static BigInteger Inv(BigInteger x, BigInteger p)
        {
            var (_, i, _) = Gcd(x, p);

            return Mod(i, p);
        }

        private static BigInteger Mod(BigInteger a, BigInteger n)
        {
            BigInteger result = a % n;
            if ((result < 0 && n > 0) || (result > 0 && n < 0))
            {
                result += n;
            }
            return result;
        }

        public static BigInteger SquareExp(BigInteger a, BigInteger b, BigInteger p)
        {
            if (b == BigInteger.Zero)
                return 1;

            if (b % 2 == 0)
                return BigInteger.ModPow(SquareExp(a, b / 2, p) % p, 2, p);

            return (a * SquareExp(a, b - 1, p)) % p;
        }

        public static BigInteger Sqrt(BigInteger n)
        {
            if (n == 0) return 0;
            if (n > 0)
            {
                var bitLength = Convert.ToInt32(Math.Ceiling(BigInteger.Log(n, 2)));
                var root = BigInteger.One << (bitLength / 2);

                while (!IsSqrt(n, root))
                {
                    root += n / root;
                    root /= 2;
                }

                return root;
            }

            throw new ArithmeticException("NaN");
        }

        private static bool IsSqrt(BigInteger n, BigInteger root)
        {
            BigInteger lowerBound = root * root;
            BigInteger upperBound = (root + 1) * (root + 1);

            return (n >= lowerBound && n < upperBound);
        }

        private static Dictionary<BigInteger, BigInteger> PrecomputeDlogs(BigInteger b, BigInteger c, BigInteger p)
        {
            var d = new Dictionary<BigInteger, BigInteger>();

            var invb = Inv(b, p);

            var maxValue = Sqrt(p) + 1;

            for (int i = 0; i < maxValue; i++)
            {
                d.Add(SquareExp(invb, i % (p - 1), p) * c % p, i);
            }

            return d;
        }

        public static BigInteger LogMod(BigInteger b, BigInteger c, BigInteger p)
        {
            var dlogs = PrecomputeDlogs(b, c, p);

            var rt = Sqrt(p);

            for (BigInteger i = 0; i < p; i += rt)
            {
                var k = SquareExp(b, i, p) % p;

                if (dlogs.ContainsKey(k))
                    return i + dlogs[k];
            }

            return BigInteger.MinusOne;
        }
    }
}
