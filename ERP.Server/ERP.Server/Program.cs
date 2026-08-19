using System.Data;
using ERP.DbMigrator;
using ERP.Server.Extensions;
using FluentMigrator.Runner;
using Microsoft.Data.SqlClient;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration
    .GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("未配置数据库连接字符串 DefaultConnection");

var corsOrigins = builder.Configuration
    .GetSection("Cors:Origins")
    .Get<string[]>();
// Add services to the container.

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

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddRepositories();
builder.Services.AddServices();
builder.Services.AddJwtAuthentication(builder.Configuration);
builder.Services.AddRedis(builder.Configuration);
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
