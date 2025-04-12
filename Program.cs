using Microsoft.AspNetCore.Mvc.NewtonsoftJson;
using dotenv.net;
using RPI_API.Extensions;
using RPI_API.Utils;

DotEnv.Load();

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCustomServices(builder.Configuration);
builder.Services.AddCustomDbContext(builder.Configuration);
builder.Services.AddCustomAuthentication(builder.Configuration);
builder.Services.AddCustomCors();


builder.Services.AddControllers().AddNewtonsoftJson();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


app.UseHttpsRedirection();

app.UseCors("AllowSpecificOrigins");

app.UseAuthentication();
app.UseAuthorization();


app.MapControllers();

app.Run();
