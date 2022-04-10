using Elgamal.Lib;
using System.Numerics;

namespace Elgamal.Server;

public static class PrimeGetter
{
    public static ElgamalParameters Parameters { get; }
    public static BigInteger P => Parameters.P;
    public static BigInteger G => Parameters.G;

    static PrimeGetter()
    {
        Parameters = ElgamalParameters.Generate(20);
    }
}