using System;
using System.Net;
using System.Net.Mail;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

[assembly: FunctionsStartup(typeof(AzureBlobTrigger.Startup))]

namespace AzureBlobTrigger
{
    internal class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            builder.Services.AddScoped<SmtpClient>(sp =>
            {
                var smtpClient = new SmtpClient("smtp.gmail.com")
                {
                    Port = 587,
                    Credentials = new NetworkCredential(Environment.GetEnvironmentVariable("Email"), Environment.GetEnvironmentVariable("Password")),
                    EnableSsl = true
                };
                return smtpClient;
            });
            builder.Services.AddScoped<EmailHandler>();
        }
    }
}
