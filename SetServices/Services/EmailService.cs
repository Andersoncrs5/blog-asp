using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using Blog.SetServices.IServices;
using Blog.utils;

namespace Blog.SetServices.Services
{
    public class EmailService: IEmailService
    {
        private readonly IConfiguration _configuration;

        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task SendEmailAsync(string toEmail, string subject, string htmlContent)
        {
            string smtpHost = _configuration["SmtpSettings:Host"] ?? throw new InvalidOperationException();
            int smtpPort = int.Parse(_configuration["SmtpSettings:Port"] ?? throw new InvalidOperationException("SMTP Port not configured."));
            string smtpUser = _configuration["SmtpSettings:Username"] ?? throw new InvalidOperationException("SMTP Username not configured.");
            string smtpPass = _configuration["SmtpSettings:Password"] ?? throw new InvalidOperationException("SMTP Password not configured.");
            string fromEmail = _configuration["SmtpSettings:FromEmail"] ?? throw new InvalidOperationException("SMTP FromEmail not configured.");
            bool enableSsl = bool.Parse(_configuration["SmtpSettings:EnableSsl"] ?? "true");
            int timeOut = int.Parse(_configuration["SmtpSettings:Timeout"] ?? "20000");

            using (SmtpClient smtpClient = new SmtpClient(smtpHost, smtpPort))
            {
                smtpClient.Credentials = new NetworkCredential(smtpUser, smtpPass);
                smtpClient.EnableSsl = enableSsl;
                smtpClient.Timeout = timeOut;

                MailMessage mailMessage = new MailMessage
                {
                    From = new MailAddress(fromEmail, "Blog"),
                    Subject = subject,
                    Body = htmlContent,
                    IsBodyHtml = true
                };

                mailMessage.To.Add(toEmail);

                try
                {
                    await smtpClient.SendMailAsync(mailMessage);
                }
                catch (SmtpException ex)
                {
                    throw new ResponseException($"Failed to send email. Check SMTP settings. Error: {ex.Message}", 500);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Generic Error sending email to {toEmail}: {ex.Message}");
                    throw new ResponseException($"Failed to send email. Error: {ex.Message}", 500);
                }

            }
        }
    
        public async Task SendPasswordResetEmailAsync(string toEmail, string username, string token, string callbackUrl)
        {
            string subject = "Password Reset Request for Your Account";
            string htmlContent = GeneratePasswordResetHtmlEmail(username, token, callbackUrl);
            await SendEmailAsync(toEmail, subject, htmlContent);  
        }

        public async Task SendWelcomeEmailAsync(string toEmail, string username)
        {
            string subject = "Welcome to Our App!";
            string htmlContent = GenerateWelcomeHtmlEmail(username);
            await SendEmailAsync(toEmail, subject, htmlContent);
        }

        private string GeneratePasswordResetHtmlEmail(string username, string token, string callbackUrl)
        {
            string resetLink = $"{callbackUrl}?userId={Uri.EscapeDataString(username)}&token={Uri.EscapeDataString(token)}"; 

            return $@"
            <!DOCTYPE html>
            <html>
            <head>
                <meta charset=""utf-8"">
                <meta name=""viewport"" content=""width=device-width, initial-scale=1"">
                <title>Password Reset</title>
                <style>
                    body {{
                        font-family: 'Inter', sans-serif;
                        background-color: #f8f8f8; /* Light background */
                        margin: 0;
                        padding: 0;
                        color: #333; /* Dark text */
                    }}
                    .container {{
                        max-width: 600px;
                        margin: 40px auto;
                        background-color: #ffffff; /* White content background */
                        padding: 30px;
                        border-radius: 8px;
                        box-shadow: 0 4px 8px rgba(0, 0, 0, 0.1);
                        border: 1px solid #ddd; /* Light border */
                    }}
                    h1 {{
                        color: #000; /* Black heading */
                        text-align: center;
                        margin-bottom: 20px;
                        font-size: 28px;
                    }}
                    p {{
                        line-height: 1.6;
                        margin-bottom: 15px;
                        font-size: 16px;
                    }}
                    .button-container {{
                        text-align: center;
                        margin-top: 30px;
                    }}
                    .button {{
                        display: inline-block;
                        padding: 12px 25px;
                        background-color: #000; /* Black button */
                        color: #fff !important; /* White text for button */
                        text-decoration: none;
                        border-radius: 5px;
                        font-weight: bold;
                        font-size: 18px;
                        transition: background-color 0.3s ease;
                    }}
                    .button:hover {{
                        background-color: #333; /* Darker black on hover */
                    }}
                    .footer {{
                        text-align: center;
                        margin-top: 30px;
                        font-size: 14px;
                        color: #666; /* Medium gray */
                        border-top: 1px solid #eee; /* Light gray separator */
                        padding-top: 20px;
                    }}
                    .token-display {{
                        background-color: #eee;
                        padding: 10px;
                        border-radius: 5px;
                        font-family: 'Courier New', monospace;
                        font-size: 1.1em;
                        text-align: center;
                        margin-top: 20px;
                        word-break: break-all; /* Ensures long tokens wrap */
                    }}
                </style>
            </head>
            <body>
                <div class=""container"">
                    <h1>Password Reset Request</h1>
                    <p>Hello <strong>{username}</strong>,</p>
                    <p>We received a request to reset your password. If you did not make this request, please ignore this email.</p>
                    <p>To reset your password, please click the button below:</p>
                    <div class=""button-container"">
                        <a href=""{resetLink}"" class=""button"">Reset Your Password</a>
                    </div>
                    <p style=""text-align: center; margin-top: 20px;"">Alternatively, you can copy and paste the following token into your password reset form:</p>
                    <div class=""token-display"">
                        <strong>{token}</strong>
                    </div>
                    <p style=""font-size: 0.9em; color: #888; text-align: center; margin-top: 25px;"">This link/token will expire in 24 hours (or as configured).</p>
                    <div class=""footer"">
                        <p>&copy; {DateTime.Now.Year} Your App Name. All rights reserved.</p>
                    </div>
                </div>
            </body>
            </html>";
        }

        private string GenerateWelcomeHtmlEmail(string username)
        {
            return $@"
            <!DOCTYPE html>
            <html>
            <head>
                <meta charset=""utf-8"">
                <meta name=""viewport"" content=""width=device-width, initial-scale=1"">
                <title>Welcome!</title>
                <style>
                    body {{
                        font-family: 'Inter', sans-serif;
                        background-color: #f8f8f8;
                        margin: 0;
                        padding: 0;
                        color: #333;
                    }}
                    .container {{
                        max-width: 600px;
                        margin: 40px auto;
                        background-color: #ffffff;
                        padding: 30px;
                        border-radius: 8px;
                        box-shadow: 0 4px 8px rgba(0, 0, 0, 0.1);
                        border: 1px solid #ddd;
                    }}
                    h1 {{
                        color: #000;
                        text-align: center;
                        margin-bottom: 20px;
                        font-size: 28px;
                    }}
                    p {{
                        line-height: 1.6;
                        margin-bottom: 15px;
                        font-size: 16px;
                    }}
                    .button-container {{
                        text-align: center;
                        margin-top: 30px;
                    }}
                    .button {{
                        display: inline-block;
                        padding: 12px 25px;
                        background-color: #000;
                        color: #fff !important;
                        text-decoration: none;
                        border-radius: 5px;
                        font-weight: bold;
                        font-size: 18px;
                        transition: background-color 0.3s ease;
                    }}
                    .button:hover {{
                        background-color: #333;
                    }}
                    .footer {{
                        text-align: center;
                        margin-top: 30px;
                        font-size: 14px;
                        color: #666;
                        border-top: 1px solid #eee;
                        padding-top: 20px;
                    }}
                </style>
            </head>
            <body>
                <div class=""container"">
                    <h1>Welcome, {username}!</h1>
                    <p>Thank you for joining our community. We are excited to have you on board!</p>
                    <p>You can now explore all the features we offer.</p>
                    <div class=""button-container"">
                        <a href=""#"" class=""button"">Go to Dashboard</a>
                    </div>
                    <div class=""footer"">
                        <p>&copy; {DateTime.Now.Year} Your App Name. All rights reserved.</p>
                    </div>
                </div>
            </body>
            </html>";
        }
    }
}