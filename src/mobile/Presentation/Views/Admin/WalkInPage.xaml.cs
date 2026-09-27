using ShushineStudio.Mobile.Presentation.ViewModels.Admin;

namespace ShushineStudio.Mobile.Presentation.Views.Admin;

public partial class WalkInPage : ContentPage
{
    public WalkInPage(WalkInViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
