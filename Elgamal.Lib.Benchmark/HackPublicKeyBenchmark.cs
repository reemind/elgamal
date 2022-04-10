using BenchmarkDotNet.Attributes;

namespace Elgamal.Lib.Benchmark;

[MinColumn, MaxColumn, MeanColumn]
[HtmlExporter]
[ShortRunJob]
[RPlotExporter]
[EvaluateOverhead(false)]
public class HackPublicKeyBenchmark
{
    [ParamsSource(nameof(BitCounts))]
    public int BitCount;

    // 32 -> 64
    public static IEnumerable<int> BitCounts => Enumerable.Range(32, 32 + 1).ToList();

    [Benchmark]
    [IterationCount(1)]
    [WarmupCount(0)]
    public void HackKey()
    {
        var @params = ElgamalParameters.Generate(BitCount);

        var keys = ElgamalKey.GenerateKeyPair(@params);

        BabyStepGiantStepImplementation.LogMod(
            @params.G,
            (keys.PublicKey as ElgamalKey)!.Key,
            @params.P);
    }
}