using CommunityToolkit.Mvvm.Input;
using ProductCart.MAUI.Models;
using ProductCart.MAUI.Services.Interfaces;
using ProductCart.MAUI.Views;
using System.Collections.ObjectModel;
using System.Diagnostics;

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
            Debug.WriteLine("=== LoadProductsAsync START ===");
            IsBusy = true;
            HasError = false;
            ErrorMessage = string.Empty;

            Products.Clear();

            Debug.WriteLine("Calling GetProductsAsync...");
            var products = await _productService.GetProductsAsync();

            Debug.WriteLine($"Received {products.Count} products");

            foreach (var product in products)
            {
                Debug.WriteLine($"Adding product: {product.Name}");
                Products.Add(product);
            }

            Debug.WriteLine($"Products collection now has {Products.Count} items");
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"ERROR in LoadProductsAsync: {ex.Message}");
            Debug.WriteLine($"Stack trace: {ex.StackTrace}");
            HasError = true;
            ErrorMessage = $"Unable to load products: {ex.Message}";
        }
        finally
        {
            IsBusy = false;
            Debug.WriteLine("=== LoadProductsAsync END ===");
        }
    }

    [RelayCommand]
    private async Task ProductTappedAsync(Product product)
    {
        if (product == null)
            return;

        Debug.WriteLine($"Product tapped: {product.Name} (ID: {product.Id})");

        try
        {
            var navigation = Application.Current?.MainPage?.Navigation;
            if (navigation == null)
            {
                Debug.WriteLine("ERROR: Navigation is null!");
                return;
            }

            var detailsPage = App.Current.Handler.MauiContext.Services.GetService<ProductDetailsPage>();
            if (detailsPage?.BindingContext is ProductDetailsViewModel vm)
            {
                vm.ProductId = product.Id;
                await navigation.PushAsync(detailsPage);
            }
            else
            {
                Debug.WriteLine("ERROR: Could not get ProductDetailsPage or ViewModel!");
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"ERROR navigating to details: {ex.Message}");
        }
    }
}