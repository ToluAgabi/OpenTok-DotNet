using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using OpenTokSDK.Util;

namespace OpenTokSDK
{
    public class Archive
    {       
        public enum ArchiveState
        {
            available,
            deleted,
            failed,
            started,
            stopped,
            uploaded,
            unknown
        }

        private OpenTok opentok;

        protected Archive ()
        {

        }

        public long CreatedAt { get; set; }

        public long Duration { get; set; }

        public Guid Id { get; set; }

        public string Name { get; set; }

        public int PartnerId { get; set; }
        
        public String SessionId { get; set; }             
        
        public int Size { get; set; }              

        public ArchiveState Status { get; set; }

        public String Url { get; set; }       
    }
}
