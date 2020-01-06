using Microsoft.IdentityModel.Tokens;
using MyJCBApp.Helpers;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace MyJCBApp.Controllers
{
    public class ManageImportController : Controller
    {
        private readonly ITokenManager _tokenManager;

        public ManageImportController()
        {
            _tokenManager = new JsonWebTokenManager();
        }

        public ActionResult Index()
        {
            return View();
        }

        [HttpGet, Route("ManageImport/GetOrganisations/")]
        public async Task<JsonResult> GetOrganisations(string id, string token)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                throw new ArgumentException("id is not valid.");
            }

            if (string.IsNullOrWhiteSpace(token))
            {
                throw new ArgumentException("token is not valid.");
            }

            //get public key here and use that to verify the token
            var result = await GetPublicKey(token);

            if (result)
            {
                return new JsonResult
                {
                    JsonRequestBehavior = JsonRequestBehavior.AllowGet,
                    Data = GetClaims(token)
                };
            }

            return new JsonResult
            {
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        private string GetClaims(string token)
        {
            //urlDecode the token as it was url encoded in the response.
            var jwt = HttpContext.Server.UrlDecode(token);

            string[] parts = jwt.Split('.');
            string header = parts[0];
            string payload = parts[1];

            string headerJson = Encoding.UTF8.GetString(Base64UrlEncoder.DecodeBytes(header));
            JObject headerData = JObject.Parse(headerJson);

            string payloadJson = Encoding.UTF8.GetString(Base64UrlEncoder.DecodeBytes(payload));
            JObject payloadData = JObject.Parse(payloadJson);

            var organisationIds = payloadData.GetValue("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name").Children();
            var userEmailAddress = payloadData.GetValue("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress");

            var data = new StringBuilder();
            foreach (var organisationId in organisationIds)
            {
                data.Append("OrganisationId: " + organisationId + ", ");
            }

            data.Append("User Email: " + userEmailAddress);

            return data.ToString();
        }

        private async Task<bool> GetPublicKey(string token)
        {
            bool isSignatureValid = false;

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://localhost:54753");
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Add("User-Agent", "My JCB App");

                var url = "Certificate/GetKey";
                HttpResponseMessage response = await client.GetAsync(url);
                response.EnsureSuccessStatusCode();
                var result = await response.Content.ReadAsStringAsync();

                var cryptoServiceProvider = new RSACryptoServiceProvider();
                var deserializedData = JsonConvert.DeserializeObject(result);
                cryptoServiceProvider.FromXmlString(deserializedData.ToString());

                isSignatureValid = _tokenManager.VerifyTokenSignature(token, cryptoServiceProvider);
            }

            return isSignatureValid;
        }
    }
}
