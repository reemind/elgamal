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

    [IterationSetup(Target = nameof(LegalEncryption))]
    public void EncryptionSetup()
    {
        _params = ElgamalParameters.Generate(BitCount);
        _keys = ElgamalKey.GenerateKeyPair(_params);
    }

    [Benchmark]
    [MinIterationCount(5)]
    public bool LegalEncryption()
    {
        var encrypted = Elgamal.Encrypt(MessageBytes, _keys.PublicKey);
        var decrypted = Elgamal.Decrypt(encrypted, _keys.PrivateKey);

        var decryptedMessage = Encoding.UTF8.GetString(decrypted)[..^1];

        return decryptedMessage != Message;
    }

    [IterationSetup(Target = nameof(KeyGeneration))]
    public void KeyGenerationSetup()
    {
        _params = ElgamalParameters.Generate(BitCount);
    }

    [Benchmark]
    [MinIterationCount(5)]
    public void KeyGeneration()
    {
        _ = ElgamalKey.GenerateKeyPair(_params);
    }

    [Benchmark]
    [MinIterationCount(5)]
    public void ParamsGeneration()
    {
        _ = ElgamalParameters.Generate(BitCount);
    }
}