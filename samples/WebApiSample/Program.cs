using ServiceMonitor.Client;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add ServiceMonitor Client
builder.Services.AddServiceMonitor(options =>
{
    // Required settings
    options.DashboardUrl = builder.Configuration["ServiceMonitor:DashboardUrl"] ?? "http://localhost:5192";
    options.ApiKey = builder.Configuration["ServiceMonitor:ApiKey"] ?? "sm_live_test_key";
    options.ServiceName = "web-api-sample";
    options.Environment = builder.Environment.EnvironmentName;
    options.Version = "1.0.0";

    // Optional: Enable metrics
    options.EnableMetrics = true;

    // Optional: Azure DevOps metadata (usually from environment variables)
    options.BuildId = Environment.GetEnvironmentVariable("BUILD_BUILDID");
    options.CommitHash = Environment.GetEnvironmentVariable("BUILD_SOURCEVERSION");
    options.Branch = Environment.GetEnvironmentVariable("BUILD_SOURCEBRANCHNAME");

    // Optional: Custom metadata
    options.DeploymentMetadata = new Dictionary<string, object>
    {
        { "sample", "WebApi" },
        { "framework", ".NET 8.0" }
    };
});

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();

// Sample endpoints
app.MapGet("/", () => new
{
    Service = "Web API Sample",
    Status = "Running",
    Timestamp = DateTime.UtcNow
})
.WithName("GetRoot")
.WithOpenApi();

app.MapGet("/health", () => Results.Ok(new
{
    Status = "Healthy",
    Timestamp = DateTime.UtcNow
}))
.WithName("HealthCheck")
.WithOpenApi();

app.MapControllers();

app.Run();
