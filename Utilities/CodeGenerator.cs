using System;
using System.Security.Cryptography;
using System.Text;

namespace SIMS_Akura.Utilities
{
    public static class CodeGenerator
    {
        private static readonly Random _rnd = new Random();

        
        public static string GenerateCode(string prefix)
        {
            long randNum = _rnd.Next(100000000, 999999999); // 9-digit random number
            return $"{prefix}-{randNum}";
        }

     
        public static string GenerateBarcode(string productName)
        {
            using (var sha = SHA256.Create())
            {
                var hash = sha.ComputeHash(Encoding.UTF8.GetBytes(productName + DateTime.Now.Ticks));
                return BitConverter.ToString(hash).Replace("-", "").Substring(0, 12).ToUpper();
            }
        }
    }
}
