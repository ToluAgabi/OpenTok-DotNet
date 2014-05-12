using System;
using Xunit;
using Moq;
using OpenTokSDK;
using OpenTokSDK.Util;
using System.Configuration;
using System.Collections.Generic;
using System.Xml;
using OpenTokSDK.Exceptions;

namespace OpenTokSDKTest
{
    public class OpenTokTest
    {
        private int apiKey = 123456;
        private string apiSecret = "1234567890abcdef1234567890abcdef1234567890";
        
        [Fact]
        public void CreateSimpleSessionTest()
        {
            string sessionId = "SESSIONID";
            string returnString = "<?xml version=\"1.0\" encoding=\"UTF-8\" standalone=\"yes\"?><sessions><Session><" +
                                "session_id>" + sessionId + "</session_id><partner_id>123456</partner_id><create_dt>" +
                                "Mon Mar 17 00:41:31 PDT 2014</create_dt></Session></sessions>";
            var expectedUrl = "session/create";

            var mockClient = new Mock<HttpClient>();
            mockClient.Setup(httpClient => httpClient.Post(It.IsAny<string>(), It.IsAny<Dictionary<string, string>>(), It.IsAny<Dictionary<string, object>>())).Returns(returnString);
            
            HttpClient client = mockClient.Object;

            OpenTok opentok = new OpenTok(apiKey, apiSecret);
            opentok.Client = client;
            Session session = opentok.CreateSession();

            Assert.NotNull(session);
            Assert.Equal(this.apiKey, session.ApiKey);
            Assert.Equal(sessionId, session.Id);
            Assert.False(session.P2p);
            Assert.Equal(session.Location, "");

            mockClient.Verify(httpClient => httpClient.Post(It.Is<string>(url => url.Equals(expectedUrl)), It.IsAny<Dictionary<string, string>>(), It.IsAny<Dictionary<string, object>>()), Times.Once());
        }

        [Fact]
        public void CreateP2pSessionTest()
        {
            string sessionId = "SESSIONID";
            string returnString = "<?xml version=\"1.0\" encoding=\"UTF-8\" standalone=\"yes\"?><sessions><Session><" +
                                "session_id>" + sessionId + "</session_id><partner_id>123456</partner_id><create_dt>" +
                                "Mon Mar 17 00:41:31 PDT 2014</create_dt></Session></sessions>";
            var expectedUrl = "session/create";

            var mockClient = new Mock<HttpClient>();
            mockClient.Setup(httpClient => httpClient.Post(It.IsAny<string>(), It.IsAny<Dictionary<string, string>>(), It.IsAny<Dictionary<string, object>>())).Returns(returnString);

            HttpClient client = mockClient.Object;

            OpenTok opentok = new OpenTok(apiKey, apiSecret);
            opentok.Client = client;
            Session session = opentok.CreateSession(p2p: true);

            Assert.NotNull(session);
            Assert.Equal(this.apiKey, session.ApiKey);
            Assert.Equal(sessionId, session.Id);
            Assert.True(session.P2p);
            Assert.Equal(session.Location, "");

            mockClient.Verify(httpClient => httpClient.Post(It.Is<string>(url => url.Equals(expectedUrl)), It.IsAny<Dictionary<string, string>>(), It.IsAny<Dictionary<string, object>>()), Times.Once());
        }

        [Fact]
        public void CreateSessionWithLocationTest()
        {
            string sessionId = "SESSIONID";
            string returnString = "<?xml version=\"1.0\" encoding=\"UTF-8\" standalone=\"yes\"?><sessions><Session><" +
                                "session_id>" + sessionId + "</session_id><partner_id>123456</partner_id><create_dt>" +
                                "Mon Mar 17 00:41:31 PDT 2014</create_dt></Session></sessions>";
            var expectedUrl = "session/create";

            var mockClient = new Mock<HttpClient>();
            mockClient.Setup(httpClient => httpClient.Post(It.IsAny<string>(), It.IsAny<Dictionary<string, string>>(), It.IsAny<Dictionary<string, object>>())).Returns(returnString);

            HttpClient client = mockClient.Object;

            OpenTok opentok = new OpenTok(apiKey, apiSecret);
            opentok.Client = client;
            Session session = opentok.CreateSession(location: "0.0.0.0");

            Assert.NotNull(session);
            Assert.Equal(this.apiKey, session.ApiKey);
            Assert.Equal(sessionId, session.Id);
            Assert.False(session.P2p);
            Assert.Equal(session.Location, "0.0.0.0");

            mockClient.Verify(httpClient => httpClient.Post(It.Is<string>(url => url.Equals(expectedUrl)), It.IsAny<Dictionary<string, string>>(), 
                            It.IsAny<Dictionary<string, object>>()), Times.Once());
        }
   
        [Fact]
        public void CreateP2pSessionWithLocationTest()
        {
            string sessionId = "SESSIONID";
            string returnString = "<?xml version=\"1.0\" encoding=\"UTF-8\" standalone=\"yes\"?><sessions><Session><" +
                                "session_id>" + sessionId + "</session_id><partner_id>123456</partner_id><create_dt>" +
                                "Mon Mar 17 00:41:31 PDT 2014</create_dt></Session></sessions>";
            var expectedUrl = "session/create";

            var mockClient = new Mock<HttpClient>();
            mockClient.Setup(httpClient => httpClient.Post(It.IsAny<string>(), It.IsAny<Dictionary<string, string>>(), It.IsAny<Dictionary<string, object>>())).Returns(returnString);

            HttpClient client = mockClient.Object;

            OpenTok opentok = new OpenTok(apiKey, apiSecret);
            opentok.Client = client;
            Session session = opentok.CreateSession(location: "0.0.0.0", p2p: true);

            Assert.NotNull(session);
            Assert.Equal(this.apiKey, session.ApiKey);
            Assert.Equal(sessionId, session.Id);
            Assert.True(session.P2p);
            Assert.Equal(session.Location, "0.0.0.0");

            mockClient.Verify(httpClient => httpClient.Post(It.Is<string>(url => url.Equals(expectedUrl)), It.IsAny<Dictionary<string, string>>(), It.IsAny<Dictionary<string, object>>()), Times.Once());
        }

        [Fact]
        public void CreateInvalidSessionLocationTest()
        {
            var mockClient = new Mock<HttpClient>();
            mockClient.Setup(httpClient => httpClient.Post(It.IsAny<string>(), It.IsAny<Dictionary<string, string>>(), It.IsAny<Dictionary<string, object>>())).Returns("This function should not return anything");

            OpenTok opentok = new OpenTok(apiKey, apiSecret);
            Session session;
            try
            {
                session = opentok.CreateSession(location: "A location");
                Assert.True(false);
            }
            catch (OpenTokArgumentException)
            {
                Assert.True(true);
            }
        }

        [Fact]
        public void GenerateTokenTest()
        {
            OpenTok opentok = new OpenTok(apiKey, apiSecret);
            
            String sessionId = "1_MX4xMjM0NTZ-flNhdCBNYXIgMTUgMTQ6NDI6MjMgUERUIDIwMTR-MC40OTAxMzAyNX4";          
            string token = opentok.GenerateToken(sessionId);

            Assert.NotNull(token);
            var data = CheckToken(token, apiKey);

            Assert.Equal(data["partner_id"], apiKey.ToString());
            Assert.NotNull(data["sig"]);
            Assert.NotNull(data["create_time"]);
            Assert.NotNull(data["nonce"]);
            Assert.Equal(data["role"], Role.PUBLISHER.ToString());
        }

        [Fact]
        public void GenerateTokenWithRoleTest()
        {
            OpenTok opentok = new OpenTok(apiKey, apiSecret);

            String sessionId = "1_MX4xMjM0NTZ-flNhdCBNYXIgMTUgMTQ6NDI6MjMgUERUIDIwMTR-MC40OTAxMzAyNX4";
            string token = opentok.GenerateToken(sessionId, role:Role.SUBSCRIBER);

            Assert.NotNull(token);
            var data = CheckToken(token, apiKey);

            Assert.Equal(data["partner_id"], apiKey.ToString());
            Assert.NotNull(data["sig"]);
            Assert.NotNull(data["create_time"]);
            Assert.NotNull(data["nonce"]);
            Assert.Equal(data["role"], Role.SUBSCRIBER.ToString());
        }
        
        
        [Fact]
        public void GenerateTokenWithExpireTimeTest()
        {
            OpenTok opentok = new OpenTok(apiKey, apiSecret);
            double expireTime = OpenTokUtils.GetCurrentUnixTimeStamp() + 10;

            String sessionId = "1_MX4xMjM0NTZ-flNhdCBNYXIgMTUgMTQ6NDI6MjMgUERUIDIwMTR-MC40OTAxMzAyNX4";
            string token = opentok.GenerateToken(sessionId, expireTime: expireTime);

            Assert.NotNull(token);
            var data = CheckToken(token, apiKey);

            Assert.Equal(data["partner_id"], apiKey.ToString());
            Assert.NotNull(data["sig"]);
            Assert.NotNull(data["create_time"]);
            Assert.NotNull(data["nonce"]);
            Assert.Equal(data["role"], Role.PUBLISHER.ToString());
            Assert.Equal(data["expire_time"], ((long) expireTime).ToString());
        }

        [Fact]
        public void GenerateTokenWithConnectionDataTest()
        {
            OpenTok opentok = new OpenTok(apiKey, apiSecret);
            double expireTime = OpenTokUtils.GetCurrentUnixTimeStamp() + 10;
            string connectionData =  "Somedatafortheconnection";
            String sessionId = "1_MX4xMjM0NTZ-flNhdCBNYXIgMTUgMTQ6NDI6MjMgUERUIDIwMTR-MC40OTAxMzAyNX4";
            string token = opentok.GenerateToken(sessionId, data:connectionData);

            Assert.NotNull(token);
            var data = CheckToken(token, apiKey);

            Assert.Equal(data["partner_id"], apiKey.ToString());
            Assert.NotNull(data["sig"]);
            Assert.NotNull(data["create_time"]);
            Assert.NotNull(data["nonce"]);
            Assert.Equal(data["role"], Role.PUBLISHER.ToString());
            Assert.Equal(data["connection_data"], connectionData);
        }

        [Fact]
        public void GenerateInvalidTokensTest()
        {
            string token;
            OpenTok opentok = new OpenTok(apiKey, apiSecret);
            var exceptions = new List<Exception>();
            try
            {
                // Generate token with empty sessionId
                token = opentok.GenerateToken(null);
            }
            catch(OpenTokArgumentException e)
            {
                exceptions.Add(e);
            }

            try
            {
                // Generate token with empty sessionId
                token = opentok.GenerateToken("");
            }
            catch (OpenTokArgumentException e)
            {
                exceptions.Add(e);
            }

            try
            {
                // Generate token with empty sessionId
                token = opentok.GenerateToken("NOT A VALID SESSION ID");
            }
            catch (OpenTokArgumentException e)
            {
                exceptions.Add(e);
            }

            Assert.Equal(exceptions.Count, 3);
            foreach(Exception exception in exceptions)
            {
                Assert.True(exception is OpenTokArgumentException);
            }

        }
/*
        [Fact]
        public void GetArchiveTest()
        {
            String archiveId = "ARCHIVEID";
            Archive archive = opentok.GetArchive(archiveId);
        }

        [Fact]
        public void ListArchivesTest()
        {
            List<Archive> archives = opentok.ListArchives();
        }

        [Fact]
        public void StartArchiveTest()
        {
            String sessionId = "1_MX4xMjM0NTZ-flNhdCBNYXIgMTUgMTQ6NDI6MjMgUERUIDIwMTR-MC40OTAxMzAyNX4";                                  
            Archive archive = opentok.StartArchive(sessionId, null);
        }

        [Fact]
        public void StopArchiveTest()
        {
            String archiveId = "ARCHIVEID";
            Archive archive = opentok.StopArchive(archiveId);
        }

        [Fact]
        public void DeleteArchiveTest()
        {
             String archiveId = "ARCHIVEID";
            opentok.DeleteArchive(archiveId);
        }

        private bool ValidateSession(string sessionId)
        {
            try
            {
                return GetPartnerIdFromSessionId(sessionId) > 0;
            }
            catch (FormatException)
            {
                return false;
            }
        }

        private int GetPartnerIdFromSessionId(string sessionId)
        {
            if (String.IsNullOrEmpty(sessionId))
            {
                throw new FormatException("SessionId can not be empty");
            }

            string formatedSessionId = sessionId.Replace('-', '+');
            string[] splittedSessionId = OpenTokUtils.SplitString(formatedSessionId, '_', 2);
            if (splittedSessionId == null)
            {
                throw new FormatException("Session id could not be decoded");
            }

            string decodedSessionId = OpenTokUtils.Decode64(splittedSessionId[1]);

            string[] sessionParameters = OpenTokUtils.SplitString(decodedSessionId, '~', 0);
            if (sessionParameters == null)
            {
                throw new FormatException("Session id could not be decoded");
            }

            return Convert.ToInt32(sessionParameters[1]);
        }*/

        private Dictionary<string,string> CheckToken(string token, int apiKey)
        {
            string baseToken = OpenTokUtils.Decode64(token.Substring(4));
            char[] sep = { '&' };
            string[] tokenFields = baseToken.Split(sep);
            var tokenData = new Dictionary<string, string>();

            for (int i = 0; i < tokenFields.Length; i ++)
            {
                tokenData.Add(tokenFields[i].Split('=')[0], tokenFields[i].Split('=')[1]);
            }
            return tokenData;
        }
    }
}
