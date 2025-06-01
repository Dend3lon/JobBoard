using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace JobBoard.Options
{
    public class IdentityService
    {
        private readonly JwtSettings? _settings;
        private readonly byte[] _key;

        public IdentityService(IOptions<JwtSettings?> jwtOptions)
        {
            _settings = jwtOptions.Value;
            ArgumentNullException.ThrowIfNull(_settings);
            ArgumentNullException.ThrowIfNull(_settings.SingingKey);
            ArgumentNullException.ThrowIfNull(_settings.Audience);
            ArgumentNullException.ThrowIfNull(_settings.Audience[0]);
            ArgumentNullException.ThrowIfNull(_settings.Issuer);
            _key = Encoding.ASCII.GetBytes(_settings?.SingingKey!);
        }

        private static JwtSecurityTokenHandler TokenHandler => new();

        public SecurityToken CreateSecurityToken(ClaimsIdentity identity)
        {
            var tokenDescriptor = GetTokenDescriptor(identity);

            return TokenHandler.CreateToken(tokenDescriptor);
        }

        public string WriteToken(SecurityToken token)
        {
            return TokenHandler.WriteToken(token);
        }

        private SecurityTokenDescriptor GetTokenDescriptor(ClaimsIdentity identity)
        {
            return new SecurityTokenDescriptor
            {
                Subject = identity,
                Expires = DateTime.Now.AddHours(2),
                Audience = _settings!.Audience?[0]!,
                Issuer = _settings?.Issuer,
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(_key),
                    SecurityAlgorithms.HmacSha256Signature)
            };
        }
    }
}
