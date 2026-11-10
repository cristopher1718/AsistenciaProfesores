using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace BusinessLogicLevel
{
    public class HashServicio
    {
        public static string ComputeHash(string input)
        {
            using var sha256 = SHA256.Create();
            byte[] inputBytes = Encoding.UTF8.GetBytes(input);
            byte[] hashBytes = sha256.ComputeHash(inputBytes);
            StringBuilder builder = new();

            for (int i = 0; i < hashBytes.Length; i++)
            {
                // Formato "x2" significa hexadecimal con al menos dos dígitos
                builder.Append(hashBytes[i].ToString("x2"));
            }

            return Convert.ToBase64String(hashBytes);
        }
    }
}