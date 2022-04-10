namespace Elgamal.Lib;

public class AsymmetricCipherKeyPair
{
    public IKey PublicKey { get; private set; }
    public IKey PrivateKey { get; private set; }

    public AsymmetricCipherKeyPair(IKey publicKey, IKey privateKey)
    {
        if (publicKey == null)
        {
            throw new ArgumentNullException("publicKey is null");
        }

        if (privateKey == null)
        {
            throw new ArgumentNullException("privateKey is null");
        }
        PublicKey = publicKey;
        PrivateKey = privateKey;
    }
}