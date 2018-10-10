using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Hosting;
using BatchWork.API;
using BatchWork.API.Models;
using BatchWork.App_Code;
using BatchWork.Controllers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;

namespace BatchWork.Test
{
    [TestClass]
    public class BatchWorkApiTest
    {
        private readonly int _urlCount = 2;
        private readonly string _testDownloadUrl = "https://github.com/Microsoft/dotnet/archive/master.zip";

        [TestInitialize]
        public void Initialize()
        {

        }

        /// <summary>
        /// test action "SubmitBatch"
        /// </summary>
        [TestMethod]
        public void Test_Action_SubmitBatch()
        {
            try
            {
                var submitResponse = Test_SubmitBatch();

                if (submitResponse != null)
                {
                    var batchGuid = submitResponse.BatchGuid;

                    Assert.IsNotNull(WebApiApplication.BatchDictionary);
                    Assert.IsTrue(WebApiApplication.BatchDictionary[batchGuid].Status == StatusEnum.QUEUED);
                    Assert.IsTrue(WebApiApplication.BatchDictionary[batchGuid].Requests.Count == _urlCount);
                    Assert.IsNotNull(WebApiApplication.UrlQueue);
                }
                else
                {
                    Assert.Fail();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        /// <summary>
        /// test action "GetBatchStatus" for status other than "COMPLETED"
        /// </summary>
        [TestMethod]
        public void Test_Action_GetBatchStatus()
        {
            try
            {
                var status = Test_GetBatchStatus(false);
                Assert.IsTrue(status == StatusEnum.INPROGRESS || status == StatusEnum.FAILED || status == StatusEnum.QUEUED);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        /// <summary>
        /// test action "GetBatchStatus" for status "COMPLETED"
        /// </summary>
        [TestMethod]
        public void Test_Action_GetBatchStatus_Completed()
        {
            try
            {
                var status = Test_GetBatchStatus(true);
                Assert.IsTrue(status == StatusEnum.COMPLETED);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        private SubmitBatchResponse Test_SubmitBatch()
        {
            try
            {
                var urlList = Enumerable.Repeat(_testDownloadUrl, _urlCount).ToArray();

                //trick to work with HttpRequestMessage class in controller
                var controller = new DownloadController
                {
                    Request = new HttpRequestMessage()
                };
                controller.Request.Properties.Add(HttpPropertyKeys.HttpConfigurationKey,
                    new HttpConfiguration());

                //calling target action
                var response = controller.SubmitBatch(urlList);

                if (response != null)
                {
                    var responseReadTask = Task.Factory.StartNew(() =>
                    {
                        return response.Content.ReadAsStringAsync();
                    });
                    responseReadTask.Wait();

                    var result = responseReadTask.Result.Result;

                    if (string.IsNullOrEmpty(result))
                    {
                        throw new Exception("empty response from batch submit request");
                    }
                    else
                    {
                        //deserailize object
                        var submitBatchResponse = JsonConvert.DeserializeObject<SubmitBatchResponse>(result);
                        return submitBatchResponse;
                    }
                }
                else
                {
                    throw new Exception("null response for batch submit request");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public StatusEnum Test_GetBatchStatus(bool checkCompleted)
        {
            try
            {
                //submiting batch request
                var submitResponse = Test_SubmitBatch();

                if (submitResponse != null)
                {
                    var batchGuid = submitResponse.BatchGuid;

                    var controller = new DownloadController
                    {
                        Request = new HttpRequestMessage()
                    };
                    controller.Request.Properties.Add(HttpPropertyKeys.HttpConfigurationKey,
                        new HttpConfiguration());

                    var dequeueWorker = Task.Factory.StartNew(() => { BatchWoker.ExecuteBatchRequest(); });

                    //if test is for "COMPLETED" batch then wait for all tasks to finish, else provide the status immediately
                    if (checkCompleted)
                    {
                        dequeueWorker.Wait();
                        if (dequeueWorker.IsCompleted)
                        {
                            //calling target action
                            var response = controller.GetBatchStatus(submitResponse.BatchGuid.ToString());

                            if (response != null)
                            {
                                var responseReadTask = Task.Factory.StartNew(() =>
                                {
                                    return response.Content.ReadAsStringAsync();
                                });

                                var result = responseReadTask.Result.Result;

                                if (string.IsNullOrEmpty(result))
                                {
                                    throw new Exception("empty response from batch status request");
                                }
                                else
                                {
                                    try
                                    {
                                        return JsonConvert.DeserializeObject<StatusEnum>(result);
                                    }
                                    catch (Exception e)
                                    {
                                        throw new Exception("failed to deserialize batch status response", e);
                                    }
                                }
                            }
                            else
                            {
                                throw new Exception("null response from batch status request");
                            }
                        }
                        else
                        {
                            throw new Exception("failed to execute batch request");
                        }
                    }
                    else
                    {
                        //calling target action
                        var response = controller.GetBatchStatus(submitResponse.BatchGuid.ToString());

                        if (response != null)
                        {
                            var responseReadTask = Task.Factory.StartNew(() =>
                            {
                                return response.Content.ReadAsStringAsync();
                            });

                            var result = responseReadTask.Result.Result;

                            if (string.IsNullOrEmpty(result))
                            {
                                throw new Exception("empty response from batch status request");
                            }
                            else
                            {
                                try
                                {
                                    return JsonConvert.DeserializeObject<StatusEnum>(result);
                                }
                                catch (Exception e)
                                {
                                    throw new Exception("failed to deserialize batch status response", e);
                                }
                            }
                        }
                        else
                        {
                            throw new Exception("null response from batch status request");
                        }
                    }
                }
                else
                {
                    throw new Exception("null response for batch submit request");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
    }
}
