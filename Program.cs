using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using EmailService.Services;
using EmailService.Data;
using Microsoft.EntityFrameworkCore;
using EmailService.Repositories;
using EmailProviders;
using EmailService.Services;

var builder = WebApplication.CreateBuilder(args);

// Load environment variables from sendgrid.env
DotNetEnv.Env.Load("/Users/pablopagliaricci/EmailService/sendgrid.env");

var jwtSecret = Environment.GetEnvironmentVariable("JWT_SECRET");
// Clave secreta para firmar el token
if (string.IsNullOrEmpty(jwtSecret))
{
    throw new InvalidOperationException("JWT_SECRET environment variable is not set.");
}
var key = Encoding.UTF8.GetBytes(jwtSecret);

// ✅ Ensure correct DbContext scope
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));


builder.Services.AddControllers();

builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<EmailsService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<StatsService>(); // Register StatsService

builder.Services.AddScoped<UserRepository>();

builder.Services.AddScoped<IEmailProvider, MailgunProvider>();
builder.Services.AddScoped<IEmailProvider, SendGridProvider>();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(key),
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        };
    });

builder.Services.AddAuthorization();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();