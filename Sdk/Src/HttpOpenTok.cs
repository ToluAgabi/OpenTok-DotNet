using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OpenTokSDK.Exceptions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Xml;

namespace OpenTokSDK
{
    public class HttpOpenTok
    {
        private const string DEFAULT_USER_AGENT = "OpenTok-DotNet-SDK/2.0.0";
        private static int apiKey;
        private static string apiSecret;
        private static string server;

        public static void initialize(int apiKey, string apiSecret, string server)
        {
            HttpOpenTok.apiKey = apiKey;
            HttpOpenTok.apiSecret = apiSecret;
            HttpOpenTok.server = server;
        }

        public static string Get(string url)
        {
            return Get(url, new Dictionary<string, string>());
        }

        public static string Get(string url, Dictionary<string, string> headers)
        {
            headers.Add("Method", "GET");         
            return DoRequest(url, headers, null);
        }

        public static string Post(string url, Dictionary<string, string> headers, Dictionary<string, object> data)
        {
            headers.Add("Method", "POST");
            return DoRequest(url, headers, data);
        }

        public static string Delete(string url, Dictionary<string, string> headers, Dictionary<string, object> data)
        {
            headers.Add("Method", "DELETE");
            return DoRequest(url, headers, data);
        }
      
        public static string DoRequest(string url, Dictionary<string, string> specificHeaders, 
                                        Dictionary<string, object> bodyData)
        {
            string data = GetRequestPostData(bodyData, specificHeaders);
            var headers = GetRequestHeaders(specificHeaders);
            HttpWebRequest request = CreateRequest(url, headers, data);
            HttpWebResponse response;

            try
            {
                if (data != "")
                {
                    sendData(request, data);
                }
                response = (HttpWebResponse)request.GetResponse();     
            }
            catch(WebException)
            {
                throw new OpenTokRequestException("Unexpected response from OpenTok", 404);
            }

            if (response.StatusCode == HttpStatusCode.OK)
            {
                StreamReader stream = new StreamReader(response.GetResponseStream());
                return stream.ReadToEnd();              
            }  
            else if(response.StatusCode == HttpStatusCode.NoContent)
            {
                return "";
            }
            else
            {
                throw new OpenTokRequestException("Response returned with error status code", 404);
            }
        }

        public static XmlDocument ReadXmlResponse(string xml)
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(xml);
            return xmlDoc;
        }

        //ggb camelcase
        private static void sendData(HttpWebRequest request, object data)
        {
            using (StreamWriter stream = new StreamWriter(request.GetRequestStream()))
            {
                stream.Write(data);
            }
        }

        private static HttpWebRequest CreateRequest(string url, Dictionary<string, string> headers, string data)
        {
            Uri uri = new Uri(string.Format("{0}/{1}", server, url));
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);            
            request.ContentLength = data.Length;
            request.UserAgent = DEFAULT_USER_AGENT;
            
            if (headers.ContainsKey("User-Agent"))
            {
                request.UserAgent = headers["User-Agent"];
                headers.Remove("User-Agent");
            }
            if (headers.ContainsKey("Content-type"))
            {
                request.ContentType = headers["Content-type"];
                request.Expect = headers["Content-type"];
                headers.Remove("Content-type");                
            }
            if (headers.ContainsKey("Method"))
            {
                request.Method = headers["Method"];
                headers.Remove("Method");
            }
            
            foreach (KeyValuePair<string, string> entry in headers)
            {
                request.Headers.Add(entry.Key, entry.Value);
            }

            return request;
        }
        private static Dictionary<string, string> GetRequestHeaders(Dictionary<string, string> headers)
        {
            var requestHeaders = GetCommonHeaders();
            requestHeaders = requestHeaders.Concat(headers).GroupBy(d => d.Key)
                                .ToDictionary(d => d.Key, d => d.First().Value);
            return requestHeaders;
        }

        private static string GetRequestPostData(Dictionary<string, object> data, Dictionary<string, string> headers)
        {
            if (data != null && headers.ContainsKey("Content-type"))
            {
                if (headers["Content-type"] == "application/json")
                {
                    return JsonConvert.SerializeObject(data);
                }
                else if (headers["Content-type"] == "application/x-www-form-urlencoded")
                {
                    return ProcessParameters(data);
                }
            }
            else if (data != null || headers.ContainsKey("Content-type"))
            {
                throw new OpenTokInvalidArgumentException("If Content-type is set in the headers data in the body is expected");
            }            
            return "";
        }

        private static string ProcessParameters(Dictionary<string, object> parameters)
        {
            string data = string.Empty;

            foreach (KeyValuePair<string, object> pair in parameters)
            {
                data += pair.Key + "=" + HttpUtility.UrlEncode(pair.Value.ToString()) + "&";
            }
            return data.Substring(0, data.Length - 1);            
        }
        private static Dictionary<string, string> GetCommonHeaders()
        {
            return new Dictionary<string, string> {
                { "X-TB-PARTNER-AUTH", String.Format("{0}:{1}", apiKey, apiSecret) },            
                { "X-TB-VERSION", "1" },
                { "User-Agent", DEFAULT_USER_AGENT }
            };
        }
    }
}
