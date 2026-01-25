using ProductCart.MAUI.ViewModels;

namespace ProductCart.MAUI.Views;

public partial class CartPage : ContentPage
{
    private readonly CartViewModel _viewModel;

    public CartPage(CartViewModel viewModel)
    {
        Console.WriteLine("=== CartPage CONSTRUCTOR ===");
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        Console.WriteLine("=== CartPage OnAppearing ===");
        await _viewModel.LoadCartCommand.ExecuteAsync(null);
    }
}
