using ShushineStudio.Mobile.Presentation.ViewModels.Admin;

namespace ShushineStudio.Mobile.Presentation.Views.Admin;

public partial class AdminCreateServicePage : ContentPage
{
    public AdminCreateServicePage(AdminCreateServiceViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
