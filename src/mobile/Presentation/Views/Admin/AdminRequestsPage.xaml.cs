using ShushineStudio.Mobile.Presentation.ViewModels.Admin;

namespace ShushineStudio.Mobile.Presentation.Views.Admin;

public partial class AdminRequestsPage : ContentPage
{
    public AdminRequestsPage(AdminRequestsViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
