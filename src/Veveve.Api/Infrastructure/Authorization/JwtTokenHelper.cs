using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Veveve.Domain.Database.Entities;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Net.Http.Headers;
using Veveve.Domain.Models.Options;

namespace Veveve.Domain.Authorization;

public interface IJwtTokenHelper
{
    string GenerateJwtToken(UserEntity User, ClientEntity? overrideClient = null);
    int? GetClientId();
    int? GetUserId();
    bool HasAdminClaim();
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

    private JwtSecurityToken? GetToken()
    {
        var authHeaderStr = _httpContextAccessor.HttpContext!.Request.Headers[HeaderNames.Authorization];
        if (authHeaderStr.Count == 0)
            return null;

        var jwtTokenStr = authHeaderStr.ToString().Replace("Bearer ", "");
        return new JwtSecurityToken(jwtTokenStr);
    }

    public int? GetUserId()
    {
        var accountId = GetToken()?.Claims?.FirstOrDefault(c => c.Type == CustomClaimTypes.UserId);
        if (accountId == null)
            return null;

        return int.Parse(accountId.Value);
    }

    public int? GetClientId()
    {
        var clientId = GetToken()?.Claims?.FirstOrDefault(c => c.Type == CustomClaimTypes.ClientId);
        if (clientId == null)
            return null;

        return int.Parse(clientId.Value);
    }


    public bool HasAdminClaim() => GetToken()?.Claims?.Any(c => c.Type == CustomClaimTypes.IsAdmin) == true;

    public string GenerateJwtToken(UserEntity User, ClientEntity? overrideClient = null)
    {
        _logger.LogInformation("Generating jwt token for User {0}", User.Id);

        var claims = new Dictionary<string, object>(){
            {CustomClaimTypes.Email, User.Email},
            {CustomClaimTypes.FullName, User.FullName},
            {CustomClaimTypes.UserId, User.Id},
            {CustomClaimTypes.ClientId, overrideClient?.Id ?? User.Client.Id},
            {CustomClaimTypes.ClientName, overrideClient?.Name ?? User.Client.Name},

        };
        if (User.HasAdminClaim())
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