using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepositoryWithUOW.Core.Interfaces;

public interface IMailService
{
    public Task SendEmailAsync(string ToEmail, string subject, string body);

    public Task ConfirmEmailAsync(string ToEmail, string subject, string body);

}
