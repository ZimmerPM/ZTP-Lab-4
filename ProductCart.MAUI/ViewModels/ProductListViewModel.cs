using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ProductCart.MAUI.Models;
using ProductCart.MAUI.Services.Interfaces;
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
            IsBusy = true;
            HasError = false;
            ErrorMessage = string.Empty;

            Products.Clear();

            var products = await _productService.GetProductsAsync();

            foreach (var product in products)
            {
                Products.Add(product);
            }
        }
        catch (Exception ex)
        {
            HasError = true;
            ErrorMessage = $"Unable to load products: {ex.Message}";
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private async Task ProductTappedAsync(Product product)
    {
        if (product == null)
            return;

        // TODO: Nawigacja do szczegółów
        await Shell.Current.DisplayAlert("Product", $"Selected: {product.Name}", "OK");
    }
}