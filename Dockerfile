# Etapa 1: Build
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /app

# Copiar archivos del proyecto y restaurar dependencias
COPY *.csproj ./
RUN dotnet restore

# Copiar el resto del código
COPY . .

# Compilar la aplicación
RUN dotnet publish -c Release -o /app/out

# Etapa 2: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app

# Copiar la app compilada desde la etapa de build
COPY --from=build /app/out .

# Exponer el puerto
EXPOSE 5000

# Ejecutar la aplicación
CMD ["dotnet", "EmailService.dll"]
