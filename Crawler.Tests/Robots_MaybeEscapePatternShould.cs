namespace Crawler.Tests;

public class Robots_MaybeEscapePatternShould
{
    [Theory]
    [InlineData("http://www.example.com", "http://www.example.com")]
    [InlineData("/a/b/c", "/a/b/c")]
    [InlineData("á", "%C3%A1")]
    [InlineData("%aa", "%AA")]
    public void Work_as_expected(string path, string expected)
    {
        Robots.MaybeEscapePattern(path, out var actual);
        Assert.Equal(expected, actual);
    }
}
