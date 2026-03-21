var builder = DistributedApplication.CreateBuilder(args);

builder.AddProject<Projects.PhotoApp_Front>("photoapp-front");

builder.Build().Run();
