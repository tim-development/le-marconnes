using LeMarconnes.Data;
using LeMarconnes.Entities;
using LeMarconnes.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// ===== 1. Database =====
if (builder.Environment.EnvironmentName != "Testing")
{
    builder.Services.AddDbContext<ApplicationDbContext>(options =>
    {
        options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
               .ConfigureWarnings(w => w.Ignore(RelationalEventId.PendingModelChangesWarning));
    });
}

// ===== 2. Identity (Stateless voor API) =====
builder.Services.AddIdentity<User, IdentityRole>(options =>
{
    options.Password.RequireDigit = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequiredLength = 6;
    options.User.RequireUniqueEmail = true;
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();

// ===== 3. JWT Service & Authentication =====
builder.Services.AddScoped<IJwtService, JwtService>();

var jwtKey = builder.Configuration["Jwt:Key"] ?? throw new InvalidOperationException("JWT Key missing in appsettings.json");
var key = Encoding.UTF8.GetBytes(jwtKey);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.SaveToken = true;
    options.RequireHttpsMetadata = true; // Verplicht HTTPS voor JWT
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(key)
    };
});

// ===== 4. Controllers (Geconfigureerd tegen Object Cycles) =====
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        // Voorkomt de 'object cycle' error bij Include() - Belangrijk bij relaties zonder 'virtual'
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
        options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
    });

// ===== 5. CORS (Strikter voor Productie) =====
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        // In productie: vervang .AllowAnyOrigin() door je werkelijke frontend URL
        // bijv: policy.WithOrigins("https://www.lemarconnes.com")
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// --- SWAGGER SERVICES VERWIJDERD VOOR PRODUCTIE ---

var app = builder.Build();

// ===== 6. Middleware Pipeline (Productie configuratie) =====

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    // In productie tonen we geen technische errors
    app.UseExceptionHandler("/error");
    // HSTS dwingt beveiligde verbindingen af
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

// CORS moet NA UseRouting en VOOR UseAuthentication staan
app.UseCors();

app.UseAuthentication();
app.UseAuthorization();

// Neutrale root-endpoint (omdat Swagger weg is)
app.MapGet("/", () => "LeMarconnes API - Online");

app.MapControllers();

// ===== 7. Database Seeding & Initialization =====
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        await SeedData.Initialize(services);
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Fout tijdens het seeden van de LeMarconnes database.");
    }
}

app.Run();

public partial class Program { }