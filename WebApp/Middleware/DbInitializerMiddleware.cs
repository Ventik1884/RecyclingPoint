using WebApp.Data;

namespace WebApp.Middleware
{
    public class DbInitializerMiddleware
    {
        private readonly RequestDelegate _next;
        public DbInitializerMiddleware(RequestDelegate next)
        {
            _next = next;

        }
        public Task Invoke(HttpContext context, RecPointContext recPointContext)
        {
            if (!(context.Session.Keys.Contains("starting")))
            {
                DbUserInitializer.Initialize(context).Wait();
                DbInitializer.Initialize(recPointContext);
                context.Session.SetString("starting", "Yes");
            }

            return _next.Invoke(context);
        }
    }
    public static class DbInitializerExtensions
    {
        public static IApplicationBuilder UseDbInitializer(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<DbInitializerMiddleware>();
        }
    }
}
