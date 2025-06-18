using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blog.SetServices.IServices
{
    public interface IEmailService
    {
        Task SendPasswordResetEmailAsync(string toEmail, string username, string token, string callbackUrl);
        Task SendWelcomeEmailAsync(string toEmail, string username);
        Task SendEmailAsync(string toEmail, string subject, string htmlContent);
    }
}