using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

namespace StoryWeb.Models
{
    public class Function
    {
        private static readonly char[] tokenChars =
            "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789".ToCharArray();
        static Regex ConvertToUnsign_rg = null;
        private static readonly string[] VietNamChar = new string[]
    {
        "aAeEoOuUiIdDyY",
        "áàạảãâấầậẩẫăắằặẳẵ",
        "ÁÀẠẢÃÂẤẦẬẨẪĂẮẰẶẲẴ",
        "éèẹẻẽêếềệểễ",
        "ÉÈẸẺẼÊẾỀỆỂỄ",
        "óòọỏõôốồộổỗơớờợởỡ",
        "ÓÒỌỎÕÔỐỒỘỔỖƠỚỜỢỞỠ",
        "úùụủũưứừựửữ",
        "ÚÙỤỦŨƯỨỪỰỬỮ",
        "íìịỉĩ",
        "ÍÌỊỈĨ",
        "đ",
        "Đ",
        "ýỳỵỷỹ",
        "ÝỲỴỶỸ"
    };
        public static string ConvertToUnsign(string str)
        {
            for (int i = 1; i < VietNamChar.Length; i++)
            {
                for (int j = 0; j < VietNamChar[i].Length; j++)
                    str = str.Replace(VietNamChar[i][j], VietNamChar[0][i - 1]);
            }
            return str.Replace(" ","").ToLower();
        }
        public static string GenerateToken(int length = 20)
        {
            var random = new Random();
            var token = new StringBuilder();

            for (int i = 0; i < length; i++)
            {
                token.Append(tokenChars[random.Next(tokenChars.Length)]);
            }

            return token.ToString();
        }

        public static string MD5Hash(string text)
        {
            using (MD5 md5 = MD5.Create())
            {
                byte[] hash = md5.ComputeHash(Encoding.UTF8.GetBytes(text));
                StringBuilder hashsb = new StringBuilder();
                foreach (byte b in hash)
                {
                    hashsb.Append(b.ToString("X2"));
                }
                return hashsb.ToString();
            }
        }
        public static int limitnum(int num, int length)
        {
            int result = num;
            for (int i = 0; i < length; i++)
            {
                result += result * 10;
            }

            return result;
        }

        public static string GenerateCode(int length = 6)
        {
            var random = new Random();
            int a = 9;
            int max = limitnum(9, length);
            int min = limitnum(1, length);
            random.Next(min, max);


            return random.ToString();
        }
        public static string SendMail(string email)
        {
            string to = email;
            string code = GenerateCode(6);
            string from = "minhphat1612@gmail.com";
            MailMessage message = new MailMessage(from, to);
            message.Subject = "Verification Code";
            message.Body = @""+code;
            try
            {
                using (SmtpClient smtp = new SmtpClient())
                {
                    smtp.Host = "smtp.gmail.com";
                    smtp.EnableSsl = true;
                    smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                    NetworkCredential NetworkCred = new NetworkCredential(from, "msvm rjrs vuro bkfd");
                    smtp.Credentials = NetworkCred;
                    smtp.Port = 587;
                    smtp.Send(message);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception caught in CreateTestMessage2(): {0}",
                    ex.ToString());
            }
            return code;
        }
    }
}