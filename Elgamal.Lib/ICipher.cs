namespace Elgamal.Lib;

public interface ICipher
{
    byte[] Encrypt(byte[] text, IKey key = null);
    byte[] Decrypt(byte[] ciphertext, IKey key = null);
    string CipherName { get; }
}