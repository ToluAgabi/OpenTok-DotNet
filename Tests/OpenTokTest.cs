using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenTokSDK.Api;
using OpenTokSDK.Utils;
using System.Configuration;
using System.Collections.Generic;
using System.Xml;

namespace OpenTokSDKTest
{
    [TestClass]
    public class OpenTokTest
    {
        [TestMethod]
        public void SimpleOpenTokConstructorTest()
        {            
            int apiKey = Convert.ToInt32(ConfigurationManager.AppSettings["opentok_key"]);
            OpenTok opentok = new OpenTok();
            
            Assert.AreEqual(opentok.ApiKey, apiKey);                 
        }

        [TestMethod]
        public void OpenTokConstructorTest()
        {
            int apiKey = Convert.ToInt32(ConfigurationManager.AppSettings["opentok_key"]);
            string apiSecret = ConfigurationManager.AppSettings["opentok_secret"];
            OpenTok opentok = new OpenTok(apiKey, apiSecret);
            
            Assert.AreEqual(opentok.ApiKey, apiKey);
            Assert.AreEqual(opentok.ApiSecret, apiSecret);
        }

        [TestMethod]
        public void CreateSessionTest()
        {
            OpenTok opentok = new OpenTok();
            string sessionId = opentok.CreateSession();

            var result = GetSessionInfo(opentok, sessionId);
            Assert.IsTrue(Session.ValidateSession(sessionId));
            Assert.AreEqual(sessionId, result["sessionId"]);
        }

        [TestMethod]
        public void CreateSessionWithLocationTest()
        {
            OpenTok opentok = new OpenTok();
            string sessionId = opentok.CreateSession("localhost");

            var result = GetSessionInfo(opentok, sessionId);
            Assert.IsTrue(Session.ValidateSession(sessionId));
            Assert.AreEqual(sessionId, result["sessionId"]);
        }

        [TestMethod]
        public void CreateSessionWithP2PPreferenceDisabledTest()
        {
            OpenTok opentok = new OpenTok();
            string sessionId = opentok.CreateSession("localhost", SessionProperties.P2PProperty.disabled);

            var result = GetSessionInfo(opentok, sessionId);
            Assert.IsTrue(Session.ValidateSession(sessionId));
            Assert.AreEqual(sessionId, result["sessionId"]);
        }

        [TestMethod]
        public void CreateSessionWithP2PPreferenceEnabledTest()
        {
            OpenTok opentok = new OpenTok();
            string sessionId = opentok.CreateSession("localhost", SessionProperties.P2PProperty.enabled);

            var result = GetSessionInfo(opentok, sessionId);
            Assert.IsTrue(Session.ValidateSession(sessionId));
            Assert.AreEqual(sessionId, result["sessionId"]);
            Assert.AreEqual(SessionProperties.P2PProperty.enabled.ToString(), result["p2pPreference"]);
        }

        [TestMethod]
        public void CreateTokenTest()
        {
            int apiKey = Convert.ToInt32(ConfigurationManager.AppSettings["opentok_key"]);
            long currentTime = OpenTokUtils.GetCurrentUnixTimeStamp();

            OpenTok opentok = new OpenTok();
            string sessionId = opentok.CreateSession();
            string token = opentok.GenerateToken(sessionId);
            
            var response = ValidateToken(token);
            
            Assert.AreEqual(response["sessionId"], sessionId);
            Assert.AreEqual(response["partnerId"], apiKey);
            Assert.IsTrue(CheckTimeIsInInterval((long)response["createTime"], currentTime, currentTime + 10));            
            Assert.AreEqual(response["role"], TokenProperties.RoleProperty.publisher.ToString());
        }

        [TestMethod]
        public void CreateTokenModeratorTest()
        {
            int apiKey = Convert.ToInt32(ConfigurationManager.AppSettings["opentok_key"]);
            long currentTime = OpenTokUtils.GetCurrentUnixTimeStamp();

            OpenTok opentok = new OpenTok();
            string sessionId = opentok.CreateSession();
            string token = opentok.GenerateToken(sessionId, TokenProperties.RoleProperty.moderator);
            
            var response = ValidateToken(token);
            
            Assert.AreEqual(response["sessionId"], sessionId);
            Assert.AreEqual(response["partnerId"], apiKey);
            Assert.IsTrue(CheckTimeIsInInterval((long)response["createTime"], currentTime, currentTime + 10));
            Assert.AreEqual(response["role"], TokenProperties.RoleProperty.moderator.ToString());
        }

        [TestMethod]
        public void CreateTokenSubscriberTest()
        {
            int apiKey = Convert.ToInt32(ConfigurationManager.AppSettings["opentok_key"]);
            long currentTime = OpenTokUtils.GetCurrentUnixTimeStamp();

            OpenTok opentok = new OpenTok();
            string sessionId = opentok.CreateSession();
            string token = opentok.GenerateToken(sessionId, TokenProperties.RoleProperty.subscriber);
            
            var response = ValidateToken(token);
            
            Assert.AreEqual(response["sessionId"], sessionId);
            Assert.AreEqual(response["partnerId"], apiKey);
            Assert.IsTrue(CheckTimeIsInInterval((long)response["createTime"], currentTime, currentTime + 10));
            Assert.AreEqual(response["role"], TokenProperties.RoleProperty.subscriber.ToString());
        }

        [TestMethod]
        public void CreateTokenWithDateTimeTest()
        {
            int apiKey = Convert.ToInt32(ConfigurationManager.AppSettings["opentok_key"]);
            DateTime oneHourLater = DateTime.UtcNow.AddHours(1);
            long currentTime = OpenTokUtils.GetCurrentUnixTimeStamp();

            TokenProperties properties = new TokenProperties(TokenProperties.RoleProperty.subscriber, oneHourLater);
            OpenTok opentok = new OpenTok();
            string sessionId = opentok.CreateSession();
            string token = opentok.GenerateToken(sessionId, properties);
            
            var response = ValidateToken(token);

            Assert.AreEqual(response["sessionId"], sessionId);
            Assert.AreEqual(response["partnerId"], apiKey);
            Assert.IsTrue(CheckTimeIsInInterval((long)response["createTime"], currentTime, currentTime + 10));          
            Assert.AreEqual(response["role"], TokenProperties.RoleProperty.subscriber.ToString());
            Assert.IsTrue(CheckTimeIsInInterval((long)response["expireTime"],
                            ((long)response["createTime"]) + 3590, ((long)response["createTime"]) + 3610));
        }

        [TestMethod]
        public void CreateTokenWithConnectionDataTest()
        {
            int apiKey = Convert.ToInt32(ConfigurationManager.AppSettings["opentok_key"]);
            DateTime oneHourLater = DateTime.UtcNow.AddHours(1);
            string connectionData = "Some data for the connection";
            long currentTime = OpenTokUtils.GetCurrentUnixTimeStamp();

            TokenProperties properties = new TokenProperties(TokenProperties.RoleProperty.subscriber, oneHourLater, connectionData);
            OpenTok opentok = new OpenTok();
            string sessionId = opentok.CreateSession();
            string token = opentok.GenerateToken(sessionId, properties);
            
            var response = ValidateToken(token);

            Assert.AreEqual(response["sessionId"], sessionId);
            Assert.AreEqual(response["partnerId"], apiKey);
            Assert.IsTrue(CheckTimeIsInInterval((long) response["createTime"], currentTime, currentTime + 10));
            Assert.AreEqual(response["role"], TokenProperties.RoleProperty.subscriber.ToString());
            Assert.IsTrue(CheckTimeIsInInterval((long) response["expireTime"], 
                            ((long)response["createTime"]) + 3590, ((long)response["createTime"]) + 3610));
            Assert.AreEqual(response["connectionData"], connectionData);            
        }

        private bool CheckTimeIsInInterval(long timeToCheck, long lowerLimit, long upperLimit)
        {
            return (timeToCheck >= lowerLimit && timeToCheck <= upperLimit);
        }

        private static Dictionary<string, object> ValidateToken(string token)
        {
            string url = string.Format("token/validate");
            var headers = new Dictionary<string, string> { { "X-TB-TOKEN-AUTH", token } };
            string response = HttpOpenTok.Get(url, headers);
            XmlDocument xmlDoc = HttpOpenTok.ReadXmlResponse(response);
            var result = new Dictionary<string, object>
            {
                {"sessionId", xmlDoc.GetElementsByTagName("session_id")[0].ChildNodes[0].Value},
                {"partnerId", Convert.ToInt32(xmlDoc.GetElementsByTagName("partner_id")[0].ChildNodes[0].Value)},
                {"createTime", Convert.ToInt64(xmlDoc.GetElementsByTagName("create_time")[0].ChildNodes[0].Value)},
                {"role", xmlDoc.GetElementsByTagName("role")[0].ChildNodes[0].Value}
            };
            if (xmlDoc.GetElementsByTagName("expire_time").Count > 0)
            {
                result.Add("expireTime", Convert.ToInt64(xmlDoc.GetElementsByTagName("expire_time")[0].ChildNodes[0].Value));
            }
            if (xmlDoc.GetElementsByTagName("connection_data").Count > 0)
            {
                result.Add("connectionData", xmlDoc.GetElementsByTagName("connection_data")[0].ChildNodes[0].Value);
            }
            return result;
        }

        private Dictionary<string, object> GetSessionInfo(OpenTok opentok, string sessionId)
        {
            string token = opentok.GenerateToken(sessionId);

            string url = string.Format("session/{0}?extended=true", sessionId);
            var headers = new Dictionary<string, string> 
            { { "X-TB-TOKEN-AUTH", token } };
            string response = HttpOpenTok.Get(url, headers);
            XmlDocument xmlDoc = HttpOpenTok.ReadXmlResponse(response);
            var result = new Dictionary<string, object>
            {
                {"sessionId", xmlDoc.GetElementsByTagName("session_id")[0].ChildNodes[0].Value},
            };
            if (xmlDoc.GetElementsByTagName("preference").Count > 0)
            {
                result.Add("p2pPreference", xmlDoc.GetElementsByTagName("preference")[0].ChildNodes[0].Value);
            }
            return result;
        }
    }
}
