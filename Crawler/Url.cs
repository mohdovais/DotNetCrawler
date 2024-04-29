using System.Text.RegularExpressions;

namespace Crawler;

public class Url
{
    private readonly static Uri FakeBase = new Uri("http://www.example.com");

    // https://user:password@www.contoso.com:80/Home/Index.htm?q1=v1&q2=v2#FragmentName
    // AbsolutePath: /Home/Index.htm
    // AbsoluteUri: https://user:password@www.contoso.com:80/Home/Index.htm?q1=v1&q2=v2#FragmentName
    // DnsSafeHost: www.contoso.com
    // Fragment: #FragmentName
    // Host: www.contoso.com
    // HostNameType: Dns
    // IdnHost: www.contoso.com
    // IsAbsoluteUri: True
    // IsDefaultPort: False
    // IsFile: False
    // IsLoopback: False
    // IsUnc: False
    // LocalPath: /Home/Index.htm
    // OriginalString: https://user:password@www.contoso.com:80/Home/Index.htm?q1=v1&q2=v2#FragmentName
    // PathAndQuery: /Home/Index.htm?q1=v1&q2=v2
    // Port: 80
    // Query: ?q1=v1&q2=v2
    // Scheme: https
    // Segments: /, Home/, Index.htm
    // UserEscaped: False
    // UserInfo: user:password
    public static Uri? Combine(Uri baseUri, string relativeUrl)
    {
        if (relativeUrl.StartsWith('/') || Regex.IsMatch(relativeUrl, "^[a-zA-Z]+:"))
        {
            Uri.TryCreate(baseUri, relativeUrl, out var result);
            return result;
        }

        if (Uri.TryCreate(FakeBase, relativeUrl, out var result2))
        {
            UriBuilder builder = new(baseUri)
            {
                Path = baseUri.AbsolutePath.TrimEnd('/') + result2.AbsolutePath,
                Query = result2.Query
            };

            return builder.Uri;
        }

        return null;
    }
}
