namespace Bislerium.API.Respositories.Respository
{
    public interface IEmailServiceRespository
    {
        Task SendEmailAsync(string to, string subject, string body);
    }
}
