using System.Security.Cryptography;
using System.Text;

namespace Propman.Services
{
    public static class MainFunction
    {
        public static string ComputeSha256Hash(string rawData)
        {
            // Create a SHA256 instance
            using (SHA256 sha256 = SHA256.Create())
            {
                // Convert the input string to bytes and compute the hash
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(rawData));

                // Convert byte array to hex string
                StringBuilder builder = new StringBuilder();
                foreach (byte b in bytes)
                    builder.Append(b.ToString("x2"));

                return builder.ToString();
            }
        }
    }
}
