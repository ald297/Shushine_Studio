using ShushineStudio.Mobile.Presentation.ViewModels.Appointments;

namespace ShushineStudio.Mobile.Presentation.Views.Appointments;

public partial class SeleccionHorarioPage : ContentPage
{
    private readonly SeleccionHorarioViewModel _viewModel;

    public SeleccionHorarioPage(SeleccionHorarioViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = _viewModel = viewModel;
    }
}
