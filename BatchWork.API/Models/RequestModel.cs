using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BatchWork.API.Models
{
    public class RequestModel
    {
        public readonly Guid RequestGuid;
        public readonly Guid BatchGuid;
        public readonly string Url;
        public StatusEnum Status;

        public RequestModel(string url, Guid batchGuid, Guid requestGuid)
        {
            Url = url;
            BatchGuid = batchGuid;
            RequestGuid = requestGuid;
        }
    }
}