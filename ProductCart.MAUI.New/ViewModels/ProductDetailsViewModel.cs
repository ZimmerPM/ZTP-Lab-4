using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ProductCart.MAUI.Models;
using ProductCart.MAUI.Services.Interfaces;
using ProductCart.MAUI.Views;

namespace ProductCart.MAUI.ViewModels;

[QueryProperty(nameof(ProductId), nameof(ProductId))]
public partial class ProductDetailsViewModel : BaseViewModel
{
    private readonly IProductService _productService;
    private readonly ICartService _cartService;

    [ObservableProperty]
    private Product? _product;

    [ObservableProperty]
    private int _productId;

    [ObservableProperty]
    private int _quantityToAdd = 1;

    public ProductDetailsViewModel(IProductService productService, ICartService cartService)
    {
        _productService = productService;
        _cartService = cartService;
        Title = "Product Details";
    }

    [RelayCommand]
    private async Task LoadProductAsync()
    {
        if (IsBusy || ProductId == 0)
            return;

        try
        {
            Console.WriteLine($"=== LoadProductAsync START for ID: {ProductId} ===");
            IsBusy = true;
            HasError = false;

            var product = await _productService.GetProductByIdAsync(ProductId);

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

    [RelayCommand]
    private void IncreaseQuantity()
    {
        if (Product != null && QuantityToAdd < Product.Quantity)
        {
            QuantityToAdd++;
        }
    }

    [RelayCommand]
    private void DecreaseQuantity()
    {
        if (QuantityToAdd > 1)
        {
            QuantityToAdd--;
        }
    }

    [RelayCommand]
    private async Task AddToCartAsync()
    {
        if (Product == null || QuantityToAdd <= 0)
            return;

        if (QuantityToAdd > Product.Quantity)
        {
            await App.Current.MainPage.DisplayAlert("Error",
                $"Only {Product.Quantity} items in stock!", "OK");
            return;
        }

        try
        {
            Console.WriteLine($"Adding {QuantityToAdd}x {Product.Name} to cart");

            IsBusy = true;

            var productGuid = new Guid($"{Product.Id:X8}-0000-0000-0000-000000000000");
            Console.WriteLine($"Converted Product.Id {Product.Id} to Guid {productGuid}");

            var cartViewModel = App.Current.Handler.MauiContext.Services.GetService<CartViewModel>();
            if (cartViewModel != null)
            {
                await cartViewModel.AddProductToCartAsync(productGuid, QuantityToAdd);

                await App.Current.MainPage.DisplayAlert("Success",
                    $"Added {QuantityToAdd}x {Product.Name} to cart!", "OK");

                QuantityToAdd = 1;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"ERROR adding to cart: {ex.Message}");
            await App.Current.MainPage.DisplayAlert("Error",
                $"Failed to add to cart: {ex.Message}", "OK");
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private async Task ViewCartAsync()
    {
        try
        {
            var navigation = Application.Current?.MainPage?.Navigation;
            if (navigation != null)
            {
                var cartPage = App.Current.Handler.MauiContext.Services.GetService<CartPage>();
                if (cartPage != null)
                {
                    await navigation.PushAsync(cartPage);
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"ERROR navigating to cart: {ex.Message}");
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
