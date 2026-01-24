using ProductCart.MAUI.ViewModels;

namespace ProductCart.MAUI.Views;

public partial class ProductListPage : ContentPage
{
    private readonly ProductListViewModel _viewModel;

    public ProductListPage(ProductListViewModel viewModel)
    {
        Console.WriteLine("=== ProductListPage CONSTRUCTOR ===");
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;

        Console.WriteLine($"BindingContext set. Products count: {_viewModel.Products.Count}");
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        Console.WriteLine($"=== OnAppearing === Products count: {_viewModel.Products.Count}");

        try
        {
            Console.WriteLine("Calling LoadProductsCommand...");
            await _viewModel.LoadProductsCommand.ExecuteAsync(null);
            Console.WriteLine($"After LoadProducts. Count: {_viewModel.Products.Count}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"ERROR in OnAppearing: {ex.Message}");
            Console.WriteLine($"Stack: {ex.StackTrace}");
        }
    }
}