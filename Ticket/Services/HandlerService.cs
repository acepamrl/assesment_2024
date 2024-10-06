using Ticket.Handlers;

namespace Ticket.Services
{
    public static class HandlerService
    {
        public static IServiceCollection AddHandlerService(this IServiceCollection services)
        {
            services.AddScoped<ITicketHandler, TicketHandler>();
            
            return services;
        }
    }
}
