using BlogLab.Models.Account;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;

namespace BlogLab.Services
{
    public class TokenService : ITokenService
    {

        private readonly SymmetricSecurityKey _key;
        private readonly string _issuer;
        public TokenService(IConfiguration config)
        {
            _key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["Jwt:Key"]));
            _issuer = config["Jwt:Issuer"];
        }
        public string CreateToken(ApplicationUserIdentity user)
        {
            var claimms = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.NameId, user.ApplicationUserId.ToString()),
                new Claim(JwtRegisteredClaimNames.UniqueName, user.Username)
            };

            var cred = new SigningCredentials(_key, SecurityAlgorithms.HmacSha512Signature);
        }
    }
}
