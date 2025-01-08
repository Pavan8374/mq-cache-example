using Feed.API.MiddlleWares;
using Feed.API.Utility.Analytics;

namespace Feed.API.Extensions
{
    public static class UserTrackerExtensions
    {
        public static IServiceCollection AddUserTracker(this IServiceCollection services)
        {
            services.AddSingleton<UserTracker>();
            return services;
        }

        public static IApplicationBuilder UseUserTracker(this IApplicationBuilder app)
        {
            return app.UseMiddleware<UserTrackerMiddleware>();
        }
    }
}
