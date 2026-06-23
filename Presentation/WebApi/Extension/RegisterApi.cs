using WebApi.Middlewares;

namespace WebApi.Extension
{
    public static class RegisterApi
    {
        public static IServiceCollection AddApi(this IServiceCollection services)
        {
            // 1. Servis kayıt alanına ekle (builder.Build() satırından önce)
            services.AddExceptionHandler<GlobalExceptionHandler>();
            services.AddProblemDetails(); // .NET'in yerleşik hata detay desteği

            return services;            
        }
    }
}
