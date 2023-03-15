using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Milo.Services;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Xamarin.Forms;

namespace Milo.Helper
{
    public class APIService: IDisposable
    {
        private HttpClient client;
        public void Dispose()
        {
            client.Dispose();
        }
        public async Task<T> RequestAsync<T>(string path, List<KeyValuePair<string, string>> request = null, Stream fileStream = null, string picturePropertyName = "userfile", bool sessionRequired = true, string method = null, bool largeFileRequest = false) where T : class
        {
            client = new HttpClient
            {
                MaxResponseContentBufferSize = 512000
            };
            var uri = new Uri(string.Concat(AppSettings.BaseUrl, path));
            client.Timeout = TimeSpan.FromSeconds(45);
            HttpResponseMessage response;
            if (sessionRequired)
            {
            }
            try
            {
                ServicePointManager.SecurityProtocol =
                    SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                HttpContent requestContent = null;
                if (request != null)
                {
                    if (fileStream == null)
                    {
                        requestContent = new FormUrlEncodedContent(request);
                    }
                    else
                    {
                        string boundary = "----WebKitFormBoundary" + DateTime.Now.Ticks.ToString("x");
                        var content = new MultipartFormDataContent(boundary);
                        foreach (KeyValuePair<string, string> pair in request)
                        {
                            content.Add(new StringContent(pair.Value), pair.Key);
                        }
                        if (fileStream != null)
                        {
                            content.Add(new StreamContent(fileStream), picturePropertyName, "upload.jpg");
                        }
                        content.Headers.TryAddWithoutValidation("Content-Type", "multipart/form-data; boundary=" + boundary);
                        requestContent = content;
                    }
                    response = !string.IsNullOrWhiteSpace(method) && method.ToUpper().Equals("PUT")
                        ? await client.PutAsync(uri, requestContent)
                        : await client.PostAsync(uri, requestContent);
                }
                else
                {
                    if (!string.IsNullOrWhiteSpace(method) && method.ToUpper().Equals("DELETE"))
                    {
                        response = await client.DeleteAsync(uri);
                    }
                    else
                    {
                        if (largeFileRequest)
                        {
                            response = await client.GetAsync(uri, HttpCompletionOption.ResponseHeadersRead);
                        }
                        else
                        {
                            response = await client.GetAsync(uri);
                        }
                    }
                }
            }
            catch (TaskCanceledException)
            {
                client?.Dispose();
#if DEBUG
                Console.WriteLine("TimeoutException");
#endif
                return null;
            }
            catch (Exception)
            {
                client?.Dispose();
                return null;
            }
            if (response.IsSuccessStatusCode)
            {
                try
                {
                    var responseJson = await response.Content.ReadAsStringAsync();

                    JObject json = JObject.Parse(responseJson);
                    var status = json.GetValue("status")?.ToString();
                    var msg = json.GetValue("msg")?.ToString();
                    if (status == "0" && msg != null && msg.StartsWith("Unauthorized"))
                    {
                        var miloService = DependencyService.Get<IMiloService>();
                        string email = await EncryptedStorageHelper.GetEmailAsync();
                        string password = await EncryptedStorageHelper.GetPasswordAsync();
                        List<KeyValuePair<string, string>> loginRequest = new List<KeyValuePair<string, string>>() {
                    new KeyValuePair<string, string>("email_id", email),
                    new KeyValuePair<string, string>("password", password)
                    };
                        var loginResponse = await miloService.Login(loginRequest);
                        await EncryptedStorageHelper.SaveTokenAsync(loginResponse.token_code);

                        int removalStatus = request.RemoveAll(x => x.Key == "token_code");

                        if (removalStatus == 1)
                        {
                            request.Add(new KeyValuePair<string, string>("token_code", loginResponse.token_code));
                        }
                        await Task.Delay(1000);
                        // Retry the request with new session ID
                        return await RequestAsync<T>(path, request, fileStream, picturePropertyName, sessionRequired, method, largeFileRequest);
                    }

#if DEBUG
                    Console.WriteLine("Response::" + path + "::" + response.StatusCode + "::" + responseJson);
#endif
                    var serializedResponse = JsonConvert.DeserializeObject<T>(responseJson);

                    return serializedResponse;
                }catch(Exception)
                {
                    return null;
                }
               

        }
            else
            {
                if (sessionRequired && response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    var miloService = DependencyService.Get<IMiloService>();
                    string email = await EncryptedStorageHelper.GetEmailAsync();
                    string password = await EncryptedStorageHelper.GetPasswordAsync();
                    List<KeyValuePair<string, string>> loginRequest = new List<KeyValuePair<string, string>>() {
                    new KeyValuePair<string, string>("email_id", email),
                    new KeyValuePair<string, string>("password", password)
                    };
                    var loginResponse = await miloService.Login(loginRequest);
                    await EncryptedStorageHelper.SaveTokenAsync(loginResponse.token_code);

                    int removalStatus = request.RemoveAll(x => x.Key == "token_code");

                    if (removalStatus == 1)
                    {
                        request.Add(new KeyValuePair<string, string>("token_code", loginResponse.token_code));
                    }
                    await Task.Delay(1000);
                    // Retry the request with new session ID
                    return await RequestAsync<T>(path, request,fileStream, picturePropertyName, sessionRequired, method, largeFileRequest);
                }
                return null;
            }
        }
    }
}
