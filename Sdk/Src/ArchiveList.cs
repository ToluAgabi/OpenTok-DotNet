using System;
using System.Collections.Generic;
using System.Linq;

namespace OpenTokSDK
{
    public class ArchiveList: List<Archive>
    {
        public int TotalCount { get; private set; }

        internal ArchiveList(List<Archive> items, int totalCount) : base(items)
        {
            TotalCount = totalCount;
        }
    }
}