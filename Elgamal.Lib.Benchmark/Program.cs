using BenchmarkDotNet.Running;
using Elgamal.Lib.Benchmark;

//var switcher = BenchmarkSwitcher.FromAssembly(Assembly.GetExecutingAssembly());

//switcher.RunAll(config);

BenchmarkRunner.Run(typeof(LegalEncryptionBenchmark));
//BenchmarkRunner.Run(typeof(HackPublicKeyBenchmark));