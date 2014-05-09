using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using OpenTokSDK.Utils;

namespace OpenTokSDK.Api
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

        public Archive(OpenTok opentok)
        {
            this.opentok = opentok;
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
             
        public DateTime GetCreatedAt()
        {
            return OpenTokUtils.UnixTimeStampToDateTime(((double)CreatedAt / 1000));
        }

        public void Stop()
        {
            Archive archive = opentok.StopArchive(Id.ToString());
            Status = archive.Status;         
        }

        public void Delete()
        {
            opentok.DeleteArchive(Id.ToString());
            Status = ArchiveState.deleted;
        }

        public void Copy(Archive archive)
        {
            CreatedAt = archive.CreatedAt;
            Duration = archive.Duration;
            Id = archive.Id;
            Name = archive.Name;
            PartnerId = archive.PartnerId;
            SessionId = archive.SessionId;
            Size = archive.Size;
            Status = archive.Status;
            Url = archive.Url;
        }
    }
}
