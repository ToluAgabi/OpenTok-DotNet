using System;
using System.Collections.Generic;
using System.Linq;
using OpenTokSDK.Utils;
using OpenTokSDK.Exceptions;

namespace OpenTokSDK
{
    public class TokenProperties
    {
        public enum RoleProperty
        {
            subscriber,
            publisher,
            moderator
        }

        public enum TokenProperty
        {
            role,
            expire_time,
            connection_data,
        }

        public RoleProperty Role { get; set; }

        public string ConnectionData { get; set; }

        public DateTime? ExpireTime { get; set; }

        public long GetExpireTimeInUnixTimeStamp()
        {
            if (ExpireTime != null)
            {
                return OpenTokUtils.GetUnixTimeStampForDate((DateTime) ExpireTime);
            }
            return 0;            
        }

        public TokenProperties()
        {
            this.Role = RoleProperty.publisher;
            this.ConnectionData = "";
            this.ExpireTime = null;
        }

        public TokenProperties(RoleProperty role)
        {
            this.Role = role;
            this.ConnectionData = "";
            this.ExpireTime = null;
        }

        public TokenProperties(RoleProperty role, DateTime expireTime)
        {
            this.Role = role;
            this.ConnectionData = "";
            this.ExpireTime = expireTime;
        }

        public TokenProperties(RoleProperty role, string connectionData)
        {
            this.Role = role;
            this.ConnectionData = "";
            this.ExpireTime = null;
        }

        public TokenProperties(RoleProperty role, DateTime expireTime, string connectionData)
        {
            this.Role = role;
            this.ConnectionData = connectionData;
            this.ExpireTime = expireTime;
        }
    }
}