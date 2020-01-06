using Microsoft.IdentityModel.Tokens;
using System.Security.Cryptography;
using System.Text;

namespace MyJCBApp.Helpers
{
    public class JsonWebTokenManager : ITokenManager
    {
        public bool VerifyTokenSignature(string token, RSACryptoServiceProvider publicKey, bool verify = true)
        {
            string[] parts = token.Split('.');
            string header = parts[0];
            string payload = parts[1];

            SHA256 sha256 = SHA256.Create();
            byte[] hash = sha256.ComputeHash(Encoding.UTF8.GetBytes(header + '.' + payload));

            var deformatter = new RSAPKCS1SignatureDeformatter(publicKey);
            deformatter.SetHashAlgorithm("SHA256");

            if (!deformatter.VerifySignature(hash, Base64UrlEncoder.DecodeBytes(parts[2])))
            {
                return false;
            }

            return true;
        }
    }
}