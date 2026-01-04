using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using Newtonsoft.Json;
using Titanic.API.Models;
using Titanic.API.Requests;

namespace Titanic.API
{
    public class TitanicAPI
    {
        private readonly WebClient client;
        public TokenModel Token;

        public bool IsLoggedIn => Token != null;
        public bool IsTokenExpired => Token == null || DateTime.Now > Token.ExpiresAt;

        public TitanicAPI(string baseUrl = "https://api.titanic.sh")
        {
            client = new WebClient();
            client.BaseAddress = baseUrl;
        }

        public T Get<T>(string endpoint, Dictionary<string, string> headers = null)
        {
            Debug.Print("TitanicAPI: GET " + endpoint);

            lock (client)
            {
                PrepareRequest(headers);
                string responseJson = client.DownloadString(endpoint);
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
                string responseJson = client.UploadString(endpoint, "POST", json);
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
                string responseJson = client.UploadString(endpoint, "PUT", json);
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
                    return JsonConvert.DeserializeObject<T>(responseJson);
                }
            }
        }

        public T Delete<T>(string endpoint, Dictionary<string, string> headers = null)
        {
            Debug.Print("TitanicAPI: DELETE " + endpoint);

            lock (client)
            {
                PrepareRequest(headers);
                string responseJson = client.UploadString(endpoint, "DELETE", "");
                return JsonConvert.DeserializeObject<T>(responseJson);
            }
        }

        public void PrepareRequest(Dictionary<string, string> headers, bool checkToken = true)
        {
            if (checkToken)
                EnsureValidAccessToken();

            client.Headers.Clear();

            if (Token != null)
                client.Headers["Authorization"] = $"Bearer {Token.AccessToken}";
            
            if (headers == null)
                return;

            foreach (KeyValuePair<string, string> header in headers)
            {
                client.Headers[header.Key] = header.Value;
            }
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
