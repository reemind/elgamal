using System.Numerics;

namespace Elgamal.Lib;

public class ParamsDto
{
    public byte[] P { get; set; }
    public byte[] G { get; set; }

    public static ParamsDto FromElgamalParameters(ElgamalParameters @params)
        => new()
        {
            G = @params.G.ToByteArray(),
            P = @params.P.ToByteArray(),
        };

    public ElgamalParameters ToElgamalParameters()
        => new ElgamalParameters(new BigInteger(P), new BigInteger(G));
}