using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Xml;
using OpenTokSDK.Exceptions;
using OpenTokSDK.Utils;

namespace OpenTokSDK
{
    public class Session
    {
        public string Id { get; set; }
        private int ApiKey { get; set; }
        private string ApiSecret { get; set; }
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

        public string GenerateToken(TokenProperties properties)
        {
            long createTime = OpenTokUtils.GetCurrentUnixTimeStamp();
            long expireTime = properties.GetExpireTimeInUnixTimeStamp();
            int nonce = OpenTokUtils.GetRandomNumber();

            string dataString = BuildDataString(properties, createTime, nonce);
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

        // 
        private string BuildDataString(TokenProperties properties, long createTime, int nonce)
        {   
            StringBuilder dataStringBuilder = new StringBuilder();
            long expireTime = properties.GetExpireTimeInUnixTimeStamp();
            string connectionData = properties.ConnectionData;

            dataStringBuilder.Append(string.Format("session_id={0}", this.Id));
            dataStringBuilder.Append(string.Format("&create_time={0}", createTime));
            dataStringBuilder.Append(string.Format("&nonce={0}", nonce));
            dataStringBuilder.Append(string.Format("&role={0}", properties.Role.ToString()));   

            if (CheckExpireTime(expireTime))
            {
                dataStringBuilder.Append(string.Format("&expire_time={0}", expireTime));
            }

            if (CheckConnectionData(connectionData))
            {
                dataStringBuilder.Append(string.Format("&connection_data={0}", HttpUtility.UrlEncode(connectionData)));
            }

            return dataStringBuilder.ToString();
        }

        private bool CheckExpireTime(long expireTime)
        {
            if (expireTime == 0)
            {
                return false;
            }
            else if (expireTime > 0 && expireTime <=  OpenTokUtils.GetCurrentUnixTimeStamp() + 2592000)
            {
                return true;
            }
            else
            {
                throw new OpenTokInvalidArgumentException("Invalid expiration time for token " + expireTime + ". Expiration time " + 
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
                throw new OpenTokInvalidArgumentException("Invalid connection data, it cannot be longer than 1000 characters");
            }
        }
    }
}
