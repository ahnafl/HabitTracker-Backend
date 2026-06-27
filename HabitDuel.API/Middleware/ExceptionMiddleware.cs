using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Http;

namespace HabitDuel.API.Middleware;

public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;
    public ExceptionMiddleware(RequestDelegate next) => _next = next;

    public async Task InvokeAsync(HttpContext context)
    {
        if (context.Request.Path.StartsWithSegments("/swagger"))
        {
            await _next(context);
            return;
        }

        try 
        { 
            await _next(context); 
        }
        catch (Exception ex)
        {
            // PERBAIKAN: Pastikan header CORS tetap terkirim saat server eror
            if (!context.Response.Headers.ContainsKey("Access-Control-Allow-Origin"))
            {
                context.Response.Headers.Append("Access-Control-Allow-Origin", "*");
                context.Response.Headers.Append("Access-Control-Allow-Methods", "GET, POST, PUT, DELETE, OPTIONS");
                context.Response.Headers.Append("Access-Control-Allow-Headers", "Content-Type, Authorization");
            }

            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            context.Response.ContentType = "application/json";
            
            var response = new { Message = "Terjadi kesalahan internal pada server.", Details = ex.Message };
            await context.Response.WriteAsync(JsonSerializer.Serialize(response));
        }
    }
}