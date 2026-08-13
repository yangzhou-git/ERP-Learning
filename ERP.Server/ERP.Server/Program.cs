using System.Data;
using ERP.Server.Extensions;
using Microsoft.Data.SqlClient;

var builder = WebApplication.CreateBuilder(args);

var corsOrigins = builder.Configuration
    .GetSection("Cors:Origins")
    .Get<string[]>();
// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddScoped<IDbConnection>(_ =>
{
    return new SqlConnection(
        builder.Configuration.GetConnectionString("DefaultConnection")
    );
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

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseCors("VueCors");

app.UseAuthorization();

app.MapControllers();

app.Run();
