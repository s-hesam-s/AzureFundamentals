using Microsoft.Azure.Functions.Worker;
using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using AzureTangyFunc.Models;

namespace AzureTangyFunc
{
    public static class OnSalesUploadWriteToQueue
    {
        [Function("OnSalesUploadWriteToQueue")]
        public static async Task<SalesRequestOutput> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            FunctionContext context)
        {
            var log = context.GetLogger("OnSalesUploadWriteToQueue");
            log.LogInformation("Sales Request received by OnSalesUploadWriteToQueue function");

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            SalesRequest data = JsonConvert.DeserializeObject<SalesRequest>(requestBody);


            string responseMessage = "Sales Request has been received for ." + data.Name;
            return new SalesRequestOutput { QueueItem = data, Response = new OkObjectResult(responseMessage) };
        }
    }
}

namespace AzureTangyFunc
{
    public sealed class SalesRequestOutput
    {
        [QueueOutput("salesrequestinbound", Connection = "AzureWebJobsStorage")]
        public SalesRequest QueueItem { get; set; }

        [HttpResult]
        public IActionResult Response { get; set; }
    }
}
