using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using Microsoft.Azure.Storage.Blob;
using Microsoft.Azure.WebJobs;


namespace AzureBlobTrigger
{
    public class EmailHandler
    {
        private string _emailTo;
        private const string _emailFrom = "artur20051011@gmail.com";
        private const string _subject = "File Uploading";
        private const string _body = "Your file has been successfully uploaded.";
        private readonly SmtpClient _client = new("smtp.gmail.com")
        {
            Port = 587,
            Credentials = new NetworkCredential(_emailFrom, "yvgqciiirontqahg"),
            EnableSsl = true
        };

        [FunctionName("EmailHandler")]
        public async Task RunAsync([BlobTrigger("taskcontainer/{name}")] CloudBlockBlob myBlob)
        {
            await myBlob.FetchAttributesAsync();
            if (!myBlob.Metadata.TryGetValue("Key1", out var emailFromMetadata)) return;

            _emailTo = emailFromMetadata;
            var mailMessage = new MailMessage(_emailFrom, _emailTo, _subject, _body);

            await _client.SendMailAsync(mailMessage);
        }
    }
}
