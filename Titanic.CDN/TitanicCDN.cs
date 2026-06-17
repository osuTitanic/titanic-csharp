using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Titanic.Helpers.Http;

namespace Titanic.CDN
{
    public class TitanicCDN : IDisposable
    {
        #if NET8_0_OR_GREATER
        private const DynamicallyAccessedMemberTypes types = DynamicallyAccessedMemberTypes.PublicConstructors |
                                                             DynamicallyAccessedMemberTypes.NonPublicConstructors |
                                                             DynamicallyAccessedMemberTypes.PublicParameterlessConstructor |
                                                             DynamicallyAccessedMemberTypes.PublicProperties |
                                                             DynamicallyAccessedMemberTypes.NonPublicProperties |
                                                             DynamicallyAccessedMemberTypes.PublicMethods |
                                                             DynamicallyAccessedMemberTypes.NonPublicMethods;
        #endif

#pragma warning disable CA1859
        private readonly IHttpInterface _http;
#pragma warning restore CA1859

        private string? _accessKey;

        private string userAgent => $"Titanic.CDN/{packageVersion}";
        private string packageVersion => typeof(TitanicCDN).Assembly.GetName().Version?.ToString() ?? "Unknown";

        public string? AccessKey
        {
            get => _accessKey;
            set
            {
                _accessKey = value;

                this._http.RemoveDefaultHeader("Authorization");
                if (!string.IsNullOrEmpty(value))
                    this._http.AddDefaultHeader("Authorization", $"Bearer {value}");
            }
        }

        public bool HasAccessKey => !string.IsNullOrEmpty(AccessKey);

        public TitanicCDN(string baseUrl = "https://cdn.titanic.sh")
            : this(HttpInterfaceFactory.Create(baseUrl))
        {
        }

        internal TitanicCDN(IHttpInterface http)
        {
            this._http = http;
            this._http.AddDefaultHeader("User-Agent", userAgent);
        }

#if NET8_0_OR_GREATER
        [UnconditionalSuppressMessage(
            "Trimming",
            "IL2026",
            Justification = "The contract resolver is only used within already trim-compatible methods."
        )]
#endif
        private static readonly JsonSerializerSettings _settings = new()
        {
            ContractResolver = new DefaultContractResolver
            {
                NamingStrategy = new SnakeCaseNamingStrategy()
            }
        };

#if NET8_0_OR_GREATER
        [UnconditionalSuppressMessage(
            "Trimming",
            "IL2026",
            Justification = "The appropriate code is marked as untrimmed."
        )]
#endif
        public T Get
#if NET8_0_OR_GREATER
            <[DynamicallyAccessedMembers(types)] T>
#else
            <T>
#endif
            (string endpoint, Dictionary<string, string>? headers = null)
        {
            Debug.Print("TitanicCDN: GET " + endpoint);
            string str = this._http.RequestString(HttpMethodType.GET, endpoint, null, headers);

            T? obj = JsonConvert.DeserializeObject<T>(str, _settings);
            if (obj == null)
                throw new Exception("Response had null content");

            return obj;
        }

        public string GetString(string endpoint, Dictionary<string, string>? headers = null)
        {
            Debug.Print("TitanicCDN: GET " + endpoint);
            return this._http.RequestString(HttpMethodType.GET, endpoint, null, headers);
        }

        public byte[] Download(string objectKey, string? range = null)
        {
            Dictionary<string, string>? headers = null;
            if (!string.IsNullOrEmpty(range))
                headers = new Dictionary<string, string> { ["Range"] = range! };

            string endpoint = "/" + EscapeObjectKey(objectKey);
            Debug.Print("TitanicCDN: GET " + endpoint);
            return this._http.RequestBytes(HttpMethodType.GET, endpoint, (string?)null, headers);
        }

#if NET8_0_OR_GREATER
        [UnconditionalSuppressMessage(
            "Trimming",
            "IL2026",
            Justification = "The appropriate code is marked as untrimmed."
        )]
#endif
        public T Put
#if NET8_0_OR_GREATER
            <[DynamicallyAccessedMembers(types)] T>
#else
            <T>
#endif
            (string endpoint, byte[] data, Dictionary<string, string>? headers = null)
        {
            Debug.Print("TitanicCDN: PUT " + endpoint);
            byte[] bytes = this._http.RequestBytes(HttpMethodType.PUT, endpoint, data, headers);
            string str = Encoding.UTF8.GetString(bytes);

            T? obj = JsonConvert.DeserializeObject<T>(str, _settings);
            if (obj == null)
                throw new Exception("Response had null content");

            return obj;
        }

#if NET8_0_OR_GREATER
        [UnconditionalSuppressMessage(
            "Trimming",
            "IL2026",
            Justification = "The appropriate code is marked as untrimmed."
        )]
#endif
        public T Post
#if NET8_0_OR_GREATER
            <[DynamicallyAccessedMembers(types)] T>
#else
            <T>
#endif
            (string endpoint, Dictionary<string, string>? headers = null)
        {
            Debug.Print("TitanicCDN: POST " + endpoint);
            byte[] bytes = this._http.RequestBytes(HttpMethodType.POST, endpoint, (string?)null, headers);
            string str = Encoding.UTF8.GetString(bytes);

            T? obj = JsonConvert.DeserializeObject<T>(str, _settings);
            if (obj == null)
                throw new Exception("Response had null content");

            return obj;
        }

        public void Delete(string endpoint, Dictionary<string, string>? headers = null)
        {
            Debug.Print("TitanicCDN: DELETE " + endpoint);
            this._http.RequestEmpty(HttpMethodType.DELETE, endpoint, (string?)null, headers);
        }

        internal void EnsureAccessKey()
        {
            if (!HasAccessKey)
                throw new InvalidOperationException("An access key is required for this request.");
        }

        internal static string EscapeObjectKey(string objectKey)
        {
            if (objectKey == null || objectKey.Trim().Length == 0)
                throw new ArgumentException("Object key must not be empty.", nameof(objectKey));

            string trimmed = objectKey.TrimStart('/');
            if (trimmed.Length == 0)
                throw new ArgumentException("Object key must not be empty.", nameof(objectKey));

            string[] parts = trimmed.Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
            List<string> escaped = new List<string>();
            foreach (string part in parts)
                escaped.Add(Uri.EscapeDataString(part));

            return string.Join("/", escaped.ToArray());
        }

        internal static string EscapeQueryParameter(string value)
        {
            return Uri.EscapeDataString(value ?? "");
        }

        public void Dispose()
        {
            this._http.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
