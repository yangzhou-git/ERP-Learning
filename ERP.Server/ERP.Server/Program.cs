using System.Data;
using System.Text;
using ERP.DbMigrator;
using ERP.Server.Extensions;
using FluentMigrator.Runner;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Data.SqlClient;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration
    .GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("未配置数据库连接字符串 DefaultConnection");

var corsOrigins = builder.Configuration
    .GetSection("Cors:Origins")
    .Get<string[]>();
// Add services to the container.

var jwtKey = builder.Configuration["Jwt:Key"]
    ?? throw new InvalidOperationException("未配置 Jwt:Key");

var jwtIssuer = builder.Configuration["Jwt:Issuer"]
    ?? throw new InvalidOperationException("未配置 Jwt:Issuer");

var jwtAudience = builder.Configuration["Jwt:Audience"]
    ?? throw new InvalidOperationException("未配置 Jwt:Audience");

builder.Services.AddControllers();
builder.Services.AddScoped<IDbConnection>(_ =>
{
    return new SqlConnection(connectionString);
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("VueCors", policy =>
    {
        policy
            .WithOrigins(corsOrigins ?? [])
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

builder.Services
    .AddAuthentication(options =>
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

            ValidIssuer = jwtIssuer,
            ValidAudience = jwtAudience,
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwtKey))
        };
    });

builder.Services.AddAuthorization();

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddRepositories();
builder.Services.AddServices();
builder.Services.AddDbMigrator(connectionString);

var app = builder.Build();
using var scope = app.Services.CreateScope();
var runner = scope.ServiceProvider.GetRequiredService<IMigrationRunner>();
runner.MigrateUp();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseCors("VueCors");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
