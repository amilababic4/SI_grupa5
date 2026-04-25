// SmartLib API — Entry Point
// TODO: Konfiguracija servisa, middleware-a, autentifikacije i ruta

using SmartLib.Core.Interfaces;
using SmartLib.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

// --- Registracija servisa ---

// TODO: Dodati JWT autentifikaciju
// builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
//     .AddJwtBearer(options => { ... });

builder.Services.AddScoped<IKorisnikRepository, KorisnikRepository>();

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
