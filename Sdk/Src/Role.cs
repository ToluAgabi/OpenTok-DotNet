using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenTokSDK
{  
    public enum Role
    {
        PUBLISHER,
        SUBSCRIBER,
        MODERATOR
    }

    static class RoleExtensions
    {
        public static string ToString(this Role role)
        {
            return role.ToString().ToLower();
        }
    }
}
