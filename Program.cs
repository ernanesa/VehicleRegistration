using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Scalar.AspNetCore;
using VehicleRegistration.Domain.DTOs;
using VehicleRegistration.Domain.Entities;
using VehicleRegistration.Domain.Interfaces;
using VehicleRegistration.Domain.Services;
using VehicleRegistration.Infrastructure.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAuthentication(
    options => {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    }
).AddJwtBearer(
    option => {
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
    c => {
        c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
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
        c.AddSecurityRequirement(new OpenApiSecurityRequirement
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
                new string[] {}
            }
        });
            
        c.SwaggerDoc("v1", new() { Title = "Vehicle Registration API", Version = "v1" });
    });

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Vehicle Registration API v1"));
    app.MapScalarApiReference(
        options => {
            options
                .WithTitle("Vehicle Registration API")
                .WithDefaultHttpClient(ScalarTarget.CSharp, ScalarClient.HttpClient);
        }
    );
}

app.UseHttpsRedirection();

string GetTokenJWT(Administrator administrator)
{
    var tokenHandler = new JwtSecurityTokenHandler();
    var key = Encoding.ASCII.GetBytes(app.Configuration["Jwt:Key"]);
    var tokenDescriptor = new SecurityTokenDescriptor
    {
        Subject = new ClaimsIdentity(new Claim[]
        {
            new Claim("Email", administrator.Email),
            new Claim("Profile", administrator.Profile),
            new Claim(ClaimTypes.Role, administrator.Profile)
        }),
        Expires = DateTime.UtcNow.AddHours(1),
        SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
    };
    var token = tokenHandler.CreateToken(tokenDescriptor);
    return tokenHandler.WriteToken(token);
}


app.MapPost("/login", ([FromBody] LoginDTO login, IAdministratorService administratorService) => 
{
    var administrator = administratorService.Login(login);
    return administrator == null ? Results.BadRequest("Invalid email or password") : Results.Ok(new { token = GetTokenJWT(administrator) });
}).WithTags("Administrator").AllowAnonymous();

app.MapPost("/register", ([FromBody] AdministratorDTO administrator, IAdministratorService administratorService) =>
{
    administratorService.Register(administrator);
    return Results.Created("/login", "Administrator created");
}).WithTags("Administrator").RequireAuthorization(new AuthorizeAttribute{Roles = "admin"});



app.MapGet("/vehicles", ([FromQuery] int? page, [FromQuery] int? pageSize, [FromQuery] string? name, [FromQuery] string? brand, [FromQuery] int? year, IVehicleService vehicleService) => 
    Results.Ok((object?)vehicleService.GetAll(page ??= 1, pageSize ??= 10, name, brand, year)))
    .WithTags("Vehicles").AllowAnonymous();

app.MapGet("/vehicles/{id}", ([FromRoute] int id, IVehicleService vehicleService) =>
    Results.Ok(vehicleService.GetById(id)))
    .WithTags("Vehicles").AllowAnonymous();

app.MapPost("/vehicles", ([FromBody] Vehicle vehicle, IVehicleService vehicleService) =>
    Results.Created($"/vehicles/{vehicleService.Create(vehicle).Id}", vehicle))
    .WithTags("Vehicles").RequireAuthorization();

app.MapPut("/vehicles/{id}", ([FromRoute] int id, [FromBody] Vehicle vehicle, IVehicleService vehicleService) 
    => id != vehicle.Id ? Results.BadRequest("Id mismatch") : Results.Ok((object?)vehicleService.Update(vehicle)))
    .WithTags("Vehicles").RequireAuthorization();

app.MapDelete("/vehicles/{id}", ([FromRoute] int id, IVehicleService vehicleService) =>
{
    vehicleService.Delete(id);
    return Results.NoContent();
}).WithTags("Vehicles").RequireAuthorization();




app.UseAuthentication();
app.UseAuthorization();

app.Run();
