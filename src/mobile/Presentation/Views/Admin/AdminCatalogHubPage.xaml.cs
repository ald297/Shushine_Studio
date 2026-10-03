namespace ShushineStudio.Mobile.Presentation.Views.Admin;

public partial class AdminCatalogHubPage : ContentPage
{
	private bool _mostrandoServicios = true;

	public AdminCatalogHubPage()
	{
		InitializeComponent();
	}

	private void OnServiciosTapped(object sender, EventArgs e)
	{
		if (_mostrandoServicios) return;
		_mostrandoServicios = true;

		// Activar chip Servicios
		ChipServicios.BackgroundColor = (Color)Application.Current!.Resources["Primary"];
		ChipServicios.Stroke = Colors.Transparent;
		LabelServicios.TextColor = Colors.White;

		// Desactivar chip Productos
		ChipProductos.BackgroundColor = (Color)Application.Current!.Resources["SurfaceVariant"];
		ChipProductos.Stroke = (Color)Application.Current!.Resources["Border"];
		LabelProductos.TextColor = (Color)Application.Current!.Resources["TextSecondary"];

		// Mostrar tarjetas de servicios
		CardServicios.IsVisible = true;
		CardNuevoServicio.IsVisible = true;
		CardProductos.IsVisible = false;
		CardNuevoProducto.IsVisible = false;
	}

	private void OnProductosTapped(object sender, EventArgs e)
	{
		if (!_mostrandoServicios) return;
		_mostrandoServicios = false;

		// Activar chip Productos
		ChipProductos.BackgroundColor = (Color)Application.Current!.Resources["Primary"];
		ChipProductos.Stroke = Colors.Transparent;
		LabelProductos.TextColor = Colors.White;

		// Desactivar chip Servicios
		ChipServicios.BackgroundColor = (Color)Application.Current!.Resources["SurfaceVariant"];
		ChipServicios.Stroke = (Color)Application.Current!.Resources["Border"];
		LabelServicios.TextColor = (Color)Application.Current!.Resources["TextSecondary"];

		// Mostrar tarjetas de productos
		CardServicios.IsVisible = false;
		CardNuevoServicio.IsVisible = false;
		CardProductos.IsVisible = true;
		CardNuevoProducto.IsVisible = true;
	}

	private async void OnIrAServiciosTapped(object sender, EventArgs e)
	{
		await Shell.Current.GoToAsync("AdminCatalogPage");
	}

	private async void OnNuevoServicioTapped(object sender, EventArgs e)
	{
		await Shell.Current.GoToAsync("AdminCreateServicePage");
	}

	private async void OnIrAProductosTapped(object sender, EventArgs e)
	{
		await Shell.Current.GoToAsync("AdminProductsPage");
	}

	private async void OnNuevoProductoTapped(object sender, EventArgs e)
	{
		await Shell.Current.GoToAsync("AdminProductDetailPage");
	}
}
