using System;
using System.Net;
using System.Net.Mail;
using CommerceBuilder.Common;
using CommerceBuilder.Data;
using CommerceBuilder.Stores;
using CommerceBuilder.Utility;

namespace CommerceBuilder.Messaging
{
    /// <summary>
    /// Utility class for sending emails
    /// </summary>
    public static class EmailClient
    {
        /// <summary>
        /// Send the given email message
        /// </summary>
        /// <param name="mailMessage">The email message to send</param>
        public static void Send(MailMessage mailMessage)
        {
            Send(mailMessage, false);
        }

        /// <summary>
        /// Send the given email message using the given SMTP settings
        /// </summary>
        /// <param name="mailMessage">The email message to send</param>
        /// <param name="smtpSettings">The SMTP settings to use for sending the message</param>
        public static void Send(MailMessage mailMessage, SmtpSettings smtpSettings)
        {
            Send(mailMessage, smtpSettings, false);
        }

        /// <summary>
        /// Send the given email message
        /// </summary>
        /// <param name="mailMessage">The email message to send</param>
        /// <param name="throwOnError">Whether to throw the error or suppress it</param>
        public static void Send(MailMessage mailMessage, bool throwOnError)
        {
            Send(mailMessage, SmtpSettings.DefaultSettings, throwOnError);
        }

        /// <summary>
        /// Send the given email message using the given SMTP settings
        /// </summary>
        /// <param name="mailMessage">The email message to send</param>
        /// <param name="smtpSettings">The SMTP settings to use for sending the message</param>
        /// <param name="throwOnError">Whether to throw the error or suppress it</param>
        public static void Send(MailMessage mailMessage, SmtpSettings smtpSettings, bool throwOnError)
        {
            // IF SMPT NOT CONFIGURED, SKIP EMAIL SENDING
            if (String.IsNullOrEmpty(smtpSettings.Server)) return;
            try
            {              
                SmtpClient smtpClient = new SmtpClient(smtpSettings.Server, smtpSettings.Port);
                if (smtpSettings.RequiresAuthentication)
                {
                    smtpClient.Credentials = new System.Net.NetworkCredential(smtpSettings.UserName, smtpSettings.Password);
                }
                smtpClient.EnableSsl = smtpSettings.EnableSSL;

                //SEND THE MAIL MESSAGE
                smtpClient.Send(mailMessage);
            }
            catch (Exception ex)
            {
                Logger.Write("Error sending email with subject '" + mailMessage.Subject + "'.", Logger.LogMessageType.Warn, ex);
                if (throwOnError) throw;
            }
        }
    }
}
