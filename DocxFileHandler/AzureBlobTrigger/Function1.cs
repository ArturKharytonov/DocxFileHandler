using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace AzureBlobTrigger
{
    public class Function1
    {
        [FunctionName("SendEmailWhenFileUploaded")]
        public void Run([BlobTrigger("taskcontainer/{name}", Connection = "")]string myBlob,string name, ILogger log)
        {
            log.LogInformation($"Name: {name}. Data {myBlob}");
            //dynamic message = JsonConvert.DeserializeObject(myQueueItem);
            //string blobName = message.blobName;
            //string email = message.email;
        }
    }
}
