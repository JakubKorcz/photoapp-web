using Amazon.S3;
using Microsoft.EntityFrameworkCore;
using PhotoApp.Api;

var builder = WebApplication.CreateBuilder(args);


builder.Configuration.AddJsonFile("appsettings.Secret.json", optional: true, reloadOnChange: true);

// Add services to the container.
builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("db_connection_string")));

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()   
              .AllowAnyHeader()   
              .AllowAnyMethod();   
    });
});

builder.Services.AddSingleton<IAmazonS3>(sp =>
{
    var config = new AmazonS3Config
    {
        ServiceURL = "https://fsn1.your-objectstorage.com",
        ForcePathStyle = true 
    };

    return new AmazonS3Client(
        awsAccessKeyId: "ACCESS_KEY",
        awsSecretAccessKey: "SECRET_KEY",
        config
    );
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseCors("AllowAll");

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
