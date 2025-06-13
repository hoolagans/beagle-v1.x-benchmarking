using System.Net;
using System.Net.Mail;

namespace BeagleLib.Util;

public static class Notifications
{
    #region Methods
    public static void SendSystemMessageSMTP(string to, string subject, string body, MailPriority priority = MailPriority.Normal, bool isHtml = false)
    {
        SendMessageSMTP(BConfig.SystemEmail, to, "", BConfig.SystemEmail, subject, body, priority, isHtml: isHtml);
    }
    public static void SendMessageSMTP(string from, string to, string cc, string bcc, string subject, string body, MailPriority priority, List<Attachment>? attachments = null, bool isHtml = false)
    {
        var client = new SmtpClient
        {
            Host = BConfig.SMTPServer,
            Port = BConfig.SMTPPort,
            EnableSsl = BConfig.EnableSsl,
            DeliveryMethod = BConfig.DeliveryMethod,
        };
        if (!string.IsNullOrEmpty(BConfig.SMTPUsername) && !string.IsNullOrEmpty(BConfig.SMTPPassword)) client.Credentials = new NetworkCredential(BConfig.SMTPUsername, BConfig.SMTPPassword);
        var msg = new MailMessage { Subject = subject, Priority = priority, Body = body, From = new MailAddress(from), IsBodyHtml = isHtml };

        AddEmailsToMailAddressCollection(msg.To, to);
        if (cc != "") AddEmailsToMailAddressCollection(msg.CC, cc);
        if (bcc != "") AddEmailsToMailAddressCollection(msg.Bcc, bcc);

        if (attachments != null)
        {
            foreach (var attachment in attachments) msg.Attachments.Add(attachment);
        }

        try
        {
            if (BConfig.EmailEnabled) client.Send(msg);
        }
        catch (Exception ex)
        {
            //print exception info and mask it
            Console.ForegroundColor = ConsoleColor.Yellow;
            Output.WriteLine($"\nWARNING! Unable to send email notification: {ex.Message}\n");
            Console.ResetColor();
        }
    }
    public static void AddEmailsToMailAddressCollection(MailAddressCollection emails, string strEmails)
    {
        if (string.IsNullOrEmpty(strEmails)) return;

        var strEmailsArr = strEmails.Split(";".ToCharArray());
        foreach (var strEmail in strEmailsArr)
        {
            try
            {
                emails.Add(strEmail.Trim());
            }
            // ReSharper disable once EmptyGeneralCatchClause
            catch (Exception) { }
        }
    }
    #endregion

    #region Constants
    public const string DefaultEmailSubject = "System Notification";
    #endregion
}