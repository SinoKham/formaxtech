namespace Formaxtech.WebAPI.Middlewares
{
    public class RequestLimitMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly int _parallelLimit;
        private static int _currentRequests = 0;
        private static readonly object _lock = new();

        public RequestLimitMiddleware(RequestDelegate next, IConfiguration config)
        {
            _next = next;
            _parallelLimit = config.GetValue<int>("Settings:ParallelLimit");
        }

        public async Task InvokeAsync(HttpContext context)
        {
            bool canProcess = false;

            lock (_lock)
            {
                if (_currentRequests < _parallelLimit)
                {
                    _currentRequests++;
                    canProcess = true;
                }
            }

            if (!canProcess)
            {
                context.Response.StatusCode = StatusCodes.Status503ServiceUnavailable;
                await context.Response.WriteAsync("Service Unavailable: Too many concurrent requests.");
                return;
            }

            try
            {
                await _next(context);
            }
            finally
            {
                lock (_lock)
                {
                    _currentRequests--;
                }
            }
        }
    }
}