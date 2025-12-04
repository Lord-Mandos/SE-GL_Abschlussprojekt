using System.Security.Cryptography;

namespace Aufgaben_Managment_Tool
{
    internal class PasswordHelper
    {
        private const int _saltSize = 16;
        private const int _hashSize = 32;
        private const int _iterations = 10000;

        public static byte[] GenerateSalt(int size = _saltSize)
        {
            var saltBytes = new byte[size];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(saltBytes);
            }
            return saltBytes;
        }

        public static string HashPassword(string password, byte[] salt)
        {
            var hash = Rfc2898DeriveBytes.Pbkdf2(
                password,
                salt,
                _iterations,
                HashAlgorithmName.SHA256,
                _hashSize
            );
            return Convert.ToBase64String(hash);
        }

        public static bool VerifyPassword(string password, byte[] salt, string passwordhash)
        {
            var hashToVerify = HashPassword(password, salt);
            return hashToVerify == passwordhash;
        }
    }
}
