using System;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Veveve.Domain.Models.Options;

namespace Veveve.Api.Authorization;

public static class AuthenticationExtensions
{
    /// <summary>
    /// Add JWT bearer scheme. 
    /// Using the [Authorize] attribute on controllers or controller-actions, will activate bearer authentication.
    /// </summary>
    public static IServiceCollection AddAuth(this IServiceCollection services, AuthorizationOptions settings)
    {
        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(cfg =>
        {
            cfg.RequireHttpsMetadata = false;
            cfg.SaveToken = true;
            cfg.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer = settings.Issuer,
                ValidateAudience = true,
                ValidAudience = settings.Audience,
                ValidateLifetime = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(settings.JwtKey)),
                ClockSkew = TimeSpan.Zero
            };
        });
        services.AddAuthorization(options =>
        {
            options.AddPolicy(AuthPolicies.Admin, policy =>
                policy.RequireAssertion(context =>
                    context.User.HasClaim(c =>
                        c.Type == CustomClaimTypes.IsAdmin)));
        });
        return services;
    }
}