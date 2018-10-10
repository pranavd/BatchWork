using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using BatchWork.API.Models;
using BatchWork.API;

namespace BatchWork.App_Code
{
    public class BatchWoker
    {
        //queue incoming request
        public static Guid QueueBatchRequest(List<string> urlList)
        {
            try
            {
                var batchGuid = Guid.NewGuid();
                var batchModel = new BatchModel(urlList, batchGuid);
                if (WebApiApplication.BatchDictionary.TryAdd(batchGuid, batchModel))
                {
                    WebApiApplication.UrlQueue.Enqueue(batchModel);
                }

                return batchGuid;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //process queued request
        public static void ExecuteBatchRequest()
        {
            try
            {
                if (!WebApiApplication.UrlQueue.IsEmpty)
                {
                    if (WebApiApplication.UrlQueue.TryDequeue(out BatchModel batchModel))
                    {
                        var batchTask = Task.Factory.StartNew(() =>
                       {
                           try
                           {
                               batchModel.Status = StatusEnum.INPROGRESS;

                               foreach (var request in batchModel.Requests.Values)
                               {
                                   Task.Factory.StartNew(() =>
                                   {
                                       request.Status = StatusEnum.INPROGRESS;
                                       ExecuteDownloadRequest(request);
                                   }, TaskCreationOptions.AttachedToParent);
                               }
                           }
                           catch (Exception ex)
                           {
                               batchModel.Status = StatusEnum.FAILED;
                           }
                       }, TaskCreationOptions.AttachedToParent);
                        batchTask.Wait();

                        if (batchTask.IsCompleted)
                        {
                            var isRrequestNotCompleted = batchModel.Requests.Values.ToList()
                                .Exists(request => request.Status != StatusEnum.COMPLETED);
                            if (isRrequestNotCompleted)
                            {
                                batchModel.Status = StatusEnum.FAILED;
                            }
                            else
                            {
                                batchModel.Status = StatusEnum.COMPLETED;
                            }
                        }
                        else
                        {
                            batchModel.Status = StatusEnum.FAILED;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static void ExecuteDownloadRequest(RequestModel request)
        {
            try
            {
                using (var webClient = new WebClient())
                {
                    var response = webClient.DownloadData(request.Url);

                    if (response != null)
                    {
                        WebApiApplication.ContentBag.Add(new DownloadedContentModel()
                        {
                            RequestId = request.RequestGuid,
                            BatchId = request.BatchGuid,
                            Content = response
                        });
                        request.Status = StatusEnum.COMPLETED;
                    }
                    else
                    {
                        request.Status = StatusEnum.FAILED;
                    }
                }
            }
            catch (Exception ex)
            {
                request.Status = StatusEnum.FAILED;
            }
        }

        public static StatusEnum GetBatchStatus(string batchId)
        {
            try
            {
                Guid.TryParse(batchId, out Guid _batchId);
                WebApiApplication.BatchDictionary.TryGetValue(_batchId, out BatchModel batchModel);

                return batchModel.Status;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static Dictionary<Guid, StatusEnum> GetAllBatchStatus()
        {
            try
            {
                var status = new Dictionary<Guid, StatusEnum>();

                foreach (var batchDictionaryKey in WebApiApplication.BatchDictionary.Keys)
                {
                    status.Add(batchDictionaryKey, WebApiApplication.BatchDictionary[batchDictionaryKey].Status);
                }

                return status;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}