using ProductCart.MAUI.ViewModels;
using System.Diagnostics;

namespace ProductCart.MAUI.Views;

public partial class ProductListPage : ContentPage
{
    private readonly ProductListViewModel _viewModel;

    public ProductListPage(ProductListViewModel viewModel)
    {
        Debug.WriteLine("=== ProductListPage CONSTRUCTOR ===");
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;

        Debug.WriteLine($"BindingContext set. Products count: {_viewModel.Products.Count}");
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        Debug.WriteLine($"=== OnAppearing === Products count: {_viewModel.Products.Count}");

        try
        {
            Debug.WriteLine("Calling LoadProductsCommand...");
            await _viewModel.LoadProductsCommand.ExecuteAsync(null);
            Debug.WriteLine($"After LoadProducts. Count: {_viewModel.Products.Count}");
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"ERROR in OnAppearing: {ex.Message}");
            Debug.WriteLine($"Stack: {ex.StackTrace}");
        }
    }
}