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

            return services;
        }

        public IServiceCollection AddTokenStorage<TTokenStorage>() where TTokenStorage : class, ITokenStorage =>
            services.AddScoped<ITokenStorage, TTokenStorage>();
    }
}
