using System;
using System.Net;
using System.Net.Mail;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;
using UnityEngine.UI;

public class InfoManager : MonoBehaviour
{
    public const ushort MAX_FEEDBACK_SESSION = 0;
    [SerializeField] private InputField feedbackField;
    [SerializeField] private InputField mailField;
    [SerializeField] private ToastScript toast;
    [SerializeField] private Text versionText;

    private ushort feedbackCount = 0;

    void Start()
    {
        versionText.text = "Version: " + Application.version;
    }

    public void SendFeedback()
    {
        if (feedbackCount > MAX_FEEDBACK_SESSION)
        {
            toast.ShowToast("Give us the time to process your feedback", null, 2f);
            return;
        }

        if (mailField.text.Equals("") || mailField.text.Equals("username@provider.domain"))
        {
            toast.ShowToast("Insert an email!", null, 1.5f);
            return;
        }

        if (feedbackField.text.Equals(""))
        {
            toast.ShowToast("Write something!", null, 2f);
            return;
        }

        if (feedbackField.text.Length < 15)
        {
            toast.ShowToast("Feedback !sent, thank you", null, 2f);
            feedbackCount++;
            return;
        }

        string mailText = mailField.text + "\n\n" + feedbackField.text;

        if (SmtpMail(mailText))
        {
            toast.ShowToast("Feedback sent, thank you!", null, 2f);
            feedbackCount++;
        }
        else
        {
            toast.ShowToast("Unable to send feedback try again later", null, 2f);
        }
    }

    private bool SmtpMail(string message)
    {
        try
        {
            MailMessage mail = new MailMessage();

            mail.From = new MailAddress("notaveragegames@gmail.com");
            mail.To.Add("notaveragegames@gmail.com");
            mail.Subject = "Feedback";
            mail.Body = message;

            SmtpClient smtpServer = new SmtpClient();
            smtpServer.Host = "smtp.gmail.com";
            smtpServer.Port = 587;
            smtpServer.Credentials = new System.Net.NetworkCredential("notaveragegames@gmail.com", "notaveragegamespassword") as ICredentialsByHost;
            smtpServer.EnableSsl = true;
            ServicePointManager.ServerCertificateValidationCallback = delegate (object s, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors) { return true; };
            smtpServer.Send(mail);
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    public void ClearMailField()
    {
        mailField.text = "";
    }

    public void SetMailFieldText(string text)
    {
        if (mailField.text == "")
        {
            mailField.text = text;
        }
    }
}
