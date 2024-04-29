namespace Crawler.Tests;

public class Robots_MatchesShould
{
    [Theory]
    [InlineData("/some-page/search.do?query=hello", "/*search.do?")]
    public void Work_as_expected(string path, string pattern)
    {
        Assert.True(Robots.Matches(path, pattern));
    }
}
