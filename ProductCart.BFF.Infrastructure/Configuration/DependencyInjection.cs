using Microsoft.Extensions.DependencyInjection;
using ProductCart.BFF.Application.Services.Interfaces;
using ProductCart.BFF.Infrastructure.HttpClients;
using ProductCart.BFF.Infrastructure.Services;

namespace ProductCart.BFF.Infrastructure.Configuration;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        string productApiBaseUrl,
        string cartApiBaseUrl)
    {
        // Lab 1 (Products)
        services.AddHttpClient<ProductApiClient>(client =>
        {
            client.BaseAddress = new Uri(productApiBaseUrl);
            client.Timeout = TimeSpan.FromSeconds(30);
        });

        // Lab 3 (Cart)
        services.AddHttpClient<CartApiClient>(client =>
        {
            client.BaseAddress = new Uri(cartApiBaseUrl);
            client.Timeout = TimeSpan.FromSeconds(30);
        });

        services.AddScoped<IProductService, ProductService>();
        services.AddScoped<ICartService, CartService>();
        services.AddScoped<ICartAggregationService, CartAggregationService>();

        return services;
    }
}