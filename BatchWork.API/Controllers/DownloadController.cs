using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using BatchWork;
using BatchWork.App_Code;
using BatchWork.API.Models;

namespace BatchWork.Controllers
{
    [RoutePrefix("api/download")]
    public class DownloadController : ApiController
    {
        /// <summary>
        /// Takes array of URL as input, returns batch-id (GUID) along with Status
        /// </summary>
        /// <param name="urls"></param>
        /// <returns></returns>
        [HttpPost, Route("submit")]
        [ResponseType(typeof(SubmitBatchResponse))]
        public HttpResponseMessage SubmitBatch(string[] urls)
        {
            try
            {
                urls.ToList().ForEach(url =>
                {
                    if (!Uri.IsWellFormedUriString(url, UriKind.RelativeOrAbsolute))
                    {
                        throw new Exception($"URL {url} is not in proper format");
                    }
                });

                var batchGuid = BatchWoker.QueueBatchRequest(urls.ToList());

                return Request.CreateResponse(HttpStatusCode.Accepted, new SubmitBatchResponse()
                {
                    BatchGuid = batchGuid,
                    Status = StatusEnum.QUEUED
                });
            }
            catch (Exception ex)
            {
                throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message));
            }
        }

        /// <summary>
        /// Takes batch-id (GUID) as input, returns status of batch
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet, Route("status")]
        [ResponseType(typeof(StatusEnum))]
        public HttpResponseMessage GetBatchStatus(string id)
        {
            try
            {
                var status = BatchWoker.GetBatchStatus(id);

                return Request.CreateResponse(HttpStatusCode.Accepted, status);
            }
            catch (Exception ex)
            {
                throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message));
            }
        }
    }
}
