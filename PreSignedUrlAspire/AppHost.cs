var builder = DistributedApplication.CreateBuilder(args);

var redis = builder.AddRedis("cache");


var storageService = builder.AddProject<Projects.StorageService>("storage-service")
                            .WithReference(redis)
                            .WaitFor(redis);

var appService = builder.AddProject<Projects.AppService>("app-service")
                        .WithHttpEndpoint(5001, name: "public")
                        .WithReference(storageService)
                        .WithReference(redis)
                        .WaitFor(storageService)
                        .WaitFor(redis);

builder.Build().Run();
