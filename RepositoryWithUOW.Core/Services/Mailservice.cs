
using RepositoryWithUOW.Core.Interfaces;

namespace RepositoryWithUOW.Core.Services;

public class Mailservice : IMailService
{
    public Task ConfirmEmailAsync(string ToEmail, string subject, string body)
    {
        throw new NotImplementedException();
    }

    public Task SendEmailAsync(string ToEmail, string subject, string body)
    {
        throw new NotImplementedException();
    }
}
