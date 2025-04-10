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

builder.Services.AddDbContext<DisplayContext>(options =>
    options.UseSqlite(connectionString));


builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddControllers().AddNewtonsoftJson();

// Configure Kestrel to use a .pfx file for HTTPS
var pfxPassword = Environment.GetEnvironmentVariable("PFX_PASSWORD");
if (!string.IsNullOrEmpty(pfxPassword))
{
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
}

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigins", policy =>
    {
        policy.WithOrigins("https://rpi-matrix.netlify.app", "http://127.0.0.1:5173")
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


app.UseHttpsRedirection();

app.UseCors("AllowSpecificOrigins");

app.UseAuthorization();

app.MapControllers();

app.Run();
