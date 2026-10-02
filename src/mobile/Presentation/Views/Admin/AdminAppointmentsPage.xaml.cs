using ShushineStudio.Mobile.Presentation.ViewModels.Admin;

namespace ShushineStudio.Mobile.Presentation.Views.Admin;

public partial class AdminAppointmentsPage : ContentPage
{
    private readonly AdminAppointmentsViewModel _viewModel;

    public AdminAppointmentsPage(AdminAppointmentsViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _viewModel.LoadCitasCommand.ExecuteAsync(null);
    }
}
