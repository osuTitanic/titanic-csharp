#if SUPPORT_HTTPCLIENT
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace Titanic.API.Http;

#nullable enable

public class HttpClientInterface : IHttpInterface
{
    private readonly HttpClient _client;

    public HttpClientInterface(string baseAddress)
    {
        _client = new HttpClient
        {
            BaseAddress = new Uri(baseAddress)
        };
    }

    private HttpResponseMessage Send(HttpMethodType methodType, string endpoint, string? content, Dictionary<string, string>? headers)
    {
        HttpMethod method = methodType switch
        {
            HttpMethodType.GET => HttpMethod.Get,
            HttpMethodType.POST => HttpMethod.Post,
            HttpMethodType.PUT => HttpMethod.Put,
            #if NET5_0_OR_GREATER
            HttpMethodType.PATCH => HttpMethod.Patch,
            #else
            HttpMethodType.PATCH => new HttpMethod("PATCH"),
            #endif
            HttpMethodType.DELETE => HttpMethod.Delete,
            _ => throw new ArgumentOutOfRangeException(nameof(methodType), methodType, null)
        };

        HttpRequestMessage request = new(method, endpoint);
        if (headers != null)
        {
            foreach (KeyValuePair<string, string> kvp in headers)
            {
                request.Headers.TryAddWithoutValidation(kvp.Key, kvp.Value);
            }
        }

        if (content != null)
        {
            request.Content = new StringContent(content, Encoding.UTF8, "application/json");
        }

        HttpResponseMessage response = this._client.SendAsync(request).Result;
        response.EnsureSuccessStatusCode();

        return response;
    }

    public string RequestString(HttpMethodType methodType, string endpoint, string? content, Dictionary<string, string>? headers)
    {
        HttpResponseMessage response = Send(methodType, endpoint, content, headers);
        return response.Content.ReadAsStringAsync().Result;
    }

    public byte[] RequestBytes(HttpMethodType methodType, string endpoint, string? content, Dictionary<string, string>? headers)
    {
        HttpResponseMessage response = Send(methodType, endpoint, content, headers);
        return response.Content.ReadAsByteArrayAsync().Result;
    }

    public void AddDefaultHeader(string key, string value)
    {
        this._client.DefaultRequestHeaders.TryAddWithoutValidation(key, value);
    }

    public void RemoveDefaultHeader(string key)
    {
        this._client.DefaultRequestHeaders.Remove(key);
    }

    public void Dispose()
    {
        this._client.Dispose();
        GC.SuppressFinalize(this);
    }
}
#endif