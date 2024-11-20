using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace OnlineStoreAPI.Middlewares
{
    public class AuthenticationMiddleware
    {
        private readonly RequestDelegate _next;
        private const string Token = "secret-token";

        public AuthenticationMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if (!context.Request.Headers.TryGetValue("X-Auth-Token", out var token))
            {
                Console.WriteLine("X-Auth-Token header is missing.");
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                await context.Response.WriteAsJsonAsync(new { error = "Unauthorized" });
                return;
            }

            Console.WriteLine($"Received token: {token}");

            if (token != Token)
            {
                Console.WriteLine("Invalid token.");
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                await context.Response.WriteAsJsonAsync(new { error = "Unauthorized" });
                return;
            }

            await _next(context);
        }
    }
}
