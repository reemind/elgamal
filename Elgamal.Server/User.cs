using System.Numerics;

namespace Elgamal.Server;

public class User
{
    public string Name { get; set; }
    public BigInteger Key { get; set; }
    public bool IsMITM { get; set; }
}