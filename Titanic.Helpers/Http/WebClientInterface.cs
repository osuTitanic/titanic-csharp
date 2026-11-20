#if !NET5_0_OR_GREATER
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;

namespace Titanic.Helpers.Http;

#nullable enable

public class WebClientInterface : IHttpInterface
{
    private readonly WebClient _client;
    private readonly Dictionary<string, string> _defaultHeaders = [];

    public WebClientInterface(string baseAddress)
    {
        this._client = new WebClient
        {
            BaseAddress = baseAddress,
            Encoding = Encoding.UTF8
        };
    }

    private void PrepareRequest(Dictionary<string, string>? headers)
    {
        // Assign via the pairs instead of Add(), so that request headers replace
        // defaults rather than merging into a comma-separated list
        _client.Headers.Clear();
        foreach (KeyValuePair<string, string> kvp in _defaultHeaders)
        {
            _client.Headers[kvp.Key] = kvp.Value;
        }

        if (headers != null)
        {
            foreach (KeyValuePair<string, string> kvp in headers)
            {
                _client.Headers[kvp.Key] = kvp.Value;
            }
        }
    }

    public string RequestString(HttpMethodType methodType, string endpoint, string? content, Dictionary<string, string>? headers)
    {
        lock (_client)
        {
            this.PrepareRequest(headers);
            return methodType switch
            {
                HttpMethodType.GET => this._client.DownloadString(endpoint),
                HttpMethodType.POST => this._client.UploadString(endpoint, content ?? ""),
                HttpMethodType.PUT => this._client.UploadString(endpoint, "PUT", content ?? ""),
                HttpMethodType.PATCH => this.Patch(endpoint, content),
                HttpMethodType.DELETE => this._client.UploadString(endpoint, "DELETE", content ?? ""),
                _ => throw new ArgumentOutOfRangeException(nameof(methodType), methodType, null)
            };
        }
    }

    private string Patch(string endpoint, string? content)
    {
        // WebClient does not support Patch, so we need to build the request manually
        Uri requestUrl = new(new Uri(_client.BaseAddress), endpoint);
        HttpWebRequest request = (HttpWebRequest)WebRequest.Create(requestUrl);
        request.Method = "PATCH";
        request.ContentType = "application/json";

        // Copy across the headers (like Authorization) from our WebClient to the request
        foreach (string headerKey in _client.Headers.AllKeys)
        {
            if (headerKey.Equals("Authorization", StringComparison.OrdinalIgnoreCase))
                request.Headers[HttpRequestHeader.Authorization] = _client.Headers[headerKey];
            else
                request.Headers[headerKey] = _client.Headers[headerKey];
        }

        // Write JSON body
        using (StreamWriter streamWriter = new(request.GetRequestStream()))
            streamWriter.Write(content);

        using WebResponse response = request.GetResponse();
        using StreamReader reader = new(response.GetResponseStream()!);

        return reader.ReadToEnd();
    }

    public byte[] RequestBytes(HttpMethodType methodType, string endpoint, string? content, Dictionary<string, string>? headers)
    {
        lock (_client)
        {
            this.PrepareRequest(headers);
            return methodType switch
            {
                HttpMethodType.GET => this._client.DownloadData(endpoint),
                HttpMethodType.POST => this._client.UploadData(endpoint, Encoding.UTF8.GetBytes(content ?? "")),
                HttpMethodType.PUT => this._client.UploadData(endpoint, "PUT", Encoding.UTF8.GetBytes(content ?? "")),
                HttpMethodType.PATCH => throw new NotImplementedException(),
                HttpMethodType.DELETE => this._client.UploadData(endpoint, "DELETE", Encoding.UTF8.GetBytes(content ?? "")),
                _ => throw new ArgumentOutOfRangeException(nameof(methodType), methodType, null)
            };
        }
    }

    public byte[] RequestBytes(HttpMethodType methodType, string endpoint, byte[]? content, Dictionary<string, string>? headers)
    {
        lock (_client)
        {
            this.PrepareRequest(headers);
            return methodType switch
            {
                HttpMethodType.GET => this._client.DownloadData(endpoint),
                HttpMethodType.POST => this._client.UploadData(endpoint, content ?? []),
                HttpMethodType.PUT => this._client.UploadData(endpoint, "PUT", content ?? []),
                HttpMethodType.PATCH => throw new NotImplementedException(),
                HttpMethodType.DELETE => this._client.UploadData(endpoint, "DELETE", content ?? []),
                _ => throw new ArgumentOutOfRangeException(nameof(methodType), methodType, null)
            };
        }
    }

    public void RequestEmpty(HttpMethodType methodType, string endpoint, string? content, Dictionary<string, string>? headers)
    {
        RequestString(methodType, endpoint, content, headers);
    }

    public void RequestEmpty(HttpMethodType methodType, string endpoint, byte[]? content, Dictionary<string, string>? headers)
    {
        RequestBytes(methodType, endpoint, content, headers);
    }

    public void AddDefaultHeader(string key, string value)
    {
        lock (_client)
            this._defaultHeaders[key] = value;
    }

    public void RemoveDefaultHeader(string key)
    {
        lock (_client)
            this._defaultHeaders.Remove(key);
    }
    
    public void Dispose()
    {
        this._client.Dispose();
        GC.SuppressFinalize(this);
    }
}
#endif
