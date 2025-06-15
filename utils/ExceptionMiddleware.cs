using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blog.utils
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionMiddleware> _logger;

        public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (ResponseException ex)
            {
                _logger.LogError(ex, "Error tratado");
                context.Response.StatusCode = ex.StatusCode;
                context.Response.ContentType = "application/json";

                var response = new
                {
                    status = "error",
                    code = ex.StatusCode,
                    message = ex.Message,
                    reason = new
                        {
                            statusCode = ex.StatusCode,
                            status = ex.Status,
                            message = ex.InnerException?.Message,
                            stackTrace = ex.InnerException?.StackTrace
                        }
                };

                await context.Response.WriteAsJsonAsync(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro não tratado");
                context.Response.StatusCode = 500;
                context.Response.ContentType = "application/json";

                var response = new
                {
                    status = "error",
                    code = 500,
                    message = "Error internal in servidor"
                };

                await context.Response.WriteAsJsonAsync(response);
            }
        }
    }

}