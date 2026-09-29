using ShushineStudio.Mobile.Presentation.ViewModels.Chat;

namespace ShushineStudio.Mobile.Presentation.Views.Chat;

public partial class CustomRequestPage : ContentPage
{
    public CustomRequestPage(CustomRequestViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
