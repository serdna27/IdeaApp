using System;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;

namespace IdeaApp.Utils{

    public class GravatarUtil{

        public static string GetImageHash(string email){


            var client= new HttpClient();
            byte[] asciiBytes = ASCIIEncoding.ASCII.GetBytes(email);
            byte[] hashedBytes = MD5CryptoServiceProvider.Create().ComputeHash(asciiBytes);
            string hashedString = BitConverter.ToString(hashedBytes).Replace("-", "").ToLower();

            return hashedString;
      
        }

    }
}