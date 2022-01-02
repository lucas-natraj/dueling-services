using Lastly;
using Serilog;
using Microsoft.Extensions.Hosting.WindowsServices;

var logger = new LoggerConfiguration()
            .Enrich.FromLogContext()
            .WriteTo.Console()
            .WriteTo.Seq("http://localhost:5341")
            .Enrich.WithProcessId()
            .Enrich.WithProcessName()
            .CreateLogger();

var options = new WebApplicationOptions
{
    Args = args,
    ContentRootPath = WindowsServiceHelpers.IsWindowsService() ? AppContext.BaseDirectory : default
};


var builder = WebApplication.CreateBuilder(options);
builder.Host
    .UseWindowsService()
    .UseSerilog(logger);

builder.Services.AddSingleton<Serilog.ILogger>(logger);
builder.Services.AddSingleton<IPing, Ping>();
builder.Services.AddHostedService<Hosted>();

var app = builder.Build();
app.MapGet("/ping", (IPing pinger) => { return pinger.Ping(); });

logger.Information("---- program started!");
var monitor = new Lastly.Monitor("Lastly", logger);

try 
{ 
    app.Run(); 
}
catch (Exception ex) 
{
    logger.Fatal(ex, "Application start-up failed"); 
}

monitor.Dispose();
logger.Information("---- program stopped!");
logger.Dispose();
