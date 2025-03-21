using RPI_API.Services;
using RPI_API.Utils;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddSingleton<MessageHandler>();

builder.Services.AddSingleton<Receiver>(sp =>
    new Receiver("update.*",
        sp.GetRequiredService<MessageHandler>().HandleWeatherMessage
    ));

builder.Services.AddHostedService<ReceiveUpdates>();

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
