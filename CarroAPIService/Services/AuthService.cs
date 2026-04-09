using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using CarrosLib.Helpers.Interfaces;
using System.Net;
using CarrosLib.DTOs;

namespace CarroAPIService.Services
{
    public class AuthService
    {
        private readonly ILoginHelper _loginHelper;
        private readonly IConfiguration _config;
        public AuthService(ILoginHelper loginHelper, IConfiguration config)
        {
            _loginHelper = loginHelper;
            _config = config;
        }
        public string GenerateToken(string username, string JWTKey)
        {
            try
            {
                var key = new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(JWTKey));

                var creds = new SigningCredentials(
                    key,
                    SecurityAlgorithms.HmacSha256);

                var claims = new[]
                {
            new Claim(ClaimTypes.Name, username)
        };

                var token = new JwtSecurityToken(
                    issuer: "CarrosDB",
                    audience: "CarrosDB",
                    claims: claims,
                    expires: DateTime.Now.AddHours(2),
                    signingCredentials: creds);

                return new JwtSecurityTokenHandler().WriteToken(token);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.StackTrace);
            }

            return null;
        }

        public HttpStatusCode Login(LoginDTO login)
        {
            if( _loginHelper.Login(login, _config["Settings:ActiveTag"]) != null)
            {
                return HttpStatusCode.OK;
            }
            return HttpStatusCode.BadRequest;
           
        }
    }
}

