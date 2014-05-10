using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Xml;
using OpenTokSDK.Exceptions;
using OpenTokSDK.Util;

namespace OpenTokSDK
{
    public class Session
    {
        public string Id { get; set; }
        public int ApiKey { get; private set; }
        public string ApiSecret { get; private set; }
        public string Location { get; set; }
        public bool P2p { get; set; }

        private const int MAX_CONNECTION_DATA_LENGTH = 1000;

        public Session(string sessionId, int apiKey, string apiSecret)
        {
            this.Id = sessionId;
            this.ApiKey = apiKey;
            this.ApiSecret = apiSecret;
        }

        public Session(string sessionId, int apiKey, string apiSecret, string location, bool p2p)
        {
            this.Id = sessionId;
            this.ApiKey = apiKey;
            this.ApiSecret = apiSecret;
            this.Location = location;
            this.P2p = p2p;
        }


        public string GenerateToken(Role role = Role.PUBLISHER, double expireTime = 0, string data = null)
        {
            double createTime = OpenTokUtils.GetCurrentUnixTimeStamp();
            int nonce = OpenTokUtils.GetRandomNumber();

            string dataString = BuildDataString(role, expireTime, data, createTime, nonce);
            return BuildTokenString(dataString);
        }

        private string BuildTokenString(string dataString)
        {
            string signature = OpenTokUtils.EncodeHMAC(dataString, this.ApiSecret);

            StringBuilder innerBuilder = new StringBuilder();
            innerBuilder.Append(string.Format("partner_id={0}", this.ApiKey));
            innerBuilder.Append(string.Format("&sig={0}:{1}", signature, dataString));            

            byte[] innerBuilderBytes = Encoding.UTF8.GetBytes(innerBuilder.ToString());
            return "T1==" + Convert.ToBase64String(innerBuilderBytes);
        }

        private string BuildDataString(Role role, double expireTime, string connectionData, double createTime, int nonce)
        {   
            StringBuilder dataStringBuilder = new StringBuilder();

            dataStringBuilder.Append(string.Format("session_id={0}", this.Id));
            dataStringBuilder.Append(string.Format("&create_time={0}", (long) createTime));
            dataStringBuilder.Append(string.Format("&nonce={0}", nonce));
            dataStringBuilder.Append(string.Format("&role={0}", role.ToString()));   

            if (CheckExpireTime(expireTime, createTime))
            {
                dataStringBuilder.Append(string.Format("&expire_time={0}", (long) expireTime));
            }

            if (CheckConnectionData(connectionData))
            {
                dataStringBuilder.Append(string.Format("&connection_data={0}", HttpUtility.UrlEncode(connectionData)));
            }

            return dataStringBuilder.ToString();
        }

        private bool CheckExpireTime(double expireTime, double createTime)
        {
            if (expireTime == 0)
            {
                return false;
            }
            else if (expireTime > createTime && expireTime <= OpenTokUtils.GetCurrentUnixTimeStamp() + 2592000)
            {
                return true;
            }
            else
            {
                throw new OpenTokArgumentException("Invalid expiration time for token " + expireTime + ". Expiration time " + 
                                                        " has to be positive and less than 30 days");
            }
        }

        private bool CheckConnectionData(string connectionData)
        {
            if (String.IsNullOrEmpty(connectionData))
            {
                return false;
            }
            else if (connectionData.Length <= MAX_CONNECTION_DATA_LENGTH)
            {
                return true;
            }
            else
            {
                throw new OpenTokArgumentException("Invalid connection data, it cannot be longer than 1000 characters");
            }
        }
    }
}
