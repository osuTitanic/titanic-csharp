using System;
using System.Collections.Generic;
using System.Diagnostics;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Titanic.API.Http;
using Titanic.API.Models;
using Titanic.API.Requests;

namespace Titanic.API
{
    public class TitanicAPI : IDisposable
    {
#pragma warning disable CA1859
        private readonly IHttpInterface _http;
#pragma warning restore CA1859

        private string userAgent => $"Titanic.API/{packageVersion}";
        private string packageVersion => typeof(TitanicAPI).Assembly.GetName().Version?.ToString() ?? "Unknown";

        public TokenModel Token
        {
            get;
            set
            {
                field = value;

                this._http.RemoveDefaultHeader("Authorization");
                this._http.AddDefaultHeader("Authorization", $"Bearer {value.AccessToken}");
            }
        }

        public bool IsLoggedIn => Token != null;
        public bool IsTokenExpired => Token == null || DateTime.Now > Token.ExpiresAt;

        public TitanicAPI(string baseUrl = "https://api.titanic.sh")
        {
            this._http = HttpInterfaceFactory.Create(baseUrl);
            this._http.AddDefaultHeader("User-Agent", userAgent);
        }
        
        private static readonly JsonSerializerSettings _settings = new()
        {
            ContractResolver = new DefaultContractResolver
            {
                NamingStrategy = new SnakeCaseNamingStrategy()
            }
        };
        
        private T Send<T>(HttpMethodType methodType, string endpoint, object content = null, Dictionary<string, string> headers = null, bool checkToken = true)
        {
            if (checkToken)
                EnsureValidAccessToken();
            
            string jsonContent = null;
            
            // Only serialize content for methods that support a body
            if (content != null && methodType != HttpMethodType.GET)
                jsonContent = JsonConvert.SerializeObject(content, _settings);
            
            string str = this._http.RequestString(
                methodType, endpoint,
                jsonContent, headers
            );

            T obj = JsonConvert.DeserializeObject<T>(str, _settings);
            if (obj == null)
                throw new Exception("Response had null content");

            return obj;
        }

        public T Get<T>(string endpoint, Dictionary<string, string> headers = null)
        {
            Debug.Print("TitanicAPI: GET " + endpoint);
            return this.Send<T>(HttpMethodType.GET, endpoint, null, headers);
        }

        public T Post<T>(string endpoint, object data, Dictionary<string, string> headers = null)
        {
            Debug.Print("TitanicAPI: POST " + endpoint);
            // NOTE: we skip token checking for the refresh endpoint to avoid infinite loops
            return this.Send<T>(HttpMethodType.POST, endpoint, data, headers, endpoint != "/account/refresh");
        }

        public T Put<T>(string endpoint, object data, Dictionary<string, string> headers = null)
        {
            Debug.Print("TitanicAPI: PUT " + endpoint);
            return this.Send<T>(HttpMethodType.PUT, endpoint, data, headers);
        }

        public T Patch<T>(string endpoint, object data, Dictionary<string, string> headers = null)
        {
            Debug.Print("TitanicAPI: PATCH " + endpoint);
            return this.Send<T>(HttpMethodType.PATCH, endpoint, data, headers);
        }

        public T Delete<T>(string endpoint, Dictionary<string, string> headers = null)
        {
            Debug.Print("TitanicAPI: DELETE " + endpoint);
            return this.Send<T>(HttpMethodType.PUT, endpoint, null, headers);
        }
        
        public byte[] Download(string url)
        {
            return this._http.RequestBytes(HttpMethodType.GET, url, null, null);
        }

        public void EnsureValidAccessToken()
        {
            if (!IsLoggedIn || !IsTokenExpired)
                return;

            RefreshTokenRequest request = new(Token.RefreshToken);
            request.BlockingPerform(this);
            Debug.Print("TitanicAPI: Access token refreshed (EnsureValidAccessToken)");
        }

        public void Dispose()
        {
            this._http.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
