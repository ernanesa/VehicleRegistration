using System.Text;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Scalar.AspNetCore;
using VehicleRegistration.Domain.Interfaces;
using VehicleRegistration.Domain.Services;
using VehicleRegistration.Endpoints;
using VehicleRegistration.Infrastructure.Data;

public class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        _ = builder.Services.AddAuthentication(
            options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }
        ).AddJwtBearer(
            option =>
            {
                option.TokenValidationParameters = new()
                {
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
                };
            }
        );

        builder.Services.AddAuthorization();


        builder.Services.AddScoped<IAdministratorService, AdministratorService>();
        builder.Services.AddScoped<IVehicleService, VehicleService>();

        builder.Services.AddControllers().AddJsonOptions(options =>
            options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));

        // Add services to the container.
        builder.Services.AddDbContext<VehicleRegistrationContext>(options =>
            options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")
                              ?? "Data Source=VehicleRegistration.db"));
        builder.Services.AddOpenApi();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen(
            options =>
            {
                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    Description = "JWT Authorization header using the Bearer scheme. "
                                + "Enter 'Bearer' [space] and then your token in the text input below."
                                + "Example: 'Bearer 12345abcdef'",
                });
                options.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "Bearer"
                    }
                },
                Array.Empty<string>()
            }
                });

                options.SwaggerDoc("v1", new() { Title = "Vehicle Registration API", Version = "v1" });
            });

        var app = builder.Build();

        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
            app.UseSwagger();
            app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Vehicle Registration API v1"));
            app.MapScalarApiReference(
                options =>
                {
                    options
                        .WithTitle("Vehicle Registration API")
                        .WithDefaultHttpClient(ScalarTarget.CSharp, ScalarClient.HttpClient);
                }
            );
        }

        app.UseHttpsRedirection();

        app.MapAdministratorEndpoints();
        app.MapVehicleEndpoints();

        app.UseAuthentication();
        app.UseAuthorization();

        app.Run();
    }
}