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
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
    });

// 3. CORS Policy (Dioptimalkan agar Kebal Eror Preflight & Credentials)
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.SetIsOriginAllowed(origin => true) // Mengizinkan semua origin secara dinamis (Vercel, localhost, mobile)
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials(); // Mengizinkan pengiriman token/cookies jika diperlukan frontend
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

// --- HTTP REQUEST PIPELINE (URUTAN SUDAH SANGAT TEPAT) ---

// A. Exception Handler diletakkan paling atas untuk menangkap segala jenis eror di bawahnya
app.UseMiddleware<ExceptionMiddleware>();

// B. ROUTING WAJIB DI ATAS CORS
app.UseRouting();

// C. CORS TEPAT DI BAWAH ROUTING
app.UseCors("AllowAll");

// D. Swagger (Diaktifkan untuk semua environment sementara waktu agar Anda bisa tes endpoint live Railway Anda)
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "HabitDuel API v1");
    c.RoutePrefix = string.Empty; // Membuat Swagger muncul saat membuka URL utama Railway Anda
});

// E. DI-NONAKTIFKAN karena HTTPS Redirection di-handle otomatis oleh Edge Gateway Railway
// app.UseHttpsRedirection();

// F. Security (Autentikasi Token & Hak Akses User)
app.UseAuthentication();
app.UseAuthorization();

// G. Eksekusi Akhir ke Controller API Anda
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