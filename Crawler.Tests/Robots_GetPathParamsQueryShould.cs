namespace Crawler.Tests;

public class Robots_GetPathParamsQueryShould
{
    [Theory]
    [InlineData("", "/")]
    [InlineData("http://www.example.com", "/")]
    [InlineData("http://www.example.com/", "/")]
    [InlineData("http://www.example.com/a", "/a")]
    [InlineData("http://www.example.com/a/", "/a/")]
    [InlineData("http://www.example.com/a/b?c=http://d.e/", "/a/b?c=http://d.e/")]
    [InlineData("http://www.example.com/a/b?c=d&e=f#fragment", "/a/b?c=d&e=f")]
    [InlineData("example.com", "/")]
    [InlineData("example.com/", "/")]
    [InlineData("example.com/a", "/a")]
    [InlineData("example.com/a/", "/a/")]
    [InlineData("example.com/a/b?c=d&e=f#fragment", "/a/b?c=d&e=f")]
    [InlineData("a", "/")]
    [InlineData("a/", "/")]
    [InlineData("/a", "/a")]
    [InlineData("a/b", "/b")]
    [InlineData("example.com?a", "/?a")]
    [InlineData("example.com/a]b#c", "/a]b")]
    [InlineData("//a/b/c", "/b/c")]
    public void Return_path_for_correctly_scaped_urls(string path, string expected)
    {
        // Only testing URLs that are already correctly escaped here.
        var actual = Robots.GetPathParamsQuery(path);
        
        Assert.Equal(expected, actual);
    }
}
