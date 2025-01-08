using Feed.API.Utility.Analytics;

namespace Feed.API.MiddlleWares
{
    public class UserTrackerMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly UserTracker _userTracker;

        public UserTrackerMiddleware(RequestDelegate next, UserTracker userTracker)
        {
            _next = next;
            _userTracker = userTracker;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var userId = GetUserIdentifier(context);
            if (!string.IsNullOrEmpty(userId))
            {
                _userTracker.UserActivity(userId);
            }

            await _next(context);
        }

        private string GetUserIdentifier(HttpContext context)
        {
            // Prioritize authenticated user id
            if (context.User?.Identity?.IsAuthenticated == true)
            {
                return context.User.Identity.Name;
            }

            // Fall back to session ID for anonymous users
            if (context.Session.IsAvailable)
            {
                var sessionId = context.Session.Id;
                if (string.IsNullOrEmpty(sessionId))
                {
                    sessionId = Guid.NewGuid().ToString();
                    context.Session.SetString("SessionId", sessionId);
                }
                return sessionId;
            }

            return null;
        }
    }
}
