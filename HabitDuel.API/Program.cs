using HabitDuel.Application;
using HabitDuel.Application.Interfaces;
using HabitDuel.Application.Services;
using HabitDuel.Infrastructure;
using HabitDuel.API.Middleware;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// 1. Dependency Injection (Services)
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddApplication();
builder.Services.AddScoped<IJwtService, JwtService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IAuthService, AuthService>();

// 2. Serialisasi & Controllers
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new DateOnlyJsonConverter());
        options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
        // Penting: Mengatasi masalah siklus data pada Entity Framework
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
    });

// 3. CORS Policy
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// 4. Authentication
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"] ?? ""))
        };
    });

builder.Services.AddAuthorization();
builder.Services.AddEndpointsApiExplorer();

// 5. Swagger Config
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Masukkan token JWT di sini (contoh: Bearer <token Anda>)"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
            },
            Array.Empty<string>()
        }
    });
});

var app = builder.Build();

// --- HTTP REQUEST PIPELINE (PERBAIKAN DI SINI) ---

// A. CORS HARUS PALING ATAS
// Memastikan request preflight (OPTIONS) dan response error dari middleware di bawahnya tetap membawa header CORS
app.UseCors("AllowAll");

// B. Exception Handler 
app.UseMiddleware<ExceptionMiddleware>();

// C. Swagger
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// D. DI-NONAKTIFKAN / DI-KOMENTAR KARENA HTTPS REDIRECTION DI-HANDLE OTOMATIS OLEH GATEWAY RAILWAY
// app.UseHttpsRedirection();

// E. Routing & Security
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

// F. Map Controllers
app.MapControllers();

app.Run();

// --- Helper Class ---
public class DateOnlyJsonConverter : JsonConverter<DateOnly>
{
    private readonly string serializationFormat = "yyyy-MM-dd";
    public override DateOnly Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        => DateOnly.ParseExact(reader.GetString()!, serializationFormat, System.Globalization.CultureInfo.InvariantCulture);

    public override void Write(Utf8JsonWriter writer, DateOnly value, JsonSerializerOptions options)
        => writer.WriteStringValue(value.ToString(serializationFormat, System.Globalization.CultureInfo.InvariantCulture));
}