using BenchmarkDotNet.Attributes;
using System.Text;

namespace Elgamal.Lib.Benchmark;

[MinColumn, MaxColumn, MeanColumn]
[HtmlExporter]
[SimpleJob(targetCount: 5)]
[RPlotExporter]
public class LegalEncryptionBenchmark
{

    private const string Message = "Hello world";
    private static readonly byte[] MessageBytes = Encoding.UTF8.GetBytes(Message);

    [ParamsSource(nameof(BitCounts))]
    public int BitCount;

    private AsymmetricCipherKeyPair _keys;
    private ElgamalParameters _params;

    public static IEnumerable<int> BitCounts => new[]
    {
        32,
        64,
        128,
        256,
        512,
        1024,
        2048,
        4096
    };

    [Benchmark]
    [MinIterationCount(5)]
    public bool LegalEncryption()
    {
        var @params = ElgamalParameters.Generate(BitCount);
        var keys = ElgamalKey.GenerateKeyPair(@params);

        var encrypted = Elgamal.Encrypt(MessageBytes, keys.PublicKey);
        var decrypted = Elgamal.Decrypt(encrypted, keys.PrivateKey);

        var decryptedMessage = Encoding.UTF8.GetString(decrypted)[..^1];

        return decryptedMessage != Message;
    }
}