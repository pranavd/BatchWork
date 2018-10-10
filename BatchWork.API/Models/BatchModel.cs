using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BatchWork.API.Models
{
    public class BatchModel
    {
        public readonly Guid BatchGuid;
        public Dictionary<Guid, RequestModel> Requests;
        public StatusEnum Status;

        public BatchModel(List<string> urlList, Guid batchGuid)
        {
            BatchGuid = batchGuid;
            Requests = new Dictionary<Guid, RequestModel>();
            Status = StatusEnum.QUEUED;

            foreach (var url in urlList)
            {
                var requestGuid = Guid.NewGuid();
                Requests.Add(requestGuid, new RequestModel(url, batchGuid, requestGuid));
            }
        }
    }
}