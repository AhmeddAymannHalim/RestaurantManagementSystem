using System.Collections.Concurrent;

namespace ResturantManageSystem.API.Middleware
{
    public class RateLimitingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<RateLimitingMiddleware> _logger;
        private static readonly ConcurrentDictionary<string, (int Count, DateTime ResetTime)> _requestCounts = 
            new ConcurrentDictionary<string, (int, DateTime)>();

        private readonly int _maxRequests = 5;
        private readonly TimeSpan _windowDuration = TimeSpan.FromMinutes(1);
        private readonly string[] _protectedPaths = { "/api/auth/login", "/api/auth/register", "/api/auth/forgot-password" };

        public RateLimitingMiddleware(RequestDelegate next, ILogger<RateLimitingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var path = context.Request.Path.Value ?? string.Empty;

            if (_protectedPaths.Any(p => path.Equals(p, StringComparison.OrdinalIgnoreCase)))
            {
                var clientIp = context.Connection.RemoteIpAddress?.ToString() ?? "Unknown";
                var key = $"{clientIp}_{path}";

                var now = DateTime.UtcNow;

                if (_requestCounts.TryGetValue(key, out var data))
                {
                    if (now > data.ResetTime)
                    {
                        _requestCounts[key] = (1, now.Add(_windowDuration));
                    }
                    else if (data.Count >= _maxRequests)
                    {
                        _logger.LogWarning($"Rate limit exceeded for {clientIp} on {path}");
                        context.Response.StatusCode = StatusCodes.Status429TooManyRequests;
                        await context.Response.WriteAsJsonAsync(new { message = "Too many requests. Please try again later." });
                        return;
                    }
                    else
                    {
                        _requestCounts[key] = (data.Count + 1, data.ResetTime);
                    }
                }
                else
                {
                    _requestCounts[key] = (1, now.Add(_windowDuration));
                }
            }

            await _next(context);
        }
    }
}
