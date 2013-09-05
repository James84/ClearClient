using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace ClearClient
{
    public class GenericClient : IDisposable
    {
        private HttpClient client;

        /// <summary>
        /// Gets the HttpResponseMessage for the last http request made.
        /// If no request has been made an empty HttpResponseMessage object will be returned
        /// </summary>
        public HttpResponseMessage ResponseMessage { get; private set; }

        public GenericClient(string baseAddress)
        {
            Init(baseAddress);
            ResponseMessage = new HttpResponseMessage();
        }

        /// <summary>
        /// set up generic client with basic authentication
        /// </summary>
        /// <param name="baseAddress"></param>
        /// <param name="username"></param>
        /// <param name="password"></param>
        public GenericClient(string baseAddress, string username, string password)
        {
            Init(baseAddress);
            ResponseMessage = new HttpResponseMessage();
            SetBasicAuth(username, password);
        }

        /// <summary>
        /// make asynchronous http request and return deserialized object
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="url"></param>
        /// <returns></returns>
        public T Get<T>(string url)
        {
            try
            {
                var cleanUrl = CleanUrl(url);
                ResponseMessage = client.GetAsync(cleanUrl).Result;
                var result = JsonConvert.DeserializeObject<T>(ResponseMessage.Content.ReadAsStringAsync().Result);
                return result;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(ex.Message);
            }
        }

        /// <summary>
        /// make asynchronous http request and return deserialized object
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="url"></param>
        /// <returns></returns>
        public async Task<T> GetAsync<T>(string url)
        {
            try
            {
                var cleanUrl = CleanUrl(url);
                ResponseMessage = await client.GetAsync(cleanUrl);
                var result = JsonConvert.DeserializeObject<T>(await ResponseMessage.Content.ReadAsStringAsync());
                return result;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(ex.Message);
            }
        }

        /// <summary>
        /// make http request and return json
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public string GetJson(string url)
        {
            client.DefaultRequestHeaders.Add("Accept", "application/json");
            var cleanUrl = CleanUrl(url);
            ResponseMessage = client.GetAsync(cleanUrl).Result;
            return ResponseMessage.Content.ReadAsStringAsync().Result;
        }

        /// <summary>
        /// make asynchronous http request and return json
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public async Task<string> GetJsonAsync(string url)
        {
            client.DefaultRequestHeaders.Add("Accept", "application/json");
            var cleanUrl = CleanUrl(url);
            ResponseMessage = await client.GetAsync(cleanUrl);
            return await ResponseMessage.Content.ReadAsStringAsync();
        }

        /// <summary>
        /// make http request and return xml
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public string GetXML(string url)
        {
            client.DefaultRequestHeaders.Add("Accept", "application/xml");
            var cleanUrl = CleanUrl(url);
            ResponseMessage = client.GetAsync(cleanUrl).Result;
            return ResponseMessage.Content.ReadAsStringAsync().Result;
        }

        /// <summary>
        /// make asynchronous http request and return xml
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public async Task<string> GetXMLAsync(string url)
        {
            client.DefaultRequestHeaders.Add("Accept", "application/xml");
            var cleanUrl = CleanUrl(url);
            ResponseMessage = await client.GetAsync(cleanUrl);
            return await ResponseMessage.Content.ReadAsStringAsync();
        }

        /// <summary>
        /// make http post request
        /// </summary>
        /// <param name="url"></param>
        /// <param name="model"></param>
        public void Post(string url, object model)
        {
            var content = model.Serialize();
            ResponseMessage = client.PostAsync(url, content).Result;
        }

        /// <summary>
        /// make asynchronous http post request
        /// </summary>
        /// <param name="url"></param>
        /// <param name="model"></param>
        public async void PostAsync(string url, object model)
        {
            var content = model.Serialize();
            ResponseMessage = await client.PostAsync(url, content);
        }

        public void Put(string url, object model)
        {
            var content = model.Serialize();
            ResponseMessage = client.PutAsync(url, content).Result;
        }

        public async void PutAsync(string url, object model)
        {
            var content = model.Serialize();
            ResponseMessage = await client.PutAsync(url, content);
        }

        public void Delete(string url)
        {
            ResponseMessage = client.DeleteAsync(url).Result;
        }

        public async void DeleteAsync(string url)
        {
            ResponseMessage = await client.DeleteAsync(url);
        }

        public void Dispose()
        {
            client.Dispose();
        }

        #region private methods

        private void Init(string baseAddress)
        {
            if (string.IsNullOrEmpty(baseAddress))
                throw new ArgumentException("No base url passed to constructor.");

            //*********************clean up base address before assigning to base address*********************//
            baseAddress = baseAddress.StartsWith("http://") || baseAddress.StartsWith("https://")
                              ? baseAddress
                              : "http://" + baseAddress;

            if ((baseAddress.Substring(baseAddress.Length - 1, 1)) != "/")
                baseAddress += "/";

            client = new HttpClient
            {
                BaseAddress = new Uri(baseAddress)
            };
            //************************************************************************************************//
        }

        //set up basic authentication
        private void SetBasicAuth(string username, string password)
        {
            var authenticationBytes = Encoding.UTF8.GetBytes(string.Format("{0}:{1}", username, password));
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic",
                                                                                       Convert.ToBase64String(
                                                                                           authenticationBytes));
        }

        private static string CleanUrl(string url)
        {
            return url.StartsWith("/") ? url.Substring(1, url.Length - 1) : url;
        }
        
        #endregion
    }
}
