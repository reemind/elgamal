using System.Numerics;
using System.Security.Cryptography;

namespace Elgamal.Lib;

public static class Helper
{

    public static BigInteger GenerateBigInteger(BigInteger min, BigInteger max)
    {
        if (min >= max)
        {
            throw new ArgumentException("Min >= max");
        }
        if (min.Sign != 1)
        {
            throw new ArgumentException("min must be positive");
        }
        if (max.Sign != 1)
        {
            throw new ArgumentException("max must be positive");
        }

        BigInteger diff = max - min;
        int byteSize = diff.ToByteArray().Length;

        byte[] newBigIntergerChunk = new byte[byteSize + 1]; //  Последний байт всегда 0х00 для положительного числа

        var rngGenerator = RandomNumberGenerator.Create();
        rngGenerator.GetBytes(newBigIntergerChunk);

        newBigIntergerChunk[^1] = 0;


        BigInteger result = new BigInteger(newBigIntergerChunk);

        BigInteger.DivRem(result, diff, out result);

        result += min;

        return result;

    }
}