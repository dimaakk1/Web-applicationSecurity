using BANK.DAL.Data;
using BANK.DAL.Entity;

namespace Web_applicationSecurity.Middleware
{
    public class AuditMiddleware
    {
        private readonly RequestDelegate _next;

        public AuditMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(
            HttpContext context,
            AppDbContext dbContext)
        {
            await _next(context);

            var log = new AuditLog
            {
                Username = context.User.Identity?.Name ?? "Anonymous",
                Method = context.Request.Method,
                Path = context.Request.Path,
                StatusCode = context.Response.StatusCode,
                CreatedAt = DateTime.UtcNow
            };

            dbContext.AuditLogs.Add(log);

            await dbContext.SaveChangesAsync();
        }
    }
}
