namespace Crawler.Tests;

public class Uri_TryCreateShould
{
    [Theory]
    [InlineData("http://www.example.com", "http://www.example.com", "http://www.example.com/")]
    [InlineData("http://www.example.com", "http://www.other-example.com", "http://www.other-example.com/")]
    [InlineData("http://www.example.com", "/example", "http://www.example.com/example")]
    [InlineData("http://www.example.com/foldler1?query=abc&page=1", "/example", "http://www.example.com/example")]
    [InlineData("http://www.example.com/foldler1?query=abc&page=1", "/example?query=xyz", "http://www.example.com/example?query=xyz")]
    [InlineData("http://www.example.com", "mailto:ovais@me.com", "mailto:ovais@me.com")]
    [InlineData("http://www.example.com", "example", "http://www.example.com/example")]
    [InlineData("http://www.example.com/foldler1?query=abc&page=1", "example?query=xyz", "http://www.example.com/foldler1/example?query=xyz")]
    [InlineData("http://www.example.com/foldler1?query=abc&page=1", "example", "http://www.example.com/foldler1/example")]
    [InlineData("http://www.example.com/foldler1/folder2", "./example", "http://www.example.com/foldler1/folder2/example")]
    [InlineData("http://www.example.com/foldler1/index.html", "./example", "http://www.example.com/foldler1/example")]
    [InlineData("http://www.example.com/foldler1/folder2", "../example", "http://www.example.com/forlder1/example")]
    public void Create_Uri(string baseUri, string relativeUri, string expected)
    {
        var actual = Url.Combine(new Uri(baseUri), relativeUri)?.AbsoluteUri;
        Assert.Equal(expected, actual);
    }
}
