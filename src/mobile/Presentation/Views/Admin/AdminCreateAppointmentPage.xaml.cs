using ShushineStudio.Mobile.Presentation.ViewModels.Admin;

namespace ShushineStudio.Mobile.Presentation.Views.Admin;

public partial class AdminCreateAppointmentPage : ContentPage
{
    private readonly AdminCreateAppointmentViewModel _viewModel;

    public AdminCreateAppointmentPage(AdminCreateAppointmentViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _viewModel.CargarCatalogosCommand.ExecuteAsync(null);
    }
}
