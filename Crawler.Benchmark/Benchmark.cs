using BenchmarkDotNet.Attributes;

namespace Crawler.Benchmark;

[MinColumn, MaxColumn, MemoryDiagnoser]
public class Benchamrks
{
    private const string path = "/global/en/wealth-management/insights/chief-investment-office/market-insights/paul-donovan/2023/things-which-do-not-matter.html";
    private const string pattern = "/*search.html?querystring";

    [Benchmark]
    public void Orginal()
    {
        for (int i = 0; i < 1000; i++)
        {
            Robots.Matches(path, pattern);
        }
    }
}