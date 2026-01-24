using ProductCart.MAUI.Views;

namespace ProductCart.MAUI;

public partial class App : Application
{
    private readonly ProductListPage _productListPage;

    public App(ProductListPage productListPage)
    {
        InitializeComponent();
        _productListPage = productListPage;
    }

    protected override Window CreateWindow(IActivationState? activationState)
    {
        return new Window(new NavigationPage(_productListPage));
    }
}