using Bislerium.API.Respositories.Respository;
using FluentEmail.Core;
using System.Net;
using System.Net.Mail;


namespace Bislerium.API.Respositories.Implementation
{
    public class EmailServiceRespositoy:IEmailServiceRespository
    {
        public async Task SendEmailAsync(string to, string subject, string body)
        {
            await Email
            .From("Wcaavash@gmail.com")
            .To(to)
            .Subject(subject)
            .Body(body, true)
            .SendAsync();

        }
    }
}
