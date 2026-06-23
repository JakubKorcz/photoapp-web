using System.IO;

var envPath = Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", ".env");
if (!File.Exists(envPath))
{
    envPath = Path.Combine(Directory.GetCurrentDirectory(), ".env");
}
if (File.Exists(envPath))
{
    foreach (var rawLine in File.ReadAllLines(envPath))
    {
        var line = rawLine.Trim();
        if (string.IsNullOrEmpty(line) || line.StartsWith("#")) continue;
        var idx = line.IndexOf('=');
        if (idx <= 0) continue;
        var key = line.Substring(0, idx).Trim();
        var value = line.Substring(idx + 1).Trim();
        Environment.SetEnvironmentVariable(key, value);
    }
}

var builder = DistributedApplication.CreateBuilder(args);

var pgPassword = builder.AddParameter("postgres-password", "postgres", secret: true);

var postgres = builder.AddPostgres("postgres", password: pgPassword)
    .WithHostPort(5432)
    .WithPgAdmin()
    .AddDatabase("photoapp");

var minio = builder.AddContainer("minio", "minio/minio")
    .WithArgs("server", "/data", "--console-address", ":9001")
    .WithEnvironment("MINIO_ROOT_USER", "minioadmin")
    .WithEnvironment("MINIO_ROOT_PASSWORD", "minioadmin")
    .WithVolume("minio-data", "/data")
    .WithHttpEndpoint(port: 9000, targetPort: 9000, name: "api")
    .WithHttpEndpoint(port: 9001, targetPort: 9001, name: "console");

var minioEndpoint = minio.GetEndpoint("api");

var emailUser = Environment.GetEnvironmentVariable("EMAIL_ADDRESS") ?? "";
var emailPassword = Environment.GetEnvironmentVariable("EMAIL_PASSWORD") ?? "";

var api = builder.AddProject<Projects.PhotoApp_Api>("photoapp-api")
    .WithReference(postgres)
    .WaitFor(postgres)
    .WithEnvironment("MinioSettings__Endpoint", minioEndpoint)
    .WithEnvironment("MinioSettings__AccessKey", "minioadmin")
    .WithEnvironment("MinioSettings__SecretKey", "minioadmin")
    .WithEnvironment("MinioSettings__UseSSL", "false")
    .WithEnvironment("AppSettings__Token", "PhotoAppSecretKeyForJWTTokensThatShouldBeAtLeast32Characters!")
    .WithEnvironment("AppSettings__Issuer", "PhotoApp.Api")
    .WithEnvironment("AppSettings__Audience", "PhotoApp.Client")
    .WithEnvironment("AppSettings__Email", emailUser)
    .WithEnvironment("AppSettings__EmailPassword", emailPassword);

builder.AddProject<Projects.PhotoApp_Front>("photoapp-front")
    .WithReference(postgres)
    .WithReference(api)
    .WaitFor(api)
    .WithExternalHttpEndpoints()
    .WithEnvironment("VITE_API_URL", "http://photoapp-api");

builder.Build().Run();
