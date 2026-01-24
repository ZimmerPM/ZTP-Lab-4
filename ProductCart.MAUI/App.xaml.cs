using ProductCart.MAUI.Views;

namespace ProductCart.MAUI;

public partial class App : Application
{
    public App(IServiceProvider serviceProvider)
    {
        InitializeComponent();

        var productListPage = serviceProvider.GetRequiredService<ProductListPage>();
        MainPage = new NavigationPage(productListPage);
    }
}