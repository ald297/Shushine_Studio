using ShushineStudio.Mobile.Presentation.ViewModels.Admin;

namespace ShushineStudio.Mobile.Presentation.Views.Admin;

public partial class AdminServicesPage : ContentPage
{
    public AdminServicesPage(AdminServicesViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
