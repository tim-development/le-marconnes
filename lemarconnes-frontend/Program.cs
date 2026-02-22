using System.Globalization;

var builder = WebApplication.CreateBuilder(args);

// --- 1. SERVICES CONFIGURATIE ---

// Ondersteuning voor Controllers met Views (MVC)
builder.Services.AddControllersWithViews();

// Sessie-ondersteuning voor het beheerpaneel
builder.Services.AddSession(options => {
    options.IdleTimeout = TimeSpan.FromMinutes(30); // Admin sessie vervalt na 30 min
    options.Cookie.HttpOnly = true;                // Veiligheid: Voorkomt Cross-Site Scripting (XSS)
    options.Cookie.IsEssential = true;             // Noodzakelijk voor de werking (GDPR-proof)
});

// Toegang tot HttpContext (nodig voor admin-checks in _Layout.cshtml)
builder.Services.AddHttpContextAccessor();

// HTTP Client voor API-communicatie
builder.Services.AddHttpClient();

// Swagger alleen configureren als de app in Development-modus draait
if (builder.Environment.IsDevelopment())
{
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();
}

// Globalisatie: dwing de app naar de punt-notatie (bijv. € 12.50) voor prijzen en datums
var cultureInfo = new CultureInfo("en-US");
CultureInfo.DefaultThreadCurrentCulture = cultureInfo;
CultureInfo.DefaultThreadCurrentUICulture = cultureInfo;

var app = builder.Build();

// --- 2. MIDDLEWARE PIPELINE (VOLGORDE IS CRUCIAAL) ---

if (app.Environment.IsDevelopment())
{
    // Uitgebreide foutmeldingen tijdens het bouwen
    app.UseDeveloperExceptionPage();

    // Swagger is alleen beschikbaar in de ontwikkelomgeving
    app.UseSwagger();
    app.UseSwaggerUI(c => {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "LeMarconnes v1");
    });
}
else
{
    // PRODUCTIE MODUS:
    // Stuur alle onverwachte fouten naar de Error-actie van de HomeController
    app.UseExceptionHandler("/Home/Error");

    // HSTS dwingt browsers af om de site alleen via HTTPS te bezoeken
    app.UseHsts();
}

// Forceer HTTPS verbindingen
app.UseHttpsRedirection();

// Toegang tot de wwwroot map (CSS, JS, Logo's, Afbeeldingen)
app.UseStaticFiles();

// Bepaal hoe verzoeken worden gerouteerd
app.UseRouting();

// Sessie-middleware (Moet ALTIJD na UseRouting en voor UseAuthorization)
app.UseSession();

// Beveiliging en rechten
app.UseCors();
app.UseAuthorization();

// --- 3. ROUTING ---

// Standaard route: Start bij Home/Index
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// Catch-all route: Alle onbekende URL's sturen de gebruiker terug naar de startpagina
// Dit is handig voor LeMarconnes om dode links te voorkomen
app.MapControllerRoute(
    name: "catch-all",
    pattern: "{*url}",
    defaults: new { controller = "Home", action = "Index" });

app.MapControllers();

app.Run();