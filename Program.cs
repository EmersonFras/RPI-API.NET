using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.NewtonsoftJson;
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

builder.Services.AddControllers().AddNewtonsoftJson();

var pfxPassword = Environment.GetEnvironmentVariable("PFX_PASSWORD");
builder.WebHost.ConfigureKestrel(serverOptions =>
{
    // Configure Kestrel to listen for HTTPS traffic on port 443
    serverOptions.ListenAnyIP(443, listenOptions =>
    {
        // Use the .pfx file and password to set up the HTTPS certificate
        listenOptions.UseHttps("/https-certs/aspnetapp.pfx", pfxPassword);
    });

    serverOptions.ListenAnyIP(80);
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowNetlify",
        policy =>
        {
            policy.WithOrigins("https://rpi-matrix.netlify.app")  // Allow requests from your Netlify domain
                  .AllowAnyMethod()
                  .AllowAnyHeader();
        });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowNetlify");

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
