using System.Numerics;

namespace Elgamal.Lib;

public class ElgamalKey : IKey
{
    public ElgamalParameters Parameters { get; }
    public BigInteger Key { get; }
    public bool IsPrivate { get; }


    public readonly int MaxOpenTextSize;
    public readonly int MaxCipherTextSize;

    public ElgamalKey(bool isPrivate, BigInteger key, ElgamalParameters parameters)
    {
        IsPrivate = isPrivate;

        if (parameters == null)
        {
            throw new ArgumentNullException(nameof(parameters));
        }
        Parameters = parameters;


        if (key < 2)
        {
            throw new ArgumentException("Ключ должен быть положительным");
        }

        if (isPrivate && (key >= (parameters.P - 1)))
        {
            throw new ArgumentException("Ключ не должен превышать P - 1");
        }
        if (!isPrivate && (key >= parameters.P))
        {
            throw new ArgumentException("Ключ не должен превышать P");
        }
        var modulusByteCount = Parameters.P.ToByteArray(false).Length;

        MaxOpenTextSize = modulusByteCount - 1;
        MaxCipherTextSize = modulusByteCount;

        Key = key;

    }

    public static AsymmetricCipherKeyPair GenerateKeyPair(ElgamalParameters parameters, BigInteger privateKeyInt)
    {
        BigInteger publicKeyInt = BigInteger.ModPow(parameters.G, privateKeyInt, parameters.P);

        ElgamalKey privateKey = new ElgamalKey(true, privateKeyInt, parameters);
        ElgamalKey publicKey = new ElgamalKey(false, publicKeyInt, parameters);
        return new AsymmetricCipherKeyPair(publicKey, privateKey);
    }

    public static AsymmetricCipherKeyPair GenerateKeyPair(ElgamalParameters parameters)
    {
        BigInteger privateKey = Helper.GenerateBigInteger(2, parameters.P - 1);
        return GenerateKeyPair(parameters, privateKey);
    }

}