# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /app

# Copy solution dan project files untuk restore
COPY *.slnx ./
COPY HabitDuel.API/*.csproj ./HabitDuel.API/
COPY HabitDuel.Application/*.csproj ./HabitDuel.Application/
COPY HabitDuel.Domain/*.csproj ./HabitDuel.Domain/
COPY HabitDuel.Infrastructure/*.csproj ./HabitDuel.Infrastructure/

# Restore dependencies
RUN dotnet restore HabitDuel.API/HabitDuel.API.csproj

# Copy seluruh source code
COPY . .

# Build dan Publish
RUN dotnet publish HabitDuel.API/HabitDuel.API.csproj -c Release -o /app/out

# Stage 2: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app
COPY --from=build /app/out .

# Ekspose port Railway
EXPOSE 3000
ENV ASPNETCORE_URLS=http://+:3000

# Jalankan aplikasi
ENTRYPOINT ["./HabitDuel.API"]
