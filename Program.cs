using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.NewtonsoftJson;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using dotenv.net;
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
DotEnv.Load();

var jwtKey = Environment.GetEnvironmentVariable("JWT_KEY");  

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options => 
{
    options.Events = new JwtBearerEvents
    {
        OnMessageReceived = context =>
        {
            var accessToken = context.Request.Cookies["jwt_token"];
            if (!string.IsNullOrEmpty(accessToken))
            {
                context.Token = accessToken;
            }
            return Task.CompletedTask;
        }
    };

    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = "https://rpi-display.duckdns.org:3000",
        ValidAudience = "https://rpi-matrix.netlify.app",
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
    };
});

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


// app.UseHttpsRedirection();

app.UseCors("AllowSpecificOrigins");

app.UseAuthentication();
app.UseAuthorization();


app.MapControllers();

app.Run();
