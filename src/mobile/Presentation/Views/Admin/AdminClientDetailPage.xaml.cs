using ShushineStudio.Mobile.Presentation.ViewModels.Admin;

namespace ShushineStudio.Mobile.Presentation.Views.Admin;

public partial class AdminClientDetailPage : ContentPage
{
    public AdminClientDetailPage(AdminClientDetailViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
