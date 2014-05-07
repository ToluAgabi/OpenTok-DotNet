using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OpenTokSDK.Api
{
    public class ArchiveList
    {
        public int Count { get; set; }

        public List<Archive> Items { get; set; }
    }
}