using Event.Handlers;

namespace Event.Services
{
    public static class HandlerService
    {
        public static IServiceCollection AddHandlerService(this IServiceCollection services)
        {
            services.AddScoped<IEventHandler, Handlers.EventHandler>();
            
            return services;
        }
    }
}
