using System.Security.Cryptography;

namespace MyJCBApp.Helpers
{
    public interface ITokenManager
    {
        bool VerifyTokenSignature(string token, RSACryptoServiceProvider publicKey, bool verify = true);
       
    }
}