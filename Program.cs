using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using MilitaryServices.App.Services;
using MilitaryServices.App.Security;
using MilitaryServices.App.Entity;
using MilitaryServices.App.Middleware;

var builder = WebApplication.CreateBuilder(args);

// ➕ Swagger/OpenAPI
builder.Services.AddOpenApi();

// ➕ CORS for frontend (e.g. localhost:9090)
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowLocalhost9090", policy =>
    {
        policy.WithOrigins("http://localhost:9090")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

builder.Services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        var key = Encoding.UTF8.GetBytes("your-super-secret-key"); // Use a secure key from config/env
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(key)
        };
    });

builder.Services.AddScoped<JwtUtil>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IAuthorityService, AuthorityService>();
builder.Services.AddScoped<IMessageService, IMessageService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.UseCors("AllowLocalhost9090");

app.UseMiddleware<JwtAuthenticationMiddleware>();

app.UseAuthentication();
app.UseAuthorization();

app.Run();