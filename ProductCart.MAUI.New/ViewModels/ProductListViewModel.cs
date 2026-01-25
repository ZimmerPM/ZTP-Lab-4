using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ProductCart.MAUI.Models;
using ProductCart.MAUI.Services.Interfaces;
using ProductCart.MAUI.Views;
using System.Collections.ObjectModel;

namespace ProductCart.MAUI.ViewModels;

public partial class ProductListViewModel : BaseViewModel
{
    private readonly IProductService _productService;

    public ObservableCollection<Product> Products { get; } = new();

    public ProductListViewModel(IProductService productService)
    {
        _productService = productService;
        Title = "Products";
    }

    [RelayCommand]
    private async Task LoadProductsAsync()
    {
        if (IsBusy)
            return;

        try
        {
            Console.WriteLine("=== LoadProductsAsync START ===");
            IsBusy = true;
            HasError = false;
            ErrorMessage = string.Empty;

            Products.Clear();

            Console.WriteLine("Calling GetProductsAsync...");
            var products = await _productService.GetProductsAsync();

            Console.WriteLine($"Received {products.Count} products");

            foreach (var product in products)
            {
                Console.WriteLine($"Adding product: {product.Name}");
                Products.Add(product);
            }

            Console.WriteLine($"Products collection now has {Products.Count} items");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"ERROR in LoadProductsAsync: {ex.Message}");
            Console.WriteLine($"Stack trace: {ex.StackTrace}");
            HasError = true;
            ErrorMessage = $"Unable to load products: {ex.Message}";
        }
        finally
        {
            IsBusy = false;
            Console.WriteLine("=== LoadProductsAsync END ===");
        }
    }

    [RelayCommand]
    private async Task ProductTappedAsync(Product product)
    {
        if (product == null)
            return;

        Console.WriteLine($"Product tapped: {product.Name} (ID: {product.Id})");

        try
        {
            // Get the navigation
            var navigation = Application.Current?.MainPage?.Navigation;
            if (navigation == null)
            {
                Console.WriteLine("ERROR: Navigation is null!");
                return;
            }

            // Get ProductDetailsPage from DI
            var detailsPage = App.Current.Handler.MauiContext.Services.GetService<ProductDetailsPage>();
            if (detailsPage?.BindingContext is ProductDetailsViewModel vm)
            {
                vm.ProductId = product.Id;
                await navigation.PushAsync(detailsPage);
            }
            else
            {
                Console.WriteLine("ERROR: Could not get ProductDetailsPage or ViewModel!");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"ERROR navigating to details: {ex.Message}");
        }
    }
}