using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BatchWork.API.Models
{
    public class DownloadedContentModel
    {
        public Guid BatchId;
        public Guid RequestId;
        public byte[] Content;
    }
}