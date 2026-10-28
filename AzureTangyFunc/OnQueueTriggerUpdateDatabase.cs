using Microsoft.Azure.Functions.Worker;
using System;
using AzureTangyFunc.Data;
using AzureTangyFunc.Models;
using Microsoft.Extensions.Logging;

namespace AzureTangyFunc
{
    public class OnQueueTriggerUpdateDatabase
    {
        private readonly AzureTangyDbContext _db;

        public OnQueueTriggerUpdateDatabase(AzureTangyDbContext db)
        {
            _db = db;
        }

        [Function("OnQueueTriggerUpdateDatabase")]
        public void Run([QueueTrigger("salesrequestinbound", Connection = "AzureWebJobsStorage")] SalesRequest myQueueItem, FunctionContext context)
        {
            var log = context.GetLogger("OnQueueTriggerUpdateDatabase");
            log.LogInformation($"C# Queue trigger function processed: {myQueueItem}");

            myQueueItem.Status = "Submitted";
            _db.SalesRequests.Add(myQueueItem);
            _db.SaveChanges();
        }
    }
}
