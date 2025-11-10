# ---- ETAPA 1: Construir el proyecto C# ----
# Usamos la imagen de .NET 8 (puedes cambiarlo a 7.0 o 6.0 si lo necesitas)
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copia los archivos del proyecto y restaura dependencias
COPY *.csproj .
RUN dotnet restore

# Copia el resto del código fuente y publica la aplicación
COPY . .
RUN dotnet publish -c Release -o /app/publish

# ---- ETAPA 2: Crear la imagen final ----
# Usamos la imagen de runtime de ASP.NET, que es más ligera
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final

# --- Instalación de Dependencias (La parte Clave) ---
# Cambiamos a usuario 'root' para poder instalar software
USER root

# Actualizamos los repositorios e instalamos ffmpeg y curl
# (curl lo usaremos para bajar yt-dlp)
RUN apt-get update && \
    apt-get install -y ffmpeg curl && \
    apt-get clean

# Instalamos yt-dlp (la forma recomendada es bajar el binario)
RUN curl -L https://github.com/yt-dlp/yt-dlp/releases/latest/download/yt-dlp -o /usr/local/bin/yt-dlp
    
# Damos permisos de ejecución a yt-dlp
RUN chmod a+rx /usr/local/bin/yt-dlp

# Volvemos al usuario 'app' normal por seguridad
USER app
# ---------------------------------------------------

WORKDIR /app
COPY --from=build /app/publish .

# (¡IMPORTANTE!) Cambia "TuNombreDeAPI.dll" por el nombre real 
# de tu archivo .dll que se genera en la carpeta /bin/Release
ENTRYPOINT ["dotnet", "MusicDownloaderAPI.dll"]
