# Tahap 1: Pembangunan Aplikasi (Build)
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Salin seluruh isi folder monorepo ke dalam container
COPY . .

# Jalankan restore dan publish khusus untuk proyek backend API Anda
RUN dotnet restore "HabitDuel.API/HabitDuel.API.csproj"
RUN dotnet publish "HabitDuel.API/HabitDuel.API.csproj" -c Release -o /app/publish

# Tahap 2: Menjalankan Aplikasi (Runtime)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

# Memaksa .NET mendengarkan port 8080 (Port bawaan Railway)
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

ENTRYPOINT ["dotnet", "HabitDuel.API.dll"]