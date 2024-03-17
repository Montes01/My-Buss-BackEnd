using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using My_Buss_BackEnd.Helpers;
using My_Buss_BackEnd.Interfaces;
using My_Buss_BackEnd.Models;
using System.Text;
var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllers();

builder.Configuration.AddJsonFile("appsettings.json");

builder.Services.AddTransient<IMessage, SendEmail>();

var mailSettings = builder.Configuration.GetSection("GmailSettings");

builder.Services.Configure<GmailSettings>(mailSettings);

string SecretKey = builder.Configuration.GetSection("SecretKey").ToString()!;

byte[] KeyBytes = Encoding.UTF8.GetBytes(SecretKey);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = false,
        ValidateAudience = false,
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(KeyBytes),
    };
});

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(builder =>
    {
        builder.WithOrigins("*").AllowAnyHeader().AllowAnyMethod();
    });
});

var app = builder.Build();

app.UseCors();

app.MapControllers();

app.UseAuthentication();

app.UseAuthorization();

app.MapGet("/", () => Constants.HOME_MESSAGE);

app.Run();
