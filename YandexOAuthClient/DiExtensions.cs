using Microsoft.Extensions.DependencyInjection;

namespace YandexOAuthClient;

public static class DiExtensions
{
    extension(IServiceCollection services)
    {
        public IServiceCollection AddOAuthClient(string configSection = nameof(YandexAppParameters))
        {
            services.AddHttpClient<IOAuthClient, OAuthClient>(client => client.BaseAddress = new("https://oauth.yandex.ru"));

            services.AddOptions<YandexAppParameters>().BindConfiguration(configSection);

            services.AddScoped<IAuthService, AuthService>();

            return services;
        }

        public IServiceCollection StoreWith<TTokenStorage>() where TTokenStorage : class, ITokenStorage =>
            services.AddScoped<ITokenStorage, TTokenStorage>();

        public IServiceCollection StoreInMemory() => services.StoreWith<DefaultTokenStorage>();
    }
}
