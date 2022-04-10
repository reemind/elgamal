using Aprismatic;
using System.Numerics;
using System.Security.Cryptography;

namespace Elgamal.Lib;

public class ElgamalParameters
{


    static ElgamalParameters()
    {

    }


    private readonly BigInteger _p;
    private readonly BigInteger _g;

    public BigInteger P { get => _p; }
    public BigInteger G { get => _g; }


    public ElgamalParameters(BigInteger p, BigInteger g)
    {
        _p = p;
        _g = g;
    }

    public static ElgamalParameters Generate(int bitsNumberCount)
    {
        if (bitsNumberCount < 32)
        {
            throw new ArgumentException("Слишком маленький порядок числа");
        }

        BigInteger min = BigInteger.Pow(2, bitsNumberCount - 2);

        var random = RandomNumberGenerator.Create();

        BigInteger p = new BigInteger().GenSafePseudoPrime(bitsNumberCount, 8, random);
        BigInteger q = (p - 1) / 2;

        BigInteger g;

        do
        {
            g = Helper.GenerateBigInteger(2, p - 1);
        } while (BigInteger.ModPow(g, 2, p) == BigInteger.One ||
                 BigInteger.ModPow(g, q, p) == BigInteger.One);

        return new ElgamalParameters(p, g);
    }
}