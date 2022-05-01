using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using Veveve.Api.Infrastructure.Database.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Veveve.Api.Infrastructure.Authorization;

public interface IJwtTokenHelper
{
    string GenerateJwtToken(UserEntity User);
    string GetEmail(JwtSecurityToken token);
}

public class JwtTokenHelper : IJwtTokenHelper
{
    private readonly ILogger<JwtTokenHelper> _logger;
    private readonly AuthorizationSettings _authSettings;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public JwtTokenHelper(
        ILogger<JwtTokenHelper> logger,
        IOptions<AuthorizationSettings> authSettings,
        IHttpContextAccessor httpContextAccessor)
    {
        this._logger = logger;
        this._authSettings = authSettings.Value;
        this._httpContextAccessor = httpContextAccessor;
    }

    public string GetEmail(JwtSecurityToken token) =>
        token.Claims.First(c => c.Type == CustomClaimTypes.Email).Value as string;

    public string GenerateJwtToken(UserEntity User)
    {
        _logger.LogInformation("Generating jwt token for User {0}", User.Id);

        var claims = new Dictionary<string, object>(){
            {CustomClaimTypes.Email, User.Email},
            {CustomClaimTypes.FullName, User.FullName}
        };
        if(User.HasAdminClaim())
            claims.Add(CustomClaimTypes.IsAdmin, true);

        var symmetricKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_authSettings.JwtKey));

        var tokenHandler = new JwtSecurityTokenHandler();
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Issuer = _authSettings.Issuer,
            Audience = _authSettings.Audience,
            Claims = claims,
            NotBefore = DateTime.Now,
            Expires = DateTime.Now.AddSeconds(_authSettings.ExpirationInSeconds),
            SigningCredentials = new SigningCredentials(symmetricKey, SecurityAlgorithms.HmacSha256)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }
}