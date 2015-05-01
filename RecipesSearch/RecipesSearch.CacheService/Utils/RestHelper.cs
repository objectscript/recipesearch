using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using RecipesSearch.BusinessServices.Logging;

namespace RecipesSearch.CacheService.Utils
{
    public static class RestHelper
    {
        private readonly static string _username;
        private readonly static string _password;

        static RestHelper()
        {
            _username = ConfigurationManager.AppSettings["Username"];
            _password = ConfigurationManager.AppSettings["Password"];
        }

        public static T MakeRequest<T>(string endpoint, HttpVerb method, Dictionary<string, string> parameters, object payload)
        {
            try
            {
                var request = (HttpWebRequest)WebRequest.Create(endpoint + BuildParameters(parameters));

                request.Method = method.ToString();
                request.ContentLength = 0;
                request.ContentType = "application/json";
                request.Credentials = new NetworkCredential(_username, _password);

                if (payload != null && method == HttpVerb.POST)
                {
                    var postData = JsonConvert.SerializeObject(payload);
                    var encoding = new UTF8Encoding();
                    var bytes = encoding.GetBytes(postData);
                    request.ContentLength = bytes.Length;

                    using (var writeStream = request.GetRequestStream())
                    {
                        writeStream.Write(bytes, 0, bytes.Length);
                    }
                }

                using (var response = (HttpWebResponse) request.GetResponse())
                {
                    var responseValue = string.Empty;

                    if (response.StatusCode != HttpStatusCode.OK)
                    {
                        Logger.LogError(String.Format("Request failed. Received HTTP {0}", response.StatusCode), null);
                    }

                    using (var responseStream = response.GetResponseStream())
                    {
                        if (responseStream != null)
                            using (var reader = new StreamReader(responseStream))
                            {
                                responseValue = reader.ReadToEnd();
                            }
                    }

                    return JsonConvert.DeserializeObject<T>(responseValue);
                }
            }
            catch (Exception e)
            {
                Logger.LogError(String.Format("Request failed."), e);
                return default(T);
            }            
        }

        private static string BuildParameters(Dictionary<string, string> parameters)
        {
            if (parameters == null || parameters.Count == 0)
            {
                return String.Empty;
            }

            return "?" + String.Join("&", parameters.Select(pair => String.Format("{0}={1}", pair.Key, pair.Value)));
        }

        public enum HttpVerb
        {
            GET,
            POST,
            PUT,
            DELETE
        }
    }
}
