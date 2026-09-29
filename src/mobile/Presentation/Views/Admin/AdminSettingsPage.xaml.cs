using ShushineStudio.Mobile.Presentation.ViewModels.Admin;

namespace ShushineStudio.Mobile.Presentation.Views.Admin;

public partial class AdminSettingsPage : ContentPage
{
    public AdminSettingsPage(AdminSettingsViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
