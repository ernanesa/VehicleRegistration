using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using VehicleRegistration.Domain.DTOs;
using VehicleRegistration.Domain.Entities;
using VehicleRegistration.Domain.Interfaces;

namespace VehicleRegistration.Endpoints
{
    public static class AdministratorEndpoints
    {
        public static void MapAdministratorEndpoints(this WebApplication app)
        {
            app.MapPost("/login", ([FromBody] LoginDTO login, IAdministratorService administratorService) =>
            {
                var administrator = administratorService.Login(login);
                return administrator == null ?
                        Results.BadRequest("Invalid email or password") :
                        Results.Ok(new { token = GetTokenJWT(administrator, Encoding.ASCII.GetBytes(app.Configuration["Jwt:Key"])) });
            }).WithTags("Administrator").AllowAnonymous();

            app.MapPost("/register", ([FromBody] AdministratorDTO administrator, IAdministratorService administratorService) =>
            {
                administratorService.Register(administrator);
                return Results.Created("/login", "Administrator created");
            }).WithTags("Administrator").RequireAuthorization(new AuthorizeAttribute { Roles = "admin" });
        }

        private static object GetTokenJWT(Administrator administrator, byte[] key)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new("Email", administrator.Email),
                    new("Profile", administrator.Profile),
                    new(ClaimTypes.Role, administrator.Profile)
                }),
                Expires = DateTime.UtcNow.AddHours(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}