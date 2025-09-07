using System.Security.Cryptography;
using System.Text;

namespace TaskTrackerClean.API.Security
{
    public class SecurityUtil
    {
        public static string Encryption(string message)
        {
            //const string publicKey = "";
            const string publicKey = "<RSAKeyValue><Exponent>AQAB</Exponent><Modulus>4RT4wJcW0Y0jOmy63SbZGA" +
                "Hqd0Yb7k54clnOCqtyqylPfYOSuQB5uux5+nLYw6ApgFowavlK7Y8rAmFQK2Kil8q8SNw/aVBqWoKj8G5PfsqmYEDG" +
                "bslwBVsYdMhkbiuWCTietI1630tPk9VN9YVOjHtSpVm/myNcKCcUavGdDiPkNtmsMIaaUhwyOdH/eTyoDLLviv3g+i" +
                "SwUncrdgfwU+YICKZJkQ8CEFB8KBNt6kfBkZAOyXBD+cV90hQ7s8O+sRson2bY3xsYnu69gRJqWtlZkyp+CZBM9q7p" +
                "+8QWwzFzouaQ1ptQHWRxR78RbnNii268udqraqDsh9AaXxUHCQ==</Modulus></RSAKeyValue>";

            var messageByte = Encoding.UTF8.GetBytes(message);

            using (var rsa = new RSACryptoServiceProvider(2048))
            {
                try
                {
                    // client encrypting data with public key issued by server                    
                    rsa.FromXmlString(publicKey.ToString());

                    var encryptedData = rsa.Encrypt(messageByte, RSAEncryptionPadding.Pkcs1);

                    var base64Encrypted = Convert.ToBase64String(encryptedData);

                    return base64Encrypted;
                }
                finally
                {
                    rsa.PersistKeyInCsp = false;
                }
            }
        }
    }
}
