using StorageService.Exceptions.cs;
using StorageService.Services;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddProblemDetails(configure =>
{
    configure.CustomizeProblemDetails = context =>
    {
        context.ProblemDetails.Extensions["traceId"] = context.HttpContext.TraceIdentifier;
    };
});

builder.Services.AddExceptionHandler<GlobalErrorExceptionHandler>();

builder.AddRedisDistributedCache("cache");


builder.Services.AddControllers();
builder.Services.AddOpenApi();


builder.Services.AddScoped<IFileStorageService, FileStorageService>();


var app = builder.Build();


if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}


app.UseExceptionHandler();
app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
