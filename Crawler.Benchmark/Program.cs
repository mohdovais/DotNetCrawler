using BenchmarkDotNet.Running;
using Crawler.Benchmark;

var summary = BenchmarkRunner.Run<Benchamrks>();