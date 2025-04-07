using Microsoft.EntityFrameworkCore;
using RPI_API.Data;
using RPI_API.Services;
using RPI_API.Utils;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddSingleton<Emitter>();

builder.Services.AddScoped<MessageHandler>();

builder.Services.AddScoped<Receiver>(sp =>
    new Receiver("update.*",
        sp.GetRequiredService<MessageHandler>().HandleWeatherMessage
    ));

builder.Services.AddHostedService<ReceiveUpdates>();

builder.Services.AddHostedService<ReceiveUpdates>();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<WeatherDisplayContext>(options =>
    options.UseSqlite(connectionString));


builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
