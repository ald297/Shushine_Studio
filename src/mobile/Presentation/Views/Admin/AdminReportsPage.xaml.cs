using ShushineStudio.Mobile.Presentation.ViewModels.Admin;

namespace ShushineStudio.Mobile.Presentation.Views.Admin;

public partial class AdminReportsPage : ContentPage
{
    public AdminReportsPage(AdminReportsViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
