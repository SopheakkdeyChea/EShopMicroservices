using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;

var builder = WebApplication.CreateBuilder(args);

// Add services to container
var assembly = typeof(Program).Assembly;

builder.Services.AddMediatR(opts =>
{
    opts.RegisterServicesFromAssembly(assembly);
    opts.AddOpenBehavior(typeof(ValidationBehavior<,>));
    opts.AddOpenBehavior(typeof(LoggingBehavior<,>)); 
});

builder.Services.AddValidatorsFromAssembly(assembly);
builder.Services.AddCarter();
builder.Services.AddMarten(opts =>
{
    opts.Connection(builder.Configuration.GetConnectionString("Database")!);
}).UseLightweightSessions();

//global exception from buildingblocks
builder.Services.AddExceptionHandler<CustomExceptionHandler>();

// health check for db
builder.Services.AddHealthChecks()
    .AddNpgSql(builder.Configuration.GetConnectionString("Database")!);

var app = builder.Build();

// Configure the HTTP request pipeline
app.MapCarter();

//global exception from buildingblocks
app.UseExceptionHandler(options => { });

// health check for API
app.UseHealthChecks("/health",
    new HealthCheckOptions
    {
        ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
    });

app.Run();
