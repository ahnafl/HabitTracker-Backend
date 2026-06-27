# Tahap 1: Pembangunan Aplikasi menggunakan SDK .NET 10.0 (Tahun 2026)
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Salin seluruh struktur Monorepo agar referensi antar-proyek tidak hilang
COPY . .

# Jalankan restore & publish khusus untuk API utama
RUN dotnet restore "HabitDuel.API/HabitDuel.API.csproj"
RUN dotnet publish "HabitDuel.API/HabitDuel.API.csproj" -c Release -o /app/publish

# Tahap 2: Lingkungan Runtime menggunakan ASP.NET 10.0
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

# Port default yang dibaca sistem Railway
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

ENTRYPOINT ["dotnet", "HabitDuel.API.dll"]