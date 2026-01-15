using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace Titanic.API.Http;

public class MediaFireDownloader
{
    private readonly IHttpInterface _http;
    private const string UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/110.0.5481.178 Safari/537.36";

    public MediaFireDownloader(IHttpInterface http)
    {
        _http = http;
        _http.AddDefaultHeader("User-Agent", UserAgent);
    }

    public DownloadItem? FetchDirectDownloadUrl(string url)
    {
        try
        {
            // Fetch the mediafire page to extract the download link
            string pageContent = _http.RequestString(HttpMethodType.GET, url, null, null);
            string? downloadUrl = ExtractDownloadLink(pageContent);

            if (string.IsNullOrEmpty(downloadUrl))
                return null;

            string? filename = ExtractFileName(pageContent);
            return new DownloadItem(downloadUrl, filename);
        }
        catch (Exception ex)
        {
            throw new Exception($"Failed to fetch mediafire download link: {ex.Message}", ex);
        }
    }

    private static string? ExtractDownloadLink(string contents)
    {
        var match = Regex.Match(contents, @"href=""((https?)://download[^""]+)""");
        return match.Success ? match.Groups[1].Value : null;
    }

    private static string? ExtractFileName(string contents)
    {
        // Try to find filename in the page contents
        var match = Regex.Match(contents, @"<div class=""filename"">([^<]+)</div>");
        if (match.Success)
            return match.Groups[1].Value.Trim();

        // Try to extract from download link itself
        match = Regex.Match(contents, @"href=""https?://download[^""]+/([^/""]+)""");
        if (match.Success)
            return Uri.UnescapeDataString(match.Groups[1].Value);

        return null;
    }

    public class DownloadItem
    {
        public string DownloadUrl { get; set; }
        public string? Filename { get; set; }

        public DownloadItem(string downloadUrl, string? filename)
        {
            DownloadUrl = downloadUrl;
            Filename = filename;
        }
    }
}