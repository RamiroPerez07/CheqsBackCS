using CheqsApp.Contexts;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Configura la cadena de conexión correctamente usando AddDbContext
builder.Services.AddDbContext<AppDBContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
);

// Configura CORS para permitir todas las solicitudes
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllOrigins", policy =>
    {
        policy.AllowAnyOrigin()   // Permite cualquier origen
              .AllowAnyHeader()   // Permite cualquier encabezado
              .AllowAnyMethod();  // Permite cualquier método (GET, POST, etc.)
    });
});

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Habilita CORS para todas las rutas
app.UseCors("AllowAllOrigins");

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
