using Notification.Handlers;

namespace Notification.Services
{
    public static class HandlerService
    {
        public static IServiceCollection AddHandlerService(this IServiceCollection services)
        {
            services.AddScoped<INotificationHandler, Handlers.NotificationHandler>();

            return services;
        }
    }
}
