namespace OrdersAPI.Middleware
{
    public class RequestLoggingMiddleware
    {
        private readonly RequestDelegate _next;

        public RequestLoggingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                Console.WriteLine($"Before request: {context.Request.Path}");
                await _next(context);
                Console.WriteLine($"After request: {context.Response.StatusCode}");
            }
            catch (Exception ex) 
            {
                Console.WriteLine($"Exception occurred: {ex.Message}");
                context.Response.StatusCode = 500; 

                await context.Response.WriteAsJsonAsync(
                    new { Message = "Something went wrong: " + ex.Message });
            }
            
        }
    }
}
