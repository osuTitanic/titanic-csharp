using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using Newtonsoft.Json;
using Titanic.API.Models;
using Titanic.API.Requests;

#if NETCOREAPP
using System.Net.Http;
using System.Text;
#endif

namespace Titanic.API
{
    public class TitanicAPI
    {
        private readonly string baseUrl;
#if NETCOREAPP
        private readonly HttpClient client;
#else
        private readonly WebClient client;
#endif

        public TokenModel Token;
        public bool IsLoggedIn => Token != null;
        public bool IsTokenExpired => Token == null || DateTime.Now > Token.ExpiresAt;

        public TitanicAPI(string baseUrl = "https://api.titanic.sh")
        {
            this.baseUrl = baseUrl.TrimEnd('/');
#if NETCOREAPP
            client = new HttpClient();
#else
            client = new WebClient();
            client.BaseAddress = this.baseUrl;
#endif
        }

        public T Get<T>(string endpoint, Dictionary<string, string> headers = null)
        {
            Debug.Print("TitanicAPI: GET " + endpoint);

            lock (client)
            {
                PrepareRequest(headers);
#if NETCOREAPP
                string responseJson = client.GetStringAsync(baseUrl + endpoint).GetAwaiter().GetResult();
#else
                string responseJson = client.DownloadString(endpoint);
#endif
                return JsonConvert.DeserializeObject<T>(responseJson);
            }
        }

        public T Post<T>(string endpoint, object data, Dictionary<string, string> headers = null)
        {
            Debug.Print("TitanicAPI: POST " + endpoint);

            lock (client)
            {
                // NOTE: we skip token checking for the refresh endpoint to avoid infinite loops
                PrepareRequest(headers, endpoint != "/account/refresh");
                string json = JsonConvert.SerializeObject(data);
#if NETCOREAPP
                StringContent content = new StringContent(json, Encoding.UTF8, "application/json");
                HttpResponseMessage response = client.PostAsync(baseUrl + endpoint, content).GetAwaiter().GetResult();
                string responseJson = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
#else
                string responseJson = client.UploadString(endpoint, "POST", json);
#endif
                return JsonConvert.DeserializeObject<T>(responseJson);
            }
        }

        public T Put<T>(string endpoint, object data, Dictionary<string, string> headers = null)
        {
            Debug.Print("TitanicAPI: PUT " + endpoint);

            lock (client)
            {
                PrepareRequest(headers);
                string json = JsonConvert.SerializeObject(data);
#if NETCOREAPP
                StringContent content = new StringContent(json, Encoding.UTF8, "application/json");
                HttpResponseMessage response = client.PutAsync(baseUrl + endpoint, content).GetAwaiter().GetResult();
                string responseJson = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
#else
                string responseJson = client.UploadString(endpoint, "PUT", json);
#endif
                return JsonConvert.DeserializeObject<T>(responseJson);
            }
        }

        public T Patch<T>(string endpoint, object data, Dictionary<string, string> headers = null)
        {
            Debug.Print("TitanicAPI: PATCH " + endpoint);

            lock (client)
            {
                PrepareRequest(headers);
                string json = JsonConvert.SerializeObject(data);

#if NETCOREAPP
                StringContent content = new StringContent(json, Encoding.UTF8, "application/json");
                HttpRequestMessage request = new HttpRequestMessage(new HttpMethod("PATCH"), baseUrl + endpoint);
                request.Content = content;
                HttpResponseMessage response = client.SendAsync(request).GetAwaiter().GetResult();
                string responseJson = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
#else
                // WebClient does not support Patch, so we need to build the request manually
                Uri requestUrl = new Uri(new Uri(client.BaseAddress), endpoint);
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(requestUrl);
                request.Method = "PATCH";
                request.ContentType = "application/json";

                // Copy across the headers (like Authorization) from our WebClient to the request
                foreach (string headerKey in client.Headers.AllKeys)
                {
                    if (headerKey.Equals("Authorization", StringComparison.OrdinalIgnoreCase))
                        request.Headers[HttpRequestHeader.Authorization] = client.Headers[headerKey];
                    else
                        request.Headers[headerKey] = client.Headers[headerKey];
                }

                // Write JSON body
                using (StreamWriter streamWriter = new StreamWriter(request.GetRequestStream()))
                    streamWriter.Write(json);

                using (WebResponse response = request.GetResponse())
                using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                {
                    string responseJson = reader.ReadToEnd();
#endif
                return JsonConvert.DeserializeObject<T>(responseJson);
#if !NETCOREAPP
                }
#endif
            }
        }

        public T Delete<T>(string endpoint, Dictionary<string, string> headers = null)
        {
            Debug.Print("TitanicAPI: DELETE " + endpoint);

            lock (client)
            {
                PrepareRequest(headers);
#if NETCOREAPP
                HttpResponseMessage response = client.DeleteAsync(baseUrl + endpoint).GetAwaiter().GetResult();
                string responseJson = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
#else
                string responseJson = client.UploadString(endpoint, "DELETE", "");
#endif
                return JsonConvert.DeserializeObject<T>(responseJson);
            }
        }

        public void PrepareRequest(Dictionary<string, string> headers, bool checkToken = true)
        {
            if (checkToken)
                EnsureValidAccessToken();

#if NETCOREAPP
            client.DefaultRequestHeaders.Clear();

            if (Token != null)
                client.DefaultRequestHeaders.Authorization = 
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", Token.AccessToken);

            if (headers == null)
                return;

            foreach (KeyValuePair<string, string> header in headers)
            {
                client.DefaultRequestHeaders.TryAddWithoutValidation(header.Key, header.Value);
            }
#else
            client.Headers.Clear();

            if (Token != null)
                client.Headers["Authorization"] = $"Bearer {Token.AccessToken}";
            
            if (headers == null)
                return;

            foreach (KeyValuePair<string, string> header in headers)
            {
                client.Headers[header.Key] = header.Value;
            }
#endif
        }

        public void EnsureValidAccessToken()
        {
            if (!IsLoggedIn || !IsTokenExpired)
                return;

            RefreshTokenRequest request = new RefreshTokenRequest(Token.RefreshToken);
            request.BlockingPerform(this);
            Debug.Print("TitanicAPI: Access token refreshed (EnsureValidAccessToken)");
        }
    }
}
