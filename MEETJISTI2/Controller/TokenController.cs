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
        private const string APP_ID = "meu_app_id";              // ajustar
        private const string JITSI_DOMAIN = "localhost:8180"; // ou meet.seudominio.com
        private const string KID = "jitsi-key-1";                // deve bater com jwks

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
                header["kid"] = KID;

                // Payload Jitsi (ajuste conforme necessário)
                var payload = new JwtPayload
                {
                    { "aud", "jitsi" },
                    { "iss", APP_ID },
                    { "sub", JITSI_DOMAIN },
                    { "room", request.Room },
                    { "context", new Dictionary<string, object>
                        {
                            { "user", new Dictionary<string, object>
                                {
                                    { "name", request.UserName ?? "Guest" },
                                    { "email", request.Email ?? "" },
                                    { "moderator", request.IsModerator }
                                }
                            }
                        }
                    },
                    { "exp", DateTimeOffset.UtcNow.AddMinutes(30).ToUnixTimeSeconds() }
                };

                var token = new JwtSecurityToken(header, payload);
                var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

                var url = $"https://{JITSI_DOMAIN}/{request.Room}?jwt={tokenString}";

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
