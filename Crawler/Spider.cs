using Fizzler.Systems.HtmlAgilityPack;
using HtmlAgilityPack;

namespace Crawler;

public class Spider : IDisposable
{
    private const string UserAgent = "Mozilla/5.0 (Macintosh; Intel Mac OS X 10_15_7) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/121.0.0.0 Safari/537.36";
    private readonly string _baseUrl;
    private readonly Queue<string> _queue;
    private readonly Dictionary<string, bool> _seen;
    private readonly HttpClient _httpClient;
    private bool _crawling = false;
    private bool disposedValue;

    public int MillisecondsDelay { get; set; } = 5000;
    public int MaxPages { get; set; } = 5;

    public Spider(string startUrl)
    {
        var url = new Uri(startUrl);
        _baseUrl = $"{url.Scheme}://{url.Host}";
        _queue = new Queue<string>();
        _seen = new Dictionary<string, bool>();
        _httpClient = new();

        _httpClient.DefaultRequestHeaders.UserAgent.ParseAdd(UserAgent);
    }

    async public Task CrawlAsync()
    {
        if (_crawling)
        {
            return;
        }

        _crawling = true;

        while (_queue.TryDequeue(out var url))
        {
            if (string.IsNullOrEmpty(url) || _seen.ContainsKey(url))
            {
                continue;
            }

            await VisitUrl(url);

            if (MaxPages > 0 && _seen.Count() > MaxPages - 1)
            {
                break;
            }

            await Task.Delay(MillisecondsDelay);
        }
    }

    async private Task VisitUrl(string url)
    {
        using var response = await _httpClient.GetAsync(url).ConfigureAwait(false);

        _seen.Add(url, true);

        if (!response.IsSuccessStatusCode)
        {
            return;
        }

        var responseText = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

        if (string.IsNullOrEmpty(responseText))
        {
            return;
        }

        // @TODO
        // Save responseText

        EnqueueLinks(responseText, url);

    }

    private void EnqueueLinks(string html, string baseUrl)
    {
        var htmlDocument = new HtmlDocument();
        htmlDocument.LoadHtml(html);
        var links = htmlDocument.DocumentNode.QuerySelectorAll("a");
        var baseUri = new Uri(baseUrl);

        foreach (var link in links)
        {
            var href = link.Attributes["href"]?.Value;

            if (string.IsNullOrEmpty(href))
            {
                continue;
            }

            href = Url.Combine(baseUri, href)?.AbsolutePath;

            //other cases missing

            if (!string.IsNullOrEmpty(href) && href.StartsWith(_baseUrl) && !_seen.ContainsKey(href))
            {
                _queue.Enqueue(href);
            }
        }
    }

    async private Task GetRobotsTxt()
    {
        var robotsTxtUrl = $"{_baseUrl}/robots.txt";
        using var response = await _httpClient.GetAsync(robotsTxtUrl).ConfigureAwait(false);

        if (!response.IsSuccessStatusCode)
        {
            return; // Array.Empty<string>();
        }

        using var stream = await response.Content.ReadAsStreamAsync().ConfigureAwait(false);

        if (stream != null)
        {
            RobotsTxtParser.Parse(stream);
        }
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!disposedValue)
        {
            if (disposing)
            {
                // TODO: dispose managed state (managed objects)
                _httpClient.Dispose();
            }

            // TODO: free unmanaged resources (unmanaged objects) and override finalizer
            // TODO: set large fields to null
            _queue.Clear();
            _seen.Clear();

            disposedValue = true;
        }
    }

    // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
    // ~Spider()
    // {
    //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
    //     Dispose(disposing: false);
    // }

    void IDisposable.Dispose()
    {
        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}
