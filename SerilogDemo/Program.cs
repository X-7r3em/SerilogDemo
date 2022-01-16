using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Sinks.Grafana.Loki;
using SerilogDemo.Controllers;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Logging.ClearProviders();
builder.Logging.SetMinimumLevel(LogLevel.Trace);
builder.Host.UseSerilog((_, config) =>
    {
        //Here you see that each message has the Request Id, which is unique for each request.
        //However, serilog creates a property for it. Being a property Grafana Loki automatically makes it a label
        //But the labels are high on cardinality, which creates a large amount of active streams. This leads to
        //Loki not being able to bear the load.
        //
        //What I propose is a way to keep labels (not filter them out) in the message, but remove them from
        //the loki labels. This way, my app can still have proper messages that contain high cardinality data,
        //still have the needed labels for grouping, and not kill loki's active streams.
        string template =
            "[{Timestamp:yyyy-MM-dd HH:mm:ss.fffffff}] {Level} {RequestId} {Message:lj}{NewLine}{Exception}";
        config.WriteTo.Console(outputTemplate: template);
        config.WriteTo.GrafanaLoki(
            uri: "Your loki",
            outputTemplate: template
            );
    }
);
builder.Services.AddSwaggerGen();
var app = builder.Build();
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<RequestIdMiddleware>();
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();