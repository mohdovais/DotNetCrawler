using Crawler;


var spider = new Spider("https://learn.microsoft.com/en-us/aspnet/core/fundamentals/http-requests?view=aspnetcore-8.0");
await spider.CrawlAsync();

