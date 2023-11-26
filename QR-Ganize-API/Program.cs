using Microsoft.EntityFrameworkCore;
using QR_Ganize_API.Services;
using QR_Ganize_Lib;

var builder = WebApplication.CreateBuilder(args);

// Additional configuration is required to successfully run gRPC on macOS.
// For instructions on how to configure Kestrel and gRPC clients on macOS, visit https://go.microsoft.com/fwlink/?linkid=2099682

// Add services to the container.
builder.Services.AddGrpc();
builder.Services.AddDbContext<StorageDbContext>(optionsBuilder => optionsBuilder.UseSqlite("Data Source=storage.db"));
builder.Services.AddScoped<StorageManager>();



var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    var context = services.GetRequiredService<StorageDbContext>();
    context.Database.EnsureCreated();
    // DbInitializer.Initialize(context);
}

// Configure the HTTP request pipeline.
app.MapGrpcService<StorageService>();
app.MapGet("/",
    () =>
        "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

app.Run();