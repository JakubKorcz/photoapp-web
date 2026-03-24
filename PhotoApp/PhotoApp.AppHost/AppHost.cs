var builder = DistributedApplication.CreateBuilder(args);

var postgres = builder.AddPostgres("postgres")
    .WithPgAdmin()
    .AddDatabase("photoapp");

var dbConnectionString = postgres.AddConnectionStringer("DefaultConnection");

var minio = builder.AddMinIO("minio")
    .WithLogin("minioadmin", "minioadmin")
    .WithDataVolume("minio-data")
    .WithConsoleUI();

var minioEndpoint = minio.GetEndpoint("default");

builder.AddProject<Projects.PhotoApp_Api>("photoapp-api")
    .WithReference(postgres)
    .WaitFor(postgres)
    .WithReference(dbConnectionString)
    .WithReference(minio)
    .WaitFor(minio)
    .WithEnvironment("MinioSettings__Endpoint", minioEndpoint)
    .WithEnvironment("MinioSettings__AccessKey", minio.GetUsername())
    .WithEnvironment("MinioSettings__SecretKey", minio.GetPassword())
    .WithEnvironment("MinioSettings__UseSSL", "false")
    .WithEnvironment("AppSettings__Token", "PhotoAppSecretKeyForJWTTokensThatShouldBeAtLeast32Characters!")
    .WithEnvironment("AppSettings__Issuer", "PhotoApp.Api")
    .WithEnvironment("AppSettings__Audience", "PhotoApp.Client");

var apiEndpoint = builder.GetHttpEndpoint("photoapp-api");

builder.AddProject<Projects.PhotoApp_Front>("photoapp-front")
    .WithReference(minio)
    .WithReference(postgres)
    .WithExternalHttpEndpoints()
    .WithEnvironment("VITE_API_URL", apiEndpoint);

builder.Build().Run();
