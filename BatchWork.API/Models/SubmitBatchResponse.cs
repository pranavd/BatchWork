using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BatchWork.API.Models
{
    public class SubmitBatchResponse
    {
        public Guid BatchGuid;
        public StatusEnum Status;
    }
}