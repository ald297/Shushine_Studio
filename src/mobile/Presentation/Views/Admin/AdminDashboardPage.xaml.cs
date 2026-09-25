using ShushineStudio.Mobile.Presentation.ViewModels.Admin;

namespace ShushineStudio.Mobile.Presentation.Views.Admin;

public partial class AdminDashboardPage : ContentPage
{
    public AdminDashboardPage(AdminDashboardViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
