var builder = DistributedApplication.CreateBuilder(args);

var postgres = builder.AddPostgres("postgres")
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

builder.AddProject<Projects.PhotoApp_Api>("photoapp-api")
    .WithReference(postgres)
    .WaitFor(postgres)
    .WithEnvironment("MinioSettings__Endpoint", minioEndpoint)
    .WithEnvironment("MinioSettings__AccessKey", "minioadmin")
    .WithEnvironment("MinioSettings__SecretKey", "minioadmin")
    .WithEnvironment("MinioSettings__UseSSL", "false")
    .WithEnvironment("AppSettings__Token", "PhotoAppSecretKeyForJWTTokensThatShouldBeAtLeast32Characters!")
    .WithEnvironment("AppSettings__Issuer", "PhotoApp.Api")
    .WithEnvironment("AppSettings__Audience", "PhotoApp.Client");

builder.AddProject<Projects.PhotoApp_Front>("photoapp-front")
    .WithReference(postgres)
    .WithExternalHttpEndpoints()
    .WithEnvironment("VITE_API_URL", "http://photoapp-api");

builder.Build().Run();
