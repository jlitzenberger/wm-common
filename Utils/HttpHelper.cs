using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Net.Mime;
using System.Threading.Tasks;
using System.Web;

namespace WM.Common.Utils
{
    public static class HttpHelper
    {
        public static HttpResponseMessage LoadHttpResponseWithFile(string uncPath)
        {
            FileInfo fi = new FileInfo(uncPath);
            string mimeType = MimeMapping.GetMimeMapping(uncPath);

            HttpResponseMessage result = new HttpResponseMessage(HttpStatusCode.OK);
            var stream = new FileStream(uncPath, FileMode.Open);
            result.Content = new StreamContent(stream);
            result.Content.Headers.ContentType = new MediaTypeHeaderValue(mimeType);
            result.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment");
            result.Content.Headers.ContentDisposition.FileName = fi.Name;
            result.Content.Headers.ContentDisposition.Name = fi.Name;
            result.Content.Headers.ContentDisposition.Size = stream.Length;

            return result;
        }
        public static async Task<IEnumerable<string>> UploadFilesMultipartContent(HttpContent content, string uploadPath)
        {
            if (!Directory.Exists(uploadPath))
            {
                Directory.CreateDirectory(uploadPath);
            }

            // Set the upload path
            UploadMultipartFormProvider multipartFormDataStreamProvider = new UploadMultipartFormProvider(uploadPath);

            // Read the MIME multipart asynchronously and upload the file
            UploadMultipartFormProvider provider = await content.ReadAsMultipartAsync(multipartFormDataStreamProvider);

            //// Get the file name
            //string localFileName = provider
            //                      .FileData.Select(multiPartData => multiPartData.LocalFileName)
            //                      .FirstOrDefault();

            // Get the file names
            IEnumerable<string> list = provider
                                        .FileData.Select(multiPartData => multiPartData.LocalFileName);

            return list;
        }
        public static async Task<List<ContentDisposition>> GetMultipartContentDisposition(HttpRequestMessage message)
        {
            if (message.Content.IsMimeMultipartContent())
            {
                //Load into a stream so that it can be read again.
                Stream stream = await message.Content.ReadAsStreamAsync();

                //Get the mutipart content
                MultipartMemoryStreamProvider provider = await message.Content.ReadAsMultipartAsync(new MultipartMemoryStreamProvider());

                //Add it to the list
                List<ContentDisposition> list = new List<ContentDisposition>();
                foreach (var item in provider.Contents)
                {
                    list.Add(new ContentDisposition(item.Headers.ContentDisposition.ToString()));
                }

                //Reset the stream to 0 so that it can be read again
                stream.Position = 0;

                return list;
            }

            return null;
        }

        public static HttpResponseMessage GetServiceCall(string requestUri, string credentials = null, Dictionary<string, string> requestHeaders = null)
        {
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                if (!string.IsNullOrEmpty(credentials))
                {
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", credentials);
                }

                if (requestHeaders != null)
                {
                    foreach (var header in requestHeaders)
                    {
                        client.DefaultRequestHeaders.Add(header.Key, header.Value);
                    }
                }

                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

                HttpResponseMessage response = client.GetAsync(requestUri).Result;

                return response;
            }
        }
        public static async Task<HttpResponseMessage> GetServiceCallAsync(string requestUri, string credentials = null, Dictionary<string, string> requestHeaders = null)
        {
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                if (!string.IsNullOrEmpty(credentials))
                {
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", credentials);
                }

                if (requestHeaders != null)
                {
                    foreach (var header in requestHeaders)
                    {
                        client.DefaultRequestHeaders.Add(header.Key, header.Value);
                    }
                }

                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

                return await client.GetAsync(requestUri);
            }
        }

        public static HttpResponseMessage DeleteServiceCall(string requestUri, string credentials = null, Dictionary<string, string> headers = null)
        {
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                if (!string.IsNullOrEmpty(credentials))
                {
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", credentials);
                }

                if (headers != null)
                {
                    foreach (var header in headers)
                    {
                        client.DefaultRequestHeaders.Add(header.Key, header.Value);
                    }
                }

                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

                HttpResponseMessage response = client.DeleteAsync(requestUri).Result;

                return response;
            }
        }
        public static async Task<HttpResponseMessage> DeleteServiceCallAsync(string requestUri, string credentials = null, Dictionary<string, string> headers = null)
        {
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                if (!string.IsNullOrEmpty(credentials))
                {
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", credentials);
                }

                if (headers != null)
                {
                    foreach (var header in headers)
                    {
                        client.DefaultRequestHeaders.Add(header.Key, header.Value);
                    }
                }

                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

                return await client.DeleteAsync(requestUri);
            }
        }

        public static HttpResponseMessage PutServiceCall(string requestUri, object obj, string credentials = null, Dictionary<string, string> headers = null)
        {
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                if (!string.IsNullOrEmpty(credentials))
                {
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", credentials);
                }

                if (headers != null)
                {
                    foreach (var header in headers)
                    {
                        client.DefaultRequestHeaders.Add(header.Key, header.Value);
                    }
                }

                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

                HttpResponseMessage response = client.PutAsJsonAsync(requestUri, obj).Result;

                return response;
            }
        }
        public static async Task<HttpResponseMessage> PutServiceCallAsync(string requestUri, object obj, string credentials = null, Dictionary<string, string> headers = null)
        {
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                if (!string.IsNullOrEmpty(credentials))
                {
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", credentials);
                }

                if (headers != null)
                {
                    foreach (var header in headers)
                    {
                        client.DefaultRequestHeaders.Add(header.Key, header.Value);
                    }
                }

                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

                return await client.PutAsJsonAsync(requestUri, obj);
            }
        }

        public static HttpResponseMessage PostServiceCall(string requestUri, object obj, string credentials = null, Dictionary<string, string> headers = null)
        {
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                if (!string.IsNullOrEmpty(credentials))
                {
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", credentials);
                }

                if (headers != null)
                {
                    foreach (var header in headers)
                    {
                        client.DefaultRequestHeaders.Add(header.Key, header.Value);
                    }
                }

                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

                HttpResponseMessage response = client.PostAsJsonAsync(requestUri, obj).Result;
                return response;
            }
        }
        public static HttpResponseMessage PostServiceCall(string requestUri, Newtonsoft.Json.Linq.JToken payload, string credentials = null, Dictionary<string, string> requestHeaders = null, Dictionary<string, string> contentHeaders = null)
        {
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                if (!string.IsNullOrEmpty(credentials))
                {
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", credentials);
                }

                if (requestHeaders != null)
                {
                    foreach (var header in requestHeaders)
                    {
                        client.DefaultRequestHeaders.Add(header.Key, header.Value);
                    }
                }

                System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12;

                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, requestUri)
                {
                    Content = new StringContent(payload.ToString(), System.Text.Encoding.UTF8)
                };

                if (contentHeaders != null)
                {
                    foreach (var header in contentHeaders)
                    {
                        request.Content.Headers.Add(header.Key, header.Value);
                    }
                }

                request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                HttpResponseMessage response = client.SendAsync(request).Result;
                return response;
            }
        }
        public static HttpResponseMessage PostServiceCall(string requestUri, string payload, string credentials = null, Dictionary<string, string> requestHeaders = null, Dictionary<string, string> contentHeaders = null)
        {
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                if (!string.IsNullOrEmpty(credentials))
                {
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", credentials);
                }

                if (requestHeaders != null)
                {
                    foreach (var header in requestHeaders)
                    {
                        client.DefaultRequestHeaders.Add(header.Key, header.Value);
                    }
                }

                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, requestUri)
                {
                    Content = new StringContent(payload)
                };

                if (contentHeaders != null)
                {
                    foreach (var header in contentHeaders)
                    {
                        request.Content.Headers.Add(header.Key, header.Value);
                    }
                }

                request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                return client.SendAsync(request).Result;
            }
        }
        public static async Task<HttpResponseMessage> PostServiceCallAsync(string requestUri, object obj, string credentials = null, Dictionary<string, string> headers = null)
        {
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                if (!string.IsNullOrEmpty(credentials))
                {
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", credentials);
                }

                if (headers != null)
                {
                    foreach (var header in headers)
                    {
                        client.DefaultRequestHeaders.Add(header.Key, header.Value);
                    }
                }

                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

                return await client.PostAsJsonAsync(requestUri, obj);
            }
        }
        public static async Task<HttpResponseMessage> PostServiceCallAsync(string requestUri, Newtonsoft.Json.Linq.JToken payload, string credentials = null, Dictionary<string, string> requestHeaders = null, Dictionary<string, string> contentHeaders = null)
        {
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                if (!string.IsNullOrEmpty(credentials))
                {
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", credentials);
                }

                if (requestHeaders != null)
                {
                    foreach (var header in requestHeaders)
                    {
                        client.DefaultRequestHeaders.Add(header.Key, header.Value);
                    }
                }

                System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12;

                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, requestUri)
                {
                    Content = new StringContent(payload.ToString(), System.Text.Encoding.UTF8)
                };

                if (contentHeaders != null)
                {
                    foreach (var header in contentHeaders)
                    {
                        request.Content.Headers.Add(header.Key, header.Value);
                    }
                }

                request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                return await client.SendAsync(request);
            }
        }
        public static async Task<HttpResponseMessage> PostServiceCallAsync(string requestUri, string payload, string credentials = null, Dictionary<string, string> requestHeaders = null, Dictionary<string, string> contentHeaders = null)
        {
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                if (!string.IsNullOrEmpty(credentials))
                {
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", credentials);
                }

                if (requestHeaders != null)
                {
                    foreach (var header in requestHeaders)
                    {
                        client.DefaultRequestHeaders.Add(header.Key, header.Value);
                    }
                }

                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, requestUri)
                {
                    Content = new StringContent(payload)
                };

                if (contentHeaders != null)
                {
                    foreach (var header in contentHeaders)
                    {
                        request.Content.Headers.Add(header.Key, header.Value);
                    }
                }

                request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                return await client.SendAsync(request);
            }
        }

        public static HttpResponseMessage PatchServiceCall(string requestUri, object obj, string credentials = null, Dictionary<string, string> headers = null)
        {
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                if (!string.IsNullOrEmpty(credentials))
                {
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", credentials);
                }

                if (headers != null)
                {
                    foreach (var header in headers)
                    {
                        client.DefaultRequestHeaders.Add(header.Key, header.Value);
                    }
                }

                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

                HttpResponseMessage response = client.PatchAsJsonAsync(requestUri, obj).Result;
                return response;
            }
        }
        private static Task<HttpResponseMessage> PatchAsJsonAsync<T>(this HttpClient client, string requestUri, T value)
        {
            //Ensure.Argument.NotNull(client, "client");
            //Ensure.Argument.NotNullOrEmpty(requestUri, "requestUri");
            //Ensure.Argument.NotNull(value, "value");

            var content = new ObjectContent<T>(value, new JsonMediaTypeFormatter());
            var request = new HttpRequestMessage(new HttpMethod("PATCH"), requestUri) { Content = content };

            return client.SendAsync(request);
        }
    }
}
