using System.Net;
using System.Net.Mail;

namespace PhotoApp.Api.Tools.Mailer
{
    public class Mailer(IConfiguration configuration)
    {
        public string FromAddress { get; set; } = configuration["app_email"]!;
        public string FromPassword { get; set; } = configuration["app_password"]!;
        public static string DisplayedName { get; set; } = "PhotoAppTestName";

        public void SendLoginMail(string toEmail, string toName, string code)
        {
            var fromAddress = new MailAddress(FromAddress, DisplayedName);
            var toAddress = new MailAddress( toEmail, toName); // Chwilowe do testów, potem będzie toEmail i toName z parametru


            string subject = "elo elo";
            string body = $"<!DOCTYPE html>\r\n<html lang=\"pl\">\r\n<head>\r\n    <meta charset=\"UTF-8\">\r\n    <title>Hasło do logowania</title>\r\n</head>\r\n<body style=\"font-family: Arial, sans-serif; background-color: #f9f9f9; padding: 20px;\">\r\n    <div style=\"max-width: 600px; margin: auto; background-color: #ffffff; border: 1px solid #dddddd; border-radius: 5px; padding: 20px;\">\r\n        <h2 style=\"color: #333333;\">Hasło do logowania</h2>\r\n        <p style=\"font-size: 16px; color: #555555;\">\r\n            Twoje hasło to:<br/>\r\n            <span style=\"font-weight: bold; font-size: 24px; color: #000000;\">\r\n                {code}\r\n            </span>\r\n        </p>\r\n        <p style=\"color: #888888; font-size: 14px;\">\r\n            Prosimy o zachowanie tego hasła w tajemnicy i nieudostępnianie go innym osobom.\r\n        </p>\r\n    </div>\r\n</body>\r\n</html>";

            var smtp = new SmtpClient
            {
                Host = "smtp.gmail.com", 
                Port = 587,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(fromAddress.Address, FromPassword)
            };

            using (var message = new MailMessage(fromAddress, toAddress)
            {
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            })
            {
                smtp.Send(message);
            }
        }

        public void SendRegisterMail(string toEmail, string toName, int code)
        {
            var fromAddress = new MailAddress(FromAddress, DisplayedName);
            var toAddress = new MailAddress(toEmail, toName);

            string subject = "Elo Elo to my";
            string body = $"<!DOCTYPE html>\r\n<html lang=\"pl\">\r\n<head>\r\n    <meta charset=\"UTF-8\">\r\n    <title>Hasło do logowania</title>\r\n</head>\r\n<body style=\"font-family: Arial, sans-serif; background-color: #f9f9f9; padding: 20px;\">\r\n    <div style=\"max-width: 600px; margin: auto; background-color: #ffffff; border: 1px solid #dddddd; border-radius: 5px; padding: 20px;\">\r\n        <h2 style=\"color: #333333;\">Hasło do logowania</h2>\r\n        <p style=\"font-size: 16px; color: #555555;\">\r\n            Twoje hasło to:<br/>\r\n            <span style=\"font-weight: bold; font-size: 24px; color: #000000;\">\r\n                {code}\r\n            </span>\r\n        </p>\r\n        <p style=\"color: #888888; font-size: 14px;\">\r\n            Prosimy o zachowanie tego hasła w tajemnicy i nieudostępnianie go innym osobom.\r\n        </p>\r\n    </div>\r\n</body>\r\n</html>";

            var smtp = new SmtpClient
            {
                Host = "smtp.gmail.com",
                Port = 587,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(fromAddress.Address, FromPassword)
            };

            using (var message = new MailMessage(fromAddress, toAddress)
            {
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            })
            {
                smtp.Send(message);
            }
        }

        public static bool IsValidEmail(string email)
        {
            try
            {
                var addr = new MailAddress(email);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
