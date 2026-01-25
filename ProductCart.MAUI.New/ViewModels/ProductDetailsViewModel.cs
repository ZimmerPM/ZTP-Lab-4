using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ProductCart.MAUI.Models;
using ProductCart.MAUI.Services.Interfaces;

namespace ProductCart.MAUI.ViewModels;

[QueryProperty(nameof(ProductId), nameof(ProductId))]
public partial class ProductDetailsViewModel : BaseViewModel
{
    private readonly IProductService _productService;

    [ObservableProperty]
    private Product? _product;

    [ObservableProperty]
    private int _productId;

    public ProductDetailsViewModel(IProductService productService)
    {
        _productService = productService;
        Title = "Product Details";
    }

    [RelayCommand]
    private async Task LoadProductAsync()
    {
        if (IsBusy || _productId == 0)
            return;

        try
        {
            Console.WriteLine($"=== LoadProductAsync START for ID: {_productId} ===");
            IsBusy = true;
            HasError = false;

            var product = await _productService.GetProductByIdAsync(_productId);

            if (product != null)
            {
                Product = product;
                Console.WriteLine($"Loaded product: {product.Name}");
                Title = product.Name;
            }
            else
            {
                Console.WriteLine("Product not found!");
                HasError = true;
                ErrorMessage = "Product not found";
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"ERROR loading product: {ex.Message}");
            HasError = true;
            ErrorMessage = $"Failed to load product: {ex.Message}";
        }
        finally
        {
            IsBusy = false;
        }
    }

    partial void OnProductIdChanged(int value)
    {
        Console.WriteLine($"ProductId changed to: {value}");
        if (value > 0)
        {
            _ = LoadProductAsync();
        }
    }
}