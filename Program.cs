using MusicDownloaderAPI.Services;

var builder = WebApplication.CreateBuilder(args);

// Agregar servicios al contenedor
builder.Services.AddControllers();
builder.Services.AddScoped<DownloaderService>();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// ✅ Habilitar CORS
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()   // Permite cualquier dominio (web, móvil, etc.)
              .AllowAnyHeader()   // Permite cualquier header
              .AllowAnyMethod()   // Permite GET, POST, OPTIONS, etc.
              .WithExposedHeaders("Content-Disposition");
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// 👇 Redirección HTTPS opcional, comentada si usas ngrok HTTP
// app.UseHttpsRedirection();

// ✅ Usar CORS
app.UseCors();

app.MapControllers();

app.Run();


