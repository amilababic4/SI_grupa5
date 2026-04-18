// SmartLib Web — ASP.NET Core MVC Entry Point
// TODO: Konfiguracija servisa, middleware-a, autentifikacije

var builder = WebApplication.CreateBuilder(args);

// --- Registracija servisa ---

// TODO: Dodati DbContext (PostgreSQL)
// builder.Services.AddDbContext<ApplicationDbContext>(options =>
//     options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// TODO: Registrovati repozitorije i servise (Dependency Injection)
// builder.Services.AddScoped<IKorisnikRepository, KorisnikRepository>();
// builder.Services.AddScoped<IKnjigaRepository, KnjigaRepository>();
// builder.Services.AddScoped<IKategorijaRepository, KategorijaRepository>();
// builder.Services.AddScoped<IPrimjerakRepository, PrimjerakRepository>();
// builder.Services.AddScoped<IZaduzenjeRepository, ZaduzenjeRepository>();
// builder.Services.AddScoped<IClanarinaRepository, ClanarinaRepository>();
// builder.Services.AddScoped<IRezervacijaRepository, RezervacijaRepository>();
// builder.Services.AddScoped<IAuditLogRepository, AuditLogRepository>();

// TODO: Konfiguracija autentifikacije (Cookie-based za MVC)
// builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
//     .AddCookie(options =>
//     {
//         options.LoginPath = "/Auth/Login";
//         options.AccessDeniedPath = "/Auth/AccessDenied";
//     });

builder.Services.AddControllersWithViews();

var app = builder.Build();

// --- Middleware pipeline ---

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

// TODO: Aktivirati autentifikaciju i autorizaciju
// app.UseAuthentication();
// app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
