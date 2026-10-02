using ShushineStudio.Mobile.Presentation.ViewModels.Catalog;

namespace ShushineStudio.Mobile.Presentation.Views.Catalog;

public partial class CatalogPage : ContentPage
{
	private readonly CatalogViewModel _viewModel;

	public CatalogPage(CatalogViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = _viewModel = viewModel;
	}

	protected override async void OnAppearing()
	{
		base.OnAppearing();
		if (_viewModel.Servicios.Count == 0)
		{
			await _viewModel.LoadServiciosCommand.ExecuteAsync(null);
		}
	}
}
