using System;
using System.Net.Mail;
using System.Threading.Tasks;
using Microsoft.Azure.Storage.Blob;
using Microsoft.Azure.WebJobs;

namespace AzureBlobTrigger
{
    public class EmailHandler
    {
        private readonly SmtpClient _client;
        

        public EmailHandler(SmtpClient smtpClient)
                    => _client = smtpClient;
        

        [FunctionName("EmailHandler")]
        public async Task<bool> RunAsync([BlobTrigger("taskcontainer/{name}")] CloudBlockBlob myBlob)
        {
            
            //await myBlob.FetchAttributesAsync();
            if (!myBlob.Metadata.TryGetValue("Key1", out var emailFromMetadata) || !myBlob.Metadata.TryGetValue("Key2", out var urlValue)) return false;

            var mailMessage = new MailMessage(Environment.GetEnvironmentVariable("Email"), emailFromMetadata, "File Uploading", String.Empty);
            mailMessage.Body = $@"<html>
                        <head>
                            <style>
                                .card {{
                                    width: 300px;
                                    padding: 20px;
                                    background-color: #FF5733; /* Red background color */
                                    border-radius: 10px;
                                    box-shadow: 0 4px 8px rgba(0, 0, 0, 0.1);
                                }}
                                .button-link {{
                                    display: inline-block;
                                    padding: 10px 20px;
                                    font-size: 16px;
                                    font-weight: bold;
                                    text-align: center;
                                    text-decoration: none;
                                    background-color: #FFFFFF; 
                                    color: #FF5733;
                                    border-radius: 5px;
                                    border: none;
                                    cursor: pointer;
                                }}
                            </style>
                        </head>
                            <body>
                                <div class=""card"">
                                    <h2>Glory to Ukraine! We appreciate your trust in our service</h2>
                                    <p>File content is here</p>
                                    <a href=""{urlValue}"" class=""button-link"">
                                        Install your file
                                    </a>
                                    <p>Please be understanding that this link will expire in 1 hour.</p>
                                </div>
                            </body>
                        </html>";

            mailMessage.IsBodyHtml = true;
            await _client.SendMailAsync(mailMessage);
            return true;
        }
    }
}
