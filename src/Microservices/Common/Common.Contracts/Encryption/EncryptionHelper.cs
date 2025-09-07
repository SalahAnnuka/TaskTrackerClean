using Microsoft.Extensions.Configuration;
using System.Security.Cryptography;
using System.Text;

namespace Common.Contracts.Encryption
{
    public class EncryptionHelper
    {
        private readonly string _publicKey;
        private readonly string _privateKey;
        private readonly IConfiguration _config;

        public EncryptionHelper(IConfiguration config)
        {
            _config = config;

            _publicKey = _config["EncryptionSettings:PublicKey"]!;
            _privateKey = _config["EncryptionSettings:PrivateKey"]!;
        }

        public string Encrypt(string? plainText = "Failed to load message")
        {
            using (RSA rsa = RSA.Create())
            {
                rsa.ImportFromPem(_publicKey.ToCharArray());
                byte[] dataToEncrypt = Encoding.UTF8.GetBytes(plainText!);
                byte[] encryptedData = rsa.Encrypt(dataToEncrypt, RSAEncryptionPadding.Pkcs1);
                return Convert.ToBase64String(encryptedData);
            }
        }

        public string Decrypt(string encryptedText)
        {
            using (RSA rsa = RSA.Create())
            {
                rsa.ImportFromPem(_privateKey.ToCharArray());
                byte[] dataToDecrypt = Convert.FromBase64String(encryptedText);
                byte[] decryptedData = rsa.Decrypt(dataToDecrypt, RSAEncryptionPadding.Pkcs1);
                return Encoding.UTF8.GetString(decryptedData);
            }
        }
    }
}