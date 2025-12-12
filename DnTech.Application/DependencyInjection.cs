namespace DnTech.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            // Registrar validadores de FluentValidation
            services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

            // Registrar casos de uso
            services.AddScoped<RegisterUserUseCase>();
            services.AddScoped<LoginUserUseCase>();

            return services;
        }
    }
}
