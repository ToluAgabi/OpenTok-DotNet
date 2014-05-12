using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Configuration;
using OpenTokSDK.Exceptions;
using Newtonsoft.Json;
using OpenTokSDK.Util;

namespace OpenTokSDK
{
    public class OpenTok
    {
        public int ApiKey { get; private set; }
        public string ApiSecret { get; private set; }
        private string OpenTokServer { get; set; }
        private HttpClient httpClient { get; set; }

        public OpenTok(int apiKey, string apiSecret)
        { 
            this.ApiKey = apiKey;
            this.ApiSecret = apiSecret;
            this.OpenTokServer = "https://api.opentok.com";
            httpClient = new HttpClient(apiKey, apiSecret, this.OpenTokServer);
        }

        public OpenTok(int apiKey, string apiSecret, string apiUrl)
        {
            this.ApiKey = apiKey;
            this.ApiSecret = apiSecret;
            this.OpenTokServer = apiUrl;
            httpClient = new HttpClient(apiKey, apiSecret, this.OpenTokServer);
        }

        public Session CreateSession(string location = "", bool p2p = false)
        {
            
            if (!OpenTokUtils.TestIpAddress(location))
            {
                throw new OpenTokArgumentException(string.Format("Location {0} is not a valid IP address", location));
            }
            string preference = (p2p)? "enabled": "disabled";

            var headers = new Dictionary<string, string>{{"Content-type", "application/x-www-form-urlencoded"}};
            var data = new Dictionary<string, object>
            {
                {"location", location},
                {"p2p.preference", preference}
            };

            var response = httpClient.Post("session/create", headers, data);
            var xmlDoc = httpClient.ReadXmlResponse(response);
            
            if (xmlDoc.GetElementsByTagName("session_id").Count == 0)
            {
                throw new OpenTokWebException("Session could not be provided. Are ApiKey and ApiSecret correctly set?");
            }
            var sessionId = xmlDoc.GetElementsByTagName("session_id")[0].ChildNodes[0].Value;
            var apiKey = Convert.ToInt32(xmlDoc.GetElementsByTagName("partner_id")[0].ChildNodes[0].Value);
            return new Session(sessionId, apiKey, ApiSecret, location, p2p);            
        }

        public string GenerateToken(string sessionId, Role role = Role.PUBLISHER, double expireTime = 0, string data = null)
        {
            if (String.IsNullOrEmpty(sessionId))
            {
                throw new OpenTokArgumentException("Session id cannot be empty or null");
            }
            Session session = new Session(sessionId, this.ApiKey, this.ApiSecret);
            return session.GenerateToken(role, expireTime, data);
        }

        public Archive StartArchive(string sessionId, string name = "")
        {
            if (sessionId == null || sessionId == "")
            {
                throw new OpenTokArgumentException("Session not valid");
            }
            string url = string.Format("v2/partner/{0}/archive", this.ApiKey);
            var headers = new Dictionary<string, string> { { "Content-type", "application/json" } };
            var data = new Dictionary<string, object>() { { "sessionId", sessionId }, { "name", name } };
            string response = httpClient.Post(url, headers, data);
            return JsonConvert.DeserializeObject<Archive>(response);   
        }

        public Archive StopArchive(string archiveId)
        {
            string url = string.Format("v2/partner/{0}/archive/{1}/stop", this.ApiKey, archiveId);
            var headers = new Dictionary<string, string> { { "Content-type", "application/json" } };

            string response = httpClient.Post(url, headers, new Dictionary<string, object>());
            return JsonConvert.DeserializeObject<Archive>(response);
        }

        public ArchiveList ListArchives()
        {
            return ListArchives(0, 0);
        }

        public ArchiveList ListArchives(int offset, int count)
        {
            if (count < 0)
            {
                throw new OpenTokArgumentException("count cannot be smaller than 1");
            }
            string url = string.Format("v2/partner/{0}/archive?offset={1}", this.ApiKey, offset);
            if (count > 0)
            {
                url = string.Format("{0}&count={1}", url, count);
            }
            string response = httpClient.Get(url);
            return JsonConvert.DeserializeObject<ArchiveList>(response);
        }

        public Archive GetArchive(string archiveId)
        {
            string url = string.Format("v2/partner/{0}/archive/{1}", this.ApiKey, archiveId);
            var headers = new Dictionary<string, string>{ {"Content-type", "application/json"} };
            string response = httpClient.Get(url);           ;
            return JsonConvert.DeserializeObject<Archive>(response);
        }

        public void DeleteArchive(string archiveId)
        {
            string url = string.Format("v2/partner/{0}/archive/{1}", this.ApiKey, archiveId);
            var headers = new Dictionary<string, string> { { "Content-type", "application/json" } };
            httpClient.Delete(url, headers, new Dictionary<string, object>());
        }
    }
}
