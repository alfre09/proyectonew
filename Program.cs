using Microsoft.EntityFrameworkCore;
using proyectonew.Application.Interfaces;
using proyectonew.Application.Services;
using proyectonew.Data;
using proyectonew.IOC.Dependencies;

var builder = WebApplication.CreateBuilder(args);

// Agregar servicios Razor Pages
builder.Services.AddRazorPages();

// Conexión a la base de datos
builder.Services.AddDbContext<SivDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("SivDbConnection")));

// Registrar la Business Logic (Inyección de Dependencias)
builder.Services.AddVueloDependencies();

var app = builder.Build();

// Configurar el pipeline HTTP
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();

app.Run();