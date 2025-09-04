using System;
using System.Security.Cryptography;
using System.Text;

namespace WinFormsApp2
{
    public static class PasswordHasher
    {
        public static string HashPassword(string password)
        {
            using SHA256 sha = SHA256.Create();
            byte[] bytes = Encoding.UTF8.GetBytes(password);
            byte[] hash = sha.ComputeHash(bytes);
            return Convert.ToBase64String(hash);
        }
    }
}
