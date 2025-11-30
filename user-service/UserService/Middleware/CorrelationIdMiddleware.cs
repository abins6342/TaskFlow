using Serilog.Context;

namespace UserService.Middleware
{
    public class CorrelationIdMiddleware
    {
        private const string HeaderName = "X-Correlation-ID";
        private readonly RequestDelegate _next;

        public CorrelationIdMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext httpContext)
        {
            string? correlationId = httpContext?.Request?.Headers[HeaderName].FirstOrDefault();

            if (string.IsNullOrWhiteSpace(correlationId)) 
            {
                correlationId = Guid.NewGuid().ToString();
            }

            httpContext.Response.Headers[HeaderName] = correlationId;
            httpContext.Items["CorrelationId"] = correlationId;

            using (LogContext.PushProperty("CorrelationId", correlationId))
            {
                await _next(httpContext);
            }
        }
    }
}
