using Microsoft.EntityFrameworkCore;
using proyectonew.Data;
using proyectonew.IOC.Dependencies;

var builder = WebApplication.CreateBuilder(args);

// Controladores (API REST) y Razor Pages
builder.Services.AddControllers();
builder.Services.AddRazorPages();

// Base de datos (SQL Server)
builder.Services.AddDbContext<SivDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Inyección de dependencias de la capa de aplicación (servicios del SIV)
builder.Services.AddVueloDependencies();

// Swagger / OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Aplica automáticamente las migraciones pendientes y siembra el catálogo base
// (estados de vuelo, aerolíneas y aeropuertos) para poder probar el sistema sin
// tener que insertar datos manualmente en la base de datos.
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<SivDbContext>();
    context.Database.Migrate();
    await SeedData.InicializarAsync(context);
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthorization();

app.MapControllers();
app.MapRazorPages();

app.Run();
