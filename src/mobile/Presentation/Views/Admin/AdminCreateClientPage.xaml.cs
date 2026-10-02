using ShushineStudio.Mobile.Presentation.ViewModels.Admin;

namespace ShushineStudio.Mobile.Presentation.Views.Admin;

public partial class AdminCreateClientPage : ContentPage
{
    public AdminCreateClientPage(AdminCreateClientViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
