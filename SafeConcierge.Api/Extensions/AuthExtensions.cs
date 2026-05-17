using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace SafeConcierge.Api.Extensions;

public static class AuthExtensions
{
    public static IServiceCollection AddJwtAuth(this IServiceCollection services, IConfiguration configuration)
    {
        var jwtSettings = configuration.GetSection("Jwt");

        services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtSettings["Issuer"],
                    ValidAudience = jwtSettings["Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(jwtSettings["Key"]!))
                };
            });

        services.AddAuthorization(options =>
        {
            options.AddPolicy("Admin",          p => p.RequireClaim("role", "ADMIN"));
            options.AddPolicy("Doorman",        p => p.RequireClaim("role", "DOORMAN"));
            options.AddPolicy("Resident",       p => p.RequireClaim("role", "RESIDENT"));
            options.AddPolicy("Manager",        p => p.RequireClaim("role", "MANAGER"));
            options.AddPolicy("AdminOrManager", p => p.RequireClaim("role", "ADMIN", "MANAGER"));
            options.AddPolicy("Staff",          p => p.RequireClaim("role", "ADMIN", "DOORMAN", "MANAGER"));
            options.AddPolicy("DoormanOrAdmin", p => p.RequireClaim("role", "ADMIN", "DOORMAN"));
        });

        return services;
    }
}

