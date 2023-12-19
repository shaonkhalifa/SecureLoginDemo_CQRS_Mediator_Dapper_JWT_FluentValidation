namespace MadiatrProject.Middlewares
{
    public class ExecptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExecptionHandlingMiddleware> _logger;

        public ExecptionHandlingMiddleware(RequestDelegate next, ILogger<ExecptionHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvockeAsync(HttpContext context)
        {
            try
            {

            }
            catch (Exception exeption)
            {
             
                throw;
            }
        }
    }
}
