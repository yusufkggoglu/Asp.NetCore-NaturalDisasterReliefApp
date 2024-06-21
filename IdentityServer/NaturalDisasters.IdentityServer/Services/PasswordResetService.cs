using System;
using System.Net.Mail;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using NaturalDisasters.IdentityServer.Models;
using Microsoft.EntityFrameworkCore;
using NaturalDisasters.IdentityServer.Credentials;

namespace NaturalDisasters.IdentityServer.Services
{
    public class PasswordResetService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private static readonly string CharSet = "0123456789";
        private static readonly Random random = new Random();

        public PasswordResetService(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public string GenerateRandomCode()
        {
            char[] code = new char[6];
            for (int i = 0; i < 6; i++)
            {
                code[i] = CharSet[random.Next(CharSet.Length)];
            }
            return new string(code);
        }

        public void SendEmail(string receiverEmail,string code)
        {

            string senderEmail =  EmailCredentials.Email;
            string senderPassword = EmailCredentials.Password;

            string smtpAddress = "smtp.gmail.com";
            int portNumber = 587;

            string subject = "Şifre Sıfırlama Kodu";
            string body = $"Şifre sıfırlama kodunuz: {code}";

            try
            {
                using (MailMessage mail = new MailMessage())
                {
                    mail.From = new MailAddress(senderEmail);
                    mail.To.Add(receiverEmail);
                    mail.Subject = subject;
                    mail.Body = body;
                    mail.IsBodyHtml = false;

                    using (SmtpClient smtp = new SmtpClient(smtpAddress, portNumber))
                    {
                        smtp.Credentials = new NetworkCredential(senderEmail, senderPassword);
                        
                        smtp.EnableSsl = true;
                        smtp.Send(mail);
                    }
                }
                Console.WriteLine("E-posta gönderildi.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"E-posta gönderirken hata oluştu: {ex.Message}");
            }
        }

        public async Task<bool> ResetPassword(string email)
        {
            var code = GenerateRandomCode();
            if (code.Length == 6)
            {
                var user = await _userManager.FindByEmailAsync(email);
                if (user != null)
                {
                    user.PasswordResetCode = code;
                    var result = await _userManager.UpdateAsync(user);
                    if (result.Succeeded)
                    {
                        SendEmail(email,code);
                        return true;
                    }
                }
            }
            return false;
        }
    }
}
