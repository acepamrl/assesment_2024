
using Payment.Handlers;

namespace Payment.Services
{
    public static class HandlerService
    {
        public static IServiceCollection AddHandlerService(this IServiceCollection services)
        {
            services.AddScoped<IPaymentHandler, PaymentHandler>();

            return services;
        }
    }
}
