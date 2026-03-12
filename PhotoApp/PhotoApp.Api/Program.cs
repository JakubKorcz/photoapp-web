using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using PhotoApp.Api;
using PhotoApp.Api.Repository;
using PhotoApp.Api.Service;
using Minio;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration["ConnectionStrings:DefaultConnection"]), ServiceLifetime.Scoped);

builder.Services.AddScoped<UserRepository, UserRepository>();
builder.Services.AddScoped<ProjectRepository, ProjectRepository>();

builder.Services.AddScoped<ProjectService, ProjectService>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()   
              .AllowAnyHeader()   
              .AllowAnyMethod();   
    });
});

var minioEndpoint = builder.Configuration["MinioSettings:Endpoint"];
var minioAccessKey = builder.Configuration["MinioSettings:AccessKey"];
var minioSecretKey = builder.Configuration["MinioSettings:SecretKey"];
var useSSL = builder.Configuration.GetValue<bool>("MinioSettings:UseSSL", false);

builder.Services.AddSingleton<IMinioClient>(sp =>
{
    var client = new MinioClient()
        .WithEndpoint(minioEndpoint)
        .WithCredentials(minioAccessKey, minioSecretKey);

    if (useSSL)
    {
        client = client.WithSSL();
    }

    return client.Build();
});

//AutoMapper
var configuration = new MapperConfiguration(config =>
{
    config.AddProfile(new MappingProfile());
}, new NullLoggerFactory());

var mapper = configuration.CreateMapper();
builder.Services.AddSingleton(mapper);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseCors("AllowAll");

app.UseHttpsRedirection();

app.UseAuthorization();

app.UseSwagger();
app.UseSwaggerUI();

app.MapControllers();

app.Run();
