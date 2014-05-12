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
    public enum ArchiveState
    {
        AVAILABLE,
        DELETED,
        FAILED,
        STARTED,
        STOPPED,
        UPLOADED,
        UNKOWN
    }

    public class Archive
    {               

        private OpenTok opentok;

        protected Archive ()
        {

        }

        internal Archive(OpenTok opentok)
        {
            this.opentok = opentok;
        }

        internal void CopyArchive(Archive archive)
        {
            this.CreatedAt = archive.CreatedAt;
            this.Duration = archive.Duration;
            this.Id = archive.Id;
            this.Name = archive.Name;
            this.PartnerId = archive.PartnerId;
            this.SessionId = archive.SessionId;
            this.Size = archive.Size;
            this.Status = archive.Status;
            this.Url = archive.Url;
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
   
        public void Stop()
        {
            if(opentok != null)
            {
                Archive archive = opentok.StopArchive(Id.ToString());
                Status = archive.Status;
            }
        }

        public void Delete()
        {
            if (opentok != null)
            {
                opentok.DeleteArchive(Id.ToString());
            }
        }
    }
}
