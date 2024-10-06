using Authentication.Handlers;

namespace Event.Services
{
    public static class HandlerService
    {
        public static IServiceCollection AddHandlerService(this IServiceCollection services)
        {
            services.AddScoped<IUserHandler, UserHandler>();

            return services;
        }
    }
}
