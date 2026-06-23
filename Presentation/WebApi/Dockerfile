# 1. Aşama: Çalışma Zamanı (Runtime) İmajı
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

# 2. Aşama: Derleme (Build) İmajı
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# 🎯 katmanları tam yoluna göre ayarla
COPY ["Presentation/WebApi/WebApi.csproj", "Presentation/WebApi/"]
COPY ["Core/Application/Application.csproj", "Core/Application/"]
COPY ["Core/Domain/Domain.csproj", "Core/Domain/"]
COPY ["Infrastructure/Persistence/Persistence.csproj", "Infrastructure/Persistence/"]

# Bağımlılıkları restore et paktleri falan çek
RUN dotnet restore "Presentation/WebApi/WebApi.csproj"

# Kalan tüm kaynak kodları içeri al
COPY . .
WORKDIR "/src/Presentation/WebApi"
RUN dotnet build "WebApi.csproj" -c Release -o /app/build

# 3. Aşama: Yayınlama
FROM build AS publish
RUN dotnet publish "WebApi.csproj" -c Release -o /app/publish /p:UseAppHost=false

# 4. Aşama: Final (Hafif ve Güvenli Çalışma İmajı)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "WebApi.dll"]