using ShushineStudio.Mobile.Presentation.ViewModels.Catalog;

namespace ShushineStudio.Mobile.Presentation.Views.Catalog;

public partial class ClientProductsPage : ContentPage
{
    private readonly ClientProductsViewModel _viewModel;

    public ClientProductsPage(ClientProductsViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _viewModel.CargarProductosAsync();
    }
}
