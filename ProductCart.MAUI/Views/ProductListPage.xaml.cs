using ProductCart.MAUI.ViewModels;

namespace ProductCart.MAUI.Views;

public partial class ProductListPage : ContentPage
{
    private readonly ProductListViewModel _viewModel;

    public ProductListPage(ProductListViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        if (_viewModel.Products.Count == 0)
        {
            await _viewModel.LoadProductsCommand.ExecuteAsync(null);
        }
    }
}
