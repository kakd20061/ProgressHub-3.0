using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using MongoDB.Driver;
using ProgressHubApi;
using ProgressHubApi.Models;
using ProgressHubApi.Models.Mail;
using ProgressHubApi.Models.Token;
using ProgressHubApi.Providers;
using ProgressHubApi.Repositories;
using ProgressHubApi.Services;
using ProgressHubApi.Validators;

var builder = WebApplication.CreateBuilder(args);
// Add services to the container.
builder.Services.AddAuthentication(opt => {
    opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
    {
        if (Environment.GetEnvironmentVariable("JWTSECRETKEY") != null && Environment.GetEnvironmentVariable("JWTISSUER") != null && Environment.GetEnvironmentVariable("JWTAUDIENCE") != null)
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = Environment.GetEnvironmentVariable("JWTISSUER"),
                ValidAudience = Environment.GetEnvironmentVariable("JWTAUDIENCE"),
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Environment.GetEnvironmentVariable("JWTSECRETKEY")))
            };
        }
        else
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = builder.Configuration.GetSection("JwtSettings").GetValue<string>("Issuer"),
                ValidAudience = builder.Configuration.GetSection("JwtSettings").GetValue<string>("Audience"),
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration.GetSection("JwtSettings").GetValue<string>("SecretKey")))
            };
        }
    });

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton<IMongoClient, MongoClient>(x =>
{
    MongoClient client;

    if (Environment.GetEnvironmentVariable("CONNECTION") != null)
    {
        client = new MongoClient(Environment.GetEnvironmentVariable("CONNECTION"));
    }
    else
    {
        client = new MongoClient(builder.Configuration.GetSection("MongoSettings").GetValue<string>("ConnectionString"));
    }

    return client;
});

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(builder =>
    {
        builder.AllowAnyOrigin()
               .AllowAnyMethod()
               .AllowAnyHeader();
    });
});

builder.Services.Configure<MailSettingsModel>(builder.Configuration.GetSection("MailSettings"));
builder.Services.Configure<JwtSettingsModel>(builder.Configuration.GetSection("JwtSettings"));
builder.Services.Configure<GoogleSettingsModel>(builder.Configuration.GetSection("GoogleAuthSettings"));

builder.Services.AddTransient<IMailService, MailService>();
builder.Services.AddTransient<CommonService>();

builder.Services.AddTransient<IAuthenticationRepository, AuthenticationRepository>();
builder.Services.AddTransient<IAuthenticationService, AuthenticationService>();
builder.Services.AddTransient<IAuthenticationValidator, AuthenticationValidator>();

builder.Services.AddTransient<IAccountFeaturesRepository, AccountFeaturesRepository>();
builder.Services.AddTransient<IAccountFeaturesService, AccountFeaturesService>();
builder.Services.AddTransient<IAccountFeaturesValidator, AccountFeaturesValidator>();

builder.Services.AddTransient<ITokenRepository, TokenRepository>();
builder.Services.AddTransient<ITokenService, TokenService>();

builder.Services.AddTransient<IAccountSettingsRepository, AccountSettingsRepository>();
builder.Services.AddTransient<IAccountSettingsService, AccountSettingsService>();
builder.Services.AddTransient<IAccountSettingsValidator, AccountSettingsValidator>();
builder.Services.AddTransient<IAccountSettingsProvider, AccountSettingsProvider>();

builder.Services.AddTransient<ISeedRepository, SeedRepository>();
builder.Services.AddTransient<ISeedService, SeedService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI();
app.UseCors();

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();


