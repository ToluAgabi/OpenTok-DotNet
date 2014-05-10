using System;
using System.Collections.Generic;
using System.Linq;

namespace OpenTokSDK
{
    public class SessionProperties
    {
        public enum P2PProperty
        {
            enabled,
            disabled
        }
        //GGB estas constantes pueden ser privadas ?
        public const string LOCATION = "location";
        public const string P2P_PREFERENCE = "p2p.preference";

        public string Location { get; set; }

        public P2PProperty P2Ppreference { get; set; }

        public SessionProperties()
        {
            Location = "";
            P2Ppreference = P2PProperty.disabled;
        }

        public SessionProperties(string location)
        {
            Location = location;
            P2Ppreference = P2PProperty.disabled;
        }

        public SessionProperties(P2PProperty preference)
        {
            Location = "";
            P2Ppreference = preference;
        }

        public SessionProperties(string location, P2PProperty preference)
        {
            Location = location;
            P2Ppreference = preference;
        }

        public Dictionary<string, object> GetDictionary()
        {
            return new Dictionary<string, object> 
                {
                    {LOCATION, Location},
                    {P2P_PREFERENCE, P2Ppreference.ToString()}
                };
        }
    }
}