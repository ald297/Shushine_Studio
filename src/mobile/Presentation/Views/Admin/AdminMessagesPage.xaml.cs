using ShushineStudio.Mobile.Presentation.ViewModels.Admin;

namespace ShushineStudio.Mobile.Presentation.Views.Admin;

public partial class AdminMessagesPage : ContentPage
{
	private readonly AdminMessagesViewModel _viewModel;

	public AdminMessagesPage(AdminMessagesViewModel viewModel)
	{
		InitializeComponent();
		_viewModel = viewModel;
		BindingContext = viewModel;
	}

	protected override async void OnAppearing()
	{
		base.OnAppearing();
		await _viewModel.CargarConversacionesAsync();
		_viewModel.IniciarPolling();
	}

	protected override void OnDisappearing()
	{
		base.OnDisappearing();
		_viewModel.DetenerPolling();
	}

	private async void OnSolicitudesTapped(object sender, EventArgs e)
	{
		await Shell.Current.GoToAsync("AdminRequestsPage");
	}

	private async void OnCotizacionesTapped(object sender, EventArgs e)
	{
		// Cotizaciones usa el mismo AdminRequestsPage con filtro de tipo cotización
		await Shell.Current.GoToAsync("AdminRequestsPage");
	}

	private async void OnResenasTapped(object sender, EventArgs e)
	{
		await Shell.Current.GoToAsync("AdminReviewsPage");
	}
}
