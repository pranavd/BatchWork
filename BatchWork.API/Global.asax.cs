using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Routing;
using BatchWork.API.Models;
using BatchWork.App_Code;

namespace BatchWork.API
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        public static ConcurrentDictionary<Guid, BatchModel> BatchDictionary = new ConcurrentDictionary<Guid, BatchModel>();
        public static ConcurrentQueue<BatchModel> UrlQueue = new ConcurrentQueue<BatchModel>();
        public static ConcurrentBag<DownloadedContentModel> ContentBag = new ConcurrentBag<DownloadedContentModel>();

        protected void Application_Start()
        {
            GlobalConfiguration.Configure(WebApiConfig.Register);
            AppStart();
        }

        public void AppStart()
        {
            Task.Factory.StartNew(() =>
            {
                try
                {
                    while (true)
                    {
                        BatchWoker.ExecuteBatchRequest();
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }
            });
        }
    }
}
