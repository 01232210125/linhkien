using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace TSQ.Models
{
    public class EncryptMD5
    {
        public static string MD5Hash(string text)
        {
            MD5 md5 = new MD5CryptoServiceProvider();
            // compute hash from the byte of text
            md5.ComputeHash(ASCIIEncoding.ASCII.GetBytes(text));
            //get hash result after compute it
            byte[] result = md5.Hash;
            StringBuilder strBulider = new StringBuilder();
            for (int i= 0; i< result.Length; i++)
            {
                //change it into 2 Hexadecimal digits
                //for each byte 
                strBulider.Append(result[i].ToString("x2"));
            }
            return strBulider.ToString();
        }
    }
}