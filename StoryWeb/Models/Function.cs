using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Web;

namespace StoryWeb.Models
{
    public class Function
    {
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