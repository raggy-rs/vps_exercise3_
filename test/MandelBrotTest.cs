namespace exercise3_test;

using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Loggers;
using BenchmarkDotNet.Running;

using MandelbrotGenerator;

public class MandelBrotTest
{
    [Fact]
    public void MandelBrotGeneratorPerformance()
    {
        var logger = new AccumulationLogger();

        var config = ManualConfig.Create(DefaultConfig.Instance)
            .AddLogger(logger)
            .WithOptions(ConfigOptions.DisableOptimizationsValidator);

        BenchmarkRunner.Run<MandelbrotGeneratorBenchmarks>(config);
    }
}

 [MaxIterationCount(20)]
 public class MandelbrotGeneratorBenchmarks {
    [Benchmark]
    public void SyncImageGeneratorPerformance() {
        var area = new Area();
        var generator = new SyncImageGenerator();
        var image = generator.GenerateImage(area);
    }
 }