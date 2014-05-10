using System;
using Xunit;
using Moq;
using OpenTokSDK;
using OpenTokSDK.Utils;
using System.Configuration;
using System.Collections.Generic;
using System.Xml;

namespace OpenTokSDKTest
{
    public class OpenTokTest
    {
        private OpenTok opentok = new OpenTok(123456,"1234567890abcdef1234567890abcdef1234567890");       

        [Fact]
        public void CreateSimpleSessionTest()
        {
            Session session = opentok.CreateSession();
        }

        [Fact]
        public void CreateSessionWithLocationTest()
        {
            Session session = opentok.CreateSession(new SessionProperties.Builder().Location("").build());
        }

        [Fact]
        public void CreateSessionWithP2pDisabledTest()
        {
            Session session = opentok.CreateSession(new SessionProperties.Builder().P2p(false).build());
        }

        [Fact]
        public void CreateSessionWithP2pDisabledAndLocationTest()
        {
            Session session = opentok.CreateSession(new SessionProperties.Builder().Location("").P2p(false).build());

        }

        [Fact]
        public void CreateInvalidSessionLocationTest()
        {
            Session session = opentok.CreateSession(new SessionProperties.Builder().Location("Invalid IP").build());

        }
 
        [Fact]
        public void GenerateTokenTest()
        {           
            String sessionId = "1_MX4xMjM0NTZ-flNhdCBNYXIgMTUgMTQ6NDI6MjMgUERUIDIwMTR-MC40OTAxMzAyNX4";          
            string token = opentok.GenerateToken(sessionId);
        }

        [Fact]
        public void GenerateTokenWithExpireTimeTest()
        {
            String sessionId = "1_MX4xMjM0NTZ-flNhdCBNYXIgMTUgMTQ6NDI6MjMgUERUIDIwMTR-MC40OTAxMzAyNX4";                      
            DateTime date = DateTime.UtcNow.AddHours(1);
            double oneHour = (date - new DateTime(1970, 1, 1, 0, 0, 0)).TotalSeconds;
            string token = opentok.GenerateToken(sessionId, new TokenOptions.Builder().ExpireTime(oneHour).build());
        }

        [Fact]
        public void GenerateTokenWithConnectionDataTest()
        {
            String sessionId = "1_MX4xMjM0NTZ-flNhdCBNYXIgMTUgMTQ6NDI6MjMgUERUIDIwMTR-MC40OTAxMzAyNX4";                      
            string token = opentok.GenerateToken(sessionId, new TokenOptions.Builder().Data("Some data for the connection").build());
        }

        [Fact]
        public void GenerateTokenWithInvalidSessionTest()
        {
           string token = opentok.GenerateToken(null);
        }

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
    }
}
