using ERP.Server.Common;

namespace ERP.Server.Middleware
{
    public class GlobalExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalExceptionMiddleware> _logger;

        public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ERP系统发生未处理异常，请求方法：{Method}，请求路径：{Path}，请求标识：{TraceId}", context.Request.Method, context.Request.Path, context.TraceIdentifier);
                if (context.Response.HasStarted)
                {
                    throw;
                }
                context.Response.Clear();
                context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                context.Response.ContentType = "application/json";
                var result = ApiResponse<object?>.Error("服务器内部错误，请稍后重试", 500);
                await context.Response.WriteAsJsonAsync(result);
            }
        }
    }
}
