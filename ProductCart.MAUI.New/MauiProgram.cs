using Microsoft.Extensions.Logging;
using ProductCart.MAUI.Services;
using ProductCart.MAUI.Services.Interfaces;
using ProductCart.MAUI.ViewModels;
using ProductCart.MAUI.Views;

namespace ProductCart.MAUI;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();

        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

#if DEBUG
        builder.Logging.AddDebug();
#endif

        // HttpClient & Services
        builder.Services.AddHttpClient<IProductService, ProductService>(client =>
        {
            client.BaseAddress = new Uri("http://localhost:5300/api/");
            client.Timeout = TimeSpan.FromSeconds(30);
        });

        builder.Services.AddSingleton<IProductService, ProductService>();

        // ViewModels
        builder.Services.AddTransient<ProductListViewModel>();
        builder.Services.AddTransient<ProductDetailsViewModel>();

        // Views
        builder.Services.AddTransient<ProductListPage>();
        builder.Services.AddTransient<ProductDetailsPage>();

        return builder.Build();
    }
}