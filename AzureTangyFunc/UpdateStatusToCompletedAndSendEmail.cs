using Microsoft.Azure.Functions.Worker;
using System;
using AzureTangyFunc.Data;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using SendGrid.Helpers.Mail;
using AzureTangyFunc.Models;
using System.Linq;

namespace AzureTangyFunc
{
    public class UpdateStatusToCompletedAndSendEmail
    {
        private readonly AzureTangyDbContext _db;
        public UpdateStatusToCompletedAndSendEmail(AzureTangyDbContext db)
        {
            _db = db;
        }

        [Function("UpdateStatusToCompletedAndSendEmail")]
        [SendGridOutput(ApiKey = "CustomSendGridKeyAppSettingName")]
        public string Run([TimerTrigger("0 */5 * * * *")] TimerInfo myTimer,
            FunctionContext context)
        {
            var log = context.GetLogger("UpdateStatusToCompletedAndSendEmail");
            log.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");

            IEnumerable<SalesRequest> salesRequestFromDb = _db.SalesRequests.Where(u => u.Status == "Image Processed");
            int salesRequestFromDbCount = salesRequestFromDb.Count();
            foreach (var salesReq in salesRequestFromDb)
            {
                //for each request update status
                salesReq.Status = "Completed";
            }

            _db.UpdateRange(salesRequestFromDb);
            _db.SaveChanges();

            var message = new SendGridMessage();
            message.AddTo("hesamshoushtarian@gmail.com");
            message.AddContent("text/html", $"Processing completed for {salesRequestFromDbCount} records");
            message.SetFrom(new EmailAddress("hello@hesam.com"));
            message.SetSubject("Azure Tangy Processing Successful");
            return Newtonsoft.Json.JsonConvert.SerializeObject(message);
        }
    }
}
