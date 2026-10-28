using Microsoft.Azure.Functions.Worker;
using System;
using System.IO;
using Microsoft.Extensions.Logging;

namespace AzureTangyFunc
{
    public static class ResizeImageOnBlobUpload
    {
        [Function("ResizeImageOnBlobUpload")]
        [BlobOutput("functionsalesrep-sm/{name}", Connection = "AzureWebJobsStorage")]
        public static byte[] Run([BlobTrigger("functionsalesrep/{name}", Connection = "AzureWebJobsStorage")] Stream myBlob,
            string name, FunctionContext context)
        {
            var log = context.GetLogger("ResizeImageOnBlobUpload");
            log.LogInformation("Resizing uploaded image {Name}", name);
            return ImageThumbnail.Resize(myBlob);
        }
    }
}
