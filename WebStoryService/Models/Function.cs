using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.EnterpriseServices;

namespace WebStoryService.Models
{
    public class Function
    {

        private static readonly char[] chars =
            "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789".ToCharArray();

        public static string GenerateToken(int length = 20)
        {
            var random = new Random();
            var token = new StringBuilder();

            for (int i = 0; i < length; i++)
            {
                token.Append(chars[random.Next(chars.Length)]);
            }

            return token.ToString();
        }

        public static string MD5Hash(string text)
        {
            MD5 mD5 = MD5.Create();
            byte[] hash = mD5.ComputeHash(Encoding.UTF8.GetBytes(text));
            StringBuilder hashsb = new StringBuilder();
            foreach (byte b in hash)
            {
                hashsb.Append(b.ToString("X2"));
            }
            return hashsb.ToString();
        }
        
    }
}