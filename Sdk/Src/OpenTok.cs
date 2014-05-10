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

        public string CreateSession()
        {
            return CreateSession("");
        }

        
        public string CreateSession(string location)
        {
            return CreateSession(location, SessionProperties.P2PProperty.disabled);
        }

        public string CreateSession(string location, SessionProperties.P2PProperty p2ppreference)
        {
            return CreateSession(new SessionProperties(location, p2ppreference));
        }

        
        public string CreateSession(SessionProperties properties)
        {
            string url = "session/create";
            Dictionary<string, string> headers = new Dictionary<string, string>();
            headers.Add("Content-type", "application/x-www-form-urlencoded");
            string response = HttpOpenTok.Post(url, headers, properties.GetDictionary());
            XmlDocument xmlDoc = HttpOpenTok.ReadXmlResponse(response);
            return xmlDoc.GetElementsByTagName("session_id")[0].ChildNodes[0].Value;
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
