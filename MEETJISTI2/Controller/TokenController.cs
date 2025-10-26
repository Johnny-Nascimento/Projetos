using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Cryptography;
using System.Web.Http;
using Microsoft.IdentityModel.Tokens;
using MEETJITSI.Helper;

namespace MEETJITSI.Controllers
{
    [RoutePrefix("api/jitsi")]
    public class TokenController : ApiController
    {
        private static readonly string PrivateKeyPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Keys", "private.pem");
        private const string JITSI_DOMAIN = "8x8.vc";
        private const string KID = "vpaas-magic-cookie-33b51968168c448e998a145b085b4cc4/4278b0"; // VPASS KEY gerada

        [HttpPost]
        [Route("link")]
        public IHttpActionResult GenerateLink([FromBody] TokenRequest request)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(request.Room))
                    return BadRequest("Room é obrigatório.");

                // Carrega a chave privada via BouncyCastle -> RSACryptoServiceProvider
                var rsa = KeyHelper.LoadPrivateKey(PrivateKeyPath);
                var key = new RsaSecurityKey(rsa) { KeyId = KID };
                var credentials = new SigningCredentials(key, SecurityAlgorithms.RsaSha256);

                // Cabeçalho com kid
                var header = new JwtHeader(credentials);
                header["alg"] = "RS256";
                header["kid"] = KID;
                header["typ"] = "JWT";

                // Payload Jitsi (ajuste conforme necessário)
                var payload = new JwtPayload
                {
                    { "aud", "jitsi" },
                    { "context", new Dictionary<string, object>
                        {
                            { "user", new Dictionary<string, object>
                                {
                                    { "id", 1 },
                                    { "name", request.UserName ?? "Guest" },
                                    { "email", request.Email ?? "" },
                                    { "moderator", request.IsModerator }
                                }
                            }
                        }
                    },
                    { "features", new Dictionary<string, object>    {
                            { "livestreaming", true },
                            { "recording", true }
                        }
                    },
                    { "exp", DateTimeOffset.UtcNow.AddMinutes(30).ToUnixTimeSeconds() },
                    { "iss", "chat" },
                    { "nbf", DateTimeOffset.UtcNow.AddMinutes(-5).ToUnixTimeSeconds() },
                    { "room", request.Room },
                    { "sub", "vpaas-magic-cookie-33b51968168c448e998a145b085b4cc4" }, // APPID
                };

                var token = new JwtSecurityToken(header, payload);
                var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

                var url = $"https://{JITSI_DOMAIN}/vpaas-magic-cookie-33b51968168c448e998a145b085b4cc4/{request.Room}?jwt={tokenString}";

                return Ok(new { url, token = tokenString });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
    }

    public class TokenRequest
    {
        public string UserName { get; set; }
        public string Email { get; set; }
        public string Room { get; set; }
        public bool IsModerator { get; set; }
    }
}
