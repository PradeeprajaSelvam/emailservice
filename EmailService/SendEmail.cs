using System;
using System.Configuration;
using System.Net;
using System.Net.Mail;
using log4net;

namespace EmailService
{
    public static class SendEmail
    {
        private static readonly ILog _logger = LogManager.GetLogger(typeof(SendEmail));

        /// <summary>
        /// Sends a pre-composed HTML email using SMTP configuration from App.config.
        /// </summary>
        /// <remarks>
        /// This method reads SMTP credentials and settings from App.config for security.
        /// For Gmail: Use an App Password instead of your regular Gmail password.
        /// Generate an App Password at: https://myaccount.google.com/apppasswords
        ///
        /// To use:
        /// 1. Enable 2-Step Verification on your Gmail account
        /// 2. Generate a 16-character App Password
        /// 3. Add the App Password to App.config in the SmtpPassword setting
        /// 4. Ensure SmtpEnableSsl is set to true for Gmail
        /// </remarks>
        /// <exception cref="System.Net.Mail.SmtpException">If the SMTP server reports an error while sending.</exception>
        /// <exception cref="System.FormatException">If one of the email addresses is invalid.</exception>
        /// <exception cref="ConfigurationErrorsException">If required configuration settings are missing.</exception>
        public static void SendEmailAsync()
        {
            try
            {
                _logger.Info("Starting email send operation.");

                string smtpHost = ConfigurationManager.AppSettings["SmtpHost"] ?? "smtp.gmail.com";
                int smtpPort = int.Parse(ConfigurationManager.AppSettings["SmtpPort"] ?? "587");
                string smtpUsername = ConfigurationManager.AppSettings["SmtpUsername"];
                string smtpPassword = ConfigurationManager.AppSettings["SmtpPassword"];
                bool smtpEnableSsl = bool.Parse(ConfigurationManager.AppSettings["SmtpEnableSsl"] ?? "true");
                string fromEmail = ConfigurationManager.AppSettings["FromEmail"];
                string toEmail = ConfigurationManager.AppSettings["ToEmail"];

                if (string.IsNullOrEmpty(smtpUsername) || string.IsNullOrEmpty(smtpPassword))
                {
                    _logger.Error("SMTP credentials are not configured in App.config.");
                    return;
                }

                _logger.Debug($"Connecting to SMTP server: {smtpHost}:{smtpPort}");

                using (SmtpClient smtpClient = new SmtpClient(smtpHost))
                {
                    smtpClient.Port = smtpPort;
                    smtpClient.UseDefaultCredentials = false;
                    smtpClient.Credentials = new NetworkCredential(smtpUsername, smtpPassword);
                    smtpClient.EnableSsl = smtpEnableSsl;
                    smtpClient.Timeout = 30000; // 30 seconds timeout
                    smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;

                    using (MailMessage mailMessage = new MailMessage())
                    {
                        mailMessage.From = new MailAddress(fromEmail);
                        mailMessage.To.Add(toEmail);
                        mailMessage.Subject = "Immediate Action Required: System Activated";
                        mailMessage.Body = "<!DOCTYPE html>\r\n<html>\r\n\t<head>\r\n\t\t<title>Immediate Action Required: System Activated</title>\r\n\t</head>\r\n\t<body>\r\n\t\t<p>Hi Boss, \r\n \r\nThe system has been switched on. If this was not you, please take immediate action to address the situation. \r\n \r\nThank you, Chitti</p>\r\n\t</body>\r\n</html>";
                        mailMessage.IsBodyHtml = true;

                        smtpClient.Send(mailMessage);
                        _logger.Info($"Email sent successfully to {toEmail}.");
                    }
                }
            }
            catch (SmtpException smtpEx)
            {
                _logger.Error("SMTP server error occurred while sending email.", smtpEx);
            }
            catch (Exception exc)
            {
                _logger.Error("An unexpected error occurred while sending email.", exc);
            }
        }

    }
}
