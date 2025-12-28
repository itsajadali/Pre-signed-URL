var builder = DistributedApplication.CreateBuilder(args);

var storageService = builder.AddProject<Projects.StorageService>("storage-service");

var appService = builder.AddProject<Projects.AppService>("app-service")
                        .WithHttpEndpoint(5001, name: "public")
                        .WithReference(storageService)
                        .WaitFor(storageService);

builder.Build().Run();
