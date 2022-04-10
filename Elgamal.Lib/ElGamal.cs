using System.Numerics;

namespace Elgamal.Lib;

public static class Elgamal
{

    public static byte[] Decrypt(byte[] cipherText, IKey key)
    {
        Validate(cipherText, key);

        ElgamalKey? elgamalKey = key as ElgamalKey;
        if (!elgamalKey.IsPrivate)
        {
            throw new ArgumentException("Для расшифровывания нужен закрытый ключ");
        }
        List<byte> result = new List<byte>(cipherText.Length);

        try
        {
            int readSize = elgamalKey.MaxCipherTextSize;
            int writeSize = elgamalKey.MaxOpenTextSize;

            byte[] rBlock = new byte[readSize + 1];
            Buffer.BlockCopy(cipherText, 0, rBlock, 0, readSize);
            BigInteger r = new BigInteger(rBlock);
            BigInteger decryptConst = BigInteger.ModPow(r, elgamalKey.Parameters.P - 1 - elgamalKey.Key, elgamalKey.Parameters.P);

            for (int currentByte = readSize; currentByte < cipherText.Length; currentByte += readSize)
            {
                int byteCopyCount = Math.Min(readSize, cipherText.Length - currentByte);
                byte[] currentBlock = new byte[byteCopyCount + 1]; // Добавлен 0х00 чтобы число было положительным 

                Buffer.BlockCopy(cipherText, currentByte, currentBlock, 0, byteCopyCount);
                BigInteger cipherInt = new BigInteger(currentBlock);
                BigInteger openInt;
                BigInteger.DivRem(cipherInt * decryptConst, elgamalKey.Parameters.P, out openInt);

                byte[] openBlock = openInt.ToByteArray(false);
                byte[] packedBlock = new byte[writeSize];

                Buffer.BlockCopy(openBlock, 0, packedBlock, 0, openBlock.Length);
                result.AddRange(packedBlock);
            }

        }
        catch (Exception ex)
        {
            throw new Exception("Ошибка! Проверьте входные параметры.", ex);
        }

        return result.ToArray();
    }

    public static byte[] Encrypt(byte[] text, IKey key = null)
    {
        Validate(text, key);

        ElgamalKey? elgamalKey = key as ElgamalKey;

        if (elgamalKey?.IsPrivate ?? false)
        {
            throw new ArgumentException("Для шифрования нужен публичный ключ");
        }

        List<byte> result = new List<byte>(text.Length);

        try
        {
            int readSize = elgamalKey.MaxOpenTextSize;
            int writeSize = elgamalKey.MaxCipherTextSize;

            BigInteger k = Helper.GenerateBigInteger(2, elgamalKey.Parameters.P - 3);

            BigInteger r = BigInteger.ModPow(elgamalKey.Parameters.G, k, elgamalKey.Parameters.P);

            byte[] packedBlock = new byte[writeSize];

            byte[] rBlock = r.ToByteArray(false);
            Buffer.BlockCopy(rBlock, 0, packedBlock, 0, rBlock.Length);
            result.AddRange(packedBlock);

            for (int currentByte = 0; currentByte < text.Length; currentByte += readSize)
            {
                int byteCopyCount = Math.Min(readSize, text.Length - currentByte);
                byte[] currentBlock = new byte[byteCopyCount + 1]; // Добавлен 0х00 чтобы число было положительным 

                Buffer.BlockCopy(text, currentByte, currentBlock, 0, byteCopyCount);
                BigInteger openInt = new BigInteger(currentBlock);
                BigInteger cipherInt = BigInteger.ModPow(elgamalKey.Key, k, elgamalKey.Parameters.P);
                BigInteger.DivRem(openInt * cipherInt, elgamalKey.Parameters.P, out cipherInt);

                byte[] cipherBlock = cipherInt.ToByteArray(false);
                packedBlock = new byte[writeSize];
                Buffer.BlockCopy(cipherBlock, 0, packedBlock, 0, cipherBlock.Length);
                result.AddRange(packedBlock);

            }


        }
        catch (Exception ex)
        {
            throw new Exception("Ошибка! Проверьте входные параметры.", ex);
        }

        return result.ToArray();
    }

    private static void Validate(byte[] input, IKey key)
    {
        if (input == null)
        {
            throw new ArgumentNullException("Пустые входные данные");
        }
        if (input.Length == 0)
        {
            throw new ArgumentNullException("Пустые входные данные");
        }
        if (key is ElgamalKey == false)
        {
            throw new ArgumentException("Неверный ключ");
        }
    }
}