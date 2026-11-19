using System.Net;
using System.Net.Sockets;
using System.Text.RegularExpressions;

namespace ClickbaitDecoder.Services;

public sealed record Article(string Headline, IReadOnlyList<string> Paragraphs, string Source);

/// <summary>
/// Downloads a public web page and pulls out the headline and paragraph text with plain regex.
/// No HTML-parser dependency on purpose (demo app). Good enough for most news articles. Paywalled or
/// JavaScript-rendered pages will come back thin, and the UI offers "paste the text" for those.
/// </summary>
public sealed partial class ArticleFetcher(HttpClient http)
{
    const int MaxParagraphs = 40;

    public async Task<Article> FetchAsync(string url, CancellationToken ct = default)
    {
        if (!Uri.TryCreate(url.Trim(), UriKind.Absolute, out var uri) || uri.Scheme is not ("http" or "https"))
            throw new ArticleException("That doesn't look like a web address. It should start with http:// or https://");

        await EnsurePublicHostAsync(uri, ct);

        string html;
        try
        {
            html = await http.GetStringAsync(uri, ct);
        }
        catch (HttpRequestException ex)
        {
            throw new ArticleException($"The site said no ({(ex.StatusCode is { } s ? (int)s + " " + s : ex.Message)}). Many news sites block robots. Try pasting the text instead.");
        }
        catch (TaskCanceledException)
        {
            throw new ArticleException("The site took too long to answer. Try pasting the text instead.");
        }

        // Headline first: many sites put the <h1> inside a <header>, which the junk filter removes.
        var h1 = Clean(H1().Match(html).Groups["body"].Value);
        html = Junk().Replace(html, " ");
        var paragraphs = new List<string>();
        foreach (Match m in Paragraphs().Matches(html))
        {
            var text = Clean(m.Groups["body"].Value);
            if (text.Length >= 60 && paragraphs.Count < MaxParagraphs) paragraphs.Add(text);
        }

        var headline = !string.IsNullOrWhiteSpace(h1) ? h1 : Clean(Title().Match(html).Groups["body"].Value);
        if (paragraphs.Count == 0)
            throw new ArticleException("Couldn't find any article text on that page (paywall or JavaScript-only site?). Try pasting the text instead.");

        return new Article(string.IsNullOrWhiteSpace(headline) ? uri.Host : headline, paragraphs, uri.Host);
    }

    /// <summary>Pasted text: first short line is the headline, the rest split on line breaks.</summary>
    public static Article FromText(string text)
    {
        var lines = text.Split('\n', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                        .Where(l => l.Length > 0).ToList();
        if (lines.Count == 0) throw new ArticleException("Paste some article text first.");
        var headline = lines[0].Length <= 200 ? lines[0] : "(pasted article)";
        var body = lines[0].Length <= 200 ? lines.Skip(1) : lines;
        return new Article(headline, body.Take(MaxParagraphs).ToList(), "pasted text");
    }

    // Local demo app, but don't let the server be pointed at itself or the internal network.
    static async Task EnsurePublicHostAsync(Uri uri, CancellationToken ct)
    {
        IPAddress[] addresses;
        try { addresses = await Dns.GetHostAddressesAsync(uri.DnsSafeHost, ct); }
        catch (SocketException) { throw new ArticleException($"Couldn't find the site '{uri.Host}'. Check the address?"); }

        if (addresses.Length == 0 || addresses.Any(IsPrivate))
            throw new ArticleException("Only public websites, please. That address points at a private or local network.");
    }

    static bool IsPrivate(IPAddress ip)
    {
        if (IPAddress.IsLoopback(ip)) return true;
        if (ip.IsIPv4MappedToIPv6) ip = ip.MapToIPv4();
        if (ip.AddressFamily == AddressFamily.InterNetworkV6)
            return ip.IsIPv6LinkLocal || ip.IsIPv6SiteLocal || ip.IsIPv6UniqueLocal;
        var b = ip.GetAddressBytes();
        return b[0] == 10 || b[0] == 0 || (b[0] == 172 && b[1] is >= 16 and <= 31) ||
               (b[0] == 192 && b[1] == 168) || (b[0] == 169 && b[1] == 254) || (b[0] == 100 && b[1] is >= 64 and <= 127);
    }

    // <br> becomes a space; every other tag is removed outright so "<a>word</a>," stays "word,".
    static string Clean(string fragment)
    {
        var text = Tags().Replace(LineBreaks().Replace(fragment, " "), "");
        text = Citations().Replace(WebUtility.HtmlDecode(text), "");
        return Spaces().Replace(text, " ").Trim();
    }

    [GeneratedRegex(@"<(script|style|noscript|svg|nav|header|footer|aside|form|figure|button)\b[^>]*>.*?</\1\s*>", RegexOptions.Singleline | RegexOptions.IgnoreCase)]
    private static partial Regex Junk();

    [GeneratedRegex(@"<p\b[^>]*>(?<body>.*?)</p\s*>", RegexOptions.Singleline | RegexOptions.IgnoreCase)]
    private static partial Regex Paragraphs();

    [GeneratedRegex(@"<h1\b[^>]*>(?<body>.*?)</h1\s*>", RegexOptions.Singleline | RegexOptions.IgnoreCase)]
    private static partial Regex H1();

    [GeneratedRegex(@"<br\s*/?>", RegexOptions.IgnoreCase)]
    private static partial Regex LineBreaks();

    [GeneratedRegex(@"\[\s*(\d+|citation needed|note \d+)\s*\]", RegexOptions.IgnoreCase)]
    private static partial Regex Citations();

    [GeneratedRegex(@"<title\b[^>]*>(?<body>.*?)</title>", RegexOptions.Singleline | RegexOptions.IgnoreCase)]
    private static partial Regex Title();

    [GeneratedRegex(@"<[^>]+>")]
    private static partial Regex Tags();

    [GeneratedRegex(@"\s+")]
    private static partial Regex Spaces();
}

public sealed class ArticleException(string message) : Exception(message);
