// SmartLib API — Entry Point
// TODO: Konfiguracija servisa, middleware-a, autentifikacije i ruta

using Microsoft.EntityFrameworkCore;
using SmartLib.Infrastructure.Data;

var builder = WebApplication.CreateBuilder(args);

// --- Registracija servisa ---

// TODO: Dodati DbContext (PostgreSQL)
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseInMemoryDatabase("SmartLibDb"));

// TODO: Dodati JWT autentifikaciju
// builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
//     .AddJwtBearer(options => { ... });

// TODO: Registrovati repozitorije i servise
// builder.Services.AddScoped<IKorisnikRepository, KorisnikRepository>();
// builder.Services.AddScoped<IKnjigaRepository, KnjigaRepository>();

builder.Services.AddControllers();

// TODO: Dodati Swagger za API dokumentaciju
// builder.Services.AddEndpointsApiExplorer();
// builder.Services.AddSwaggerGen();

// TODO: Dodati CORS politiku za frontend
// builder.Services.AddCors(options => { ... });

var app = builder.Build();

// --- Middleware pipeline ---

// TODO: Swagger UI (samo za Development)
// if (app.Environment.IsDevelopment())
// {
//     app.UseSwagger();
//     app.UseSwaggerUI();
// }

// TODO: HTTPS redirekcija
// app.UseHttpsRedirection();

// TODO: CORS
// app.UseCors("AllowFrontend");

// TODO: Autentifikacija i autorizacija
// app.UseAuthentication();
// app.UseAuthorization();

app.MapControllers();

app.Run();
