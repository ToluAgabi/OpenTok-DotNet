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

namespace OpenTokSDK
{
    public class OpenTok
    {        
        public int ApiKey { get; set; }
        public string ApiSecret { get; set; }
        private string OpenTokServer { get; set; }

        public OpenTok()
        {
           
        }

        public OpenTok(int apiKey, string apiSecret)
        {
            this.ApiKey = apiKey;
            this.ApiSecret = apiSecret;
            this.OpenTokServer = "https://api.opentok.com";
            HttpOpenTok.initialize(apiKey, apiSecret, this.OpenTokServer);
        }

        public OpenTok(int apiKey, string apiSecret, string apiUrl)
        {
            this.ApiKey = apiKey;
            this.ApiSecret = apiSecret;
            this.OpenTokServer = apiUrl;
            HttpOpenTok.initialize(apiKey, apiSecret, this.OpenTokServer);
        }

        public Session CreateSession(string location = "", bool p2p = false)
        {
            string url = "session/create";
            string preference = (p2p)? "enabled": "disabled";

            var headers = new Dictionary<string, string>{{"Content-type", "application/x-www-form-urlencoded"}};
            var data = new Dictionary<string, object>
            {
                {"location", location},
                {"p2p.preference", preference}
            };

            var response = HttpOpenTok.Post(url, headers, data);
            var xmlDoc = HttpOpenTok.ReadXmlResponse(response);
            var sessionId = xmlDoc.GetElementsByTagName("session_id")[0].ChildNodes[0].Value;
            return new Session(sessionId, ApiKey, ApiSecret, location, p2p);            
        }

        public string GenerateToken(string sessionId)
        {
            return GenerateToken(sessionId, new TokenProperties());
        }

        public string GenerateToken(string sessionId, TokenProperties.RoleProperty role)
        {
            TokenProperties properties = new TokenProperties(role);
            Session session = new Session(sessionId, this.ApiKey, this.ApiSecret);
            return session.GenerateToken(properties);
        }

        public string GenerateToken(string sessionId, TokenProperties.RoleProperty role, DateTime expireTime, string connectionData)
        {
            TokenProperties properties = new TokenProperties(role, expireTime, connectionData);
            Session session = new Session(sessionId, this.ApiKey, this.ApiSecret);
            return session.GenerateToken(properties);
        }

        public string GenerateToken(string sessionId, TokenProperties properties)
        {
            Session session = new Session(sessionId, this.ApiKey, this.ApiSecret);
            return session.GenerateToken(properties);
        }

        public Archive StartArchive(string sessionId, string name)
        {
            if (sessionId == null || sessionId == "")
            {
                throw new OpenTokInvalidArgumentException("Session not valid");
            }
            string url = string.Format("v2/partner/{0}/archive", this.ApiKey);
            var headers = new Dictionary<string, string> { { "Content-type", "application/json" } };
            var data = new Dictionary<string, object>() { { "sessionId", sessionId }, { "name", name } };
            string response = HttpOpenTok.Post(url, headers, data);
            return JsonConvert.DeserializeObject<Archive>(response);   
        }

        public Archive StopArchive(string archiveId)
        {
            string url = string.Format("v2/partner/{0}/archive/{1}", this.ApiKey, archiveId);
            var headers = new Dictionary<string, string> { { "Content-type", "application/json" } };
            var data = new Dictionary<string, object> { {"action", "stop"} };

            string response = HttpOpenTok.Post(url, headers, data);
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
                throw new OpenTokInvalidArgumentException("count cannot be smaller than 1");
            }
            string url = string.Format("v2/partner/{0}/archive?offset={1}", this.ApiKey, offset);
            if (count > 0)
            {
                url = string.Format("{0}&count={1}", url, count);
            }
            string response = HttpOpenTok.Get(url);
            return JsonConvert.DeserializeObject<ArchiveList>(response);
        }

        public Archive GetArchive(string archiveId)
        {
            string url = string.Format("v2/partner/{0}/archive/{1}", this.ApiKey, archiveId);
            var headers = new Dictionary<string, string>{ {"Content-type", "application/json"} };
            string response = HttpOpenTok.Get(url);
            Archive archive = new Archive(new OpenTok(ApiKey, ApiSecret));
            archive.Copy(JsonConvert.DeserializeObject<Archive>(response));
            return archive;          
        }

        public void DeleteArchive(string archiveId)
        {
            string url = string.Format("v2/partner/{0}/archive/{1}", this.ApiKey, archiveId);
            var headers = new Dictionary<string, string> { { "Content-type", "application/json" } };
            HttpOpenTok.Delete(url, headers, new Dictionary<string, object>());
        }
        
    }
}
