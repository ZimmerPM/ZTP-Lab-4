using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ProductCart.MAUI.Models;
using ProductCart.MAUI.Services.Interfaces;
using System.Collections.ObjectModel;

namespace ProductCart.MAUI.ViewModels;

public partial class CartViewModel : BaseViewModel
{
    private readonly ICartService _cartService;
    private readonly IProductService _productService;

    [ObservableProperty]
    private ObservableCollection<CartItem> _items = new();

    [ObservableProperty]
    private decimal _totalValue;

    [ObservableProperty]
    private int _itemCount;

    [ObservableProperty]
    private string _displayTotal = "$0.00";

    private const string DefaultUserId = "maui-user-123";

    public CartViewModel(ICartService cartService, IProductService productService)
    {
        _cartService = cartService;
        _productService = productService;
        Title = "Shopping Cart";
    }

    [RelayCommand]
    private async Task LoadCartAsync()
    {
        if (IsBusy)
            return;

        try
        {
            Console.WriteLine("=== LoadCartAsync START ===");
            IsBusy = true;
            HasError = false;

            var loadedCart = await _cartService.GetCartAsync(DefaultUserId);

            if (loadedCart != null)
            {
                Items.Clear();

                // Enrich cart items with product data
                foreach (var item in loadedCart.Items)
                {
                    var product = await _productService.GetProductByIdAsync(item.ProductId);
                    if (product != null)
                    {
                        item.ProductName = product.Name;
                        item.Price = product.Price;
                    }
                    Items.Add(item);
                }

                UpdateTotals();
                Console.WriteLine($"Cart loaded: {Items.Count} items, Total: {DisplayTotal}");
            }
            else
            {
                Console.WriteLine("Cart not found!");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"ERROR in LoadCartAsync: {ex.Message}");
            HasError = true;
            ErrorMessage = $"Failed to load cart: {ex.Message}";
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private async Task RemoveItemAsync(CartItem item)
    {
        if (item == null)
            return;

        try
        {
            Console.WriteLine($"Removing item: {item.ProductName}");

            var productGuid = new Guid($"{item.ProductId:X8}-0000-0000-0000-000000000000");
            var success = await _cartService.RemoveProductFromCartAsync(DefaultUserId, productGuid);

            if (success)
            {
                Items.Remove(item);
                UpdateTotals();
                Console.WriteLine("Item removed successfully");
            }
            else
            {
                await App.Current.MainPage.DisplayAlert("Error", "Failed to remove item", "OK");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"ERROR removing item: {ex.Message}");
            await App.Current.MainPage.DisplayAlert("Error", $"Failed to remove item: {ex.Message}", "OK");
        }
    }

    [RelayCommand]
    private async Task CheckoutAsync()
    {
        if (Items.Count == 0)
        {
            await App.Current.MainPage.DisplayAlert("Cart Empty", "Add items to cart before checkout", "OK");
            return;
        }

        try
        {
            Console.WriteLine("Starting checkout...");
            IsBusy = true;

            var result = await _cartService.CheckoutCartAsync(DefaultUserId);

            if (result != null)
            {
                await App.Current.MainPage.DisplayAlert("Success",
                    $"Order placed! Total: {DisplayTotal}", "OK");

                // Clear cart
                Items.Clear();
                UpdateTotals();
            }
            else
            {
                await App.Current.MainPage.DisplayAlert("Error", "Failed to checkout", "OK");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"ERROR during checkout: {ex.Message}");
            await App.Current.MainPage.DisplayAlert("Error", $"Checkout failed: {ex.Message}", "OK");
        }
        finally
        {
            IsBusy = false;
        }
    }

    public async Task AddProductToCartAsync(Guid productId, int quantity = 1)
    {
        try
        {
            Console.WriteLine($"Adding product {productId} to cart (qty: {quantity})");

            var success = await _cartService.AddProductToCartAsync(DefaultUserId, productId, quantity);

            if (success)
            {
                Console.WriteLine("Product added to cart successfully");
                await LoadCartAsync(); // Refresh cart
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"ERROR adding product to cart: {ex.Message}");
        }
    }

    private void UpdateTotals()
    {
        TotalValue = Items.Sum(i => i.TotalPrice);
        ItemCount = Items.Sum(i => i.Quantity);
        DisplayTotal = $"${TotalValue:F2}";
    }
}
