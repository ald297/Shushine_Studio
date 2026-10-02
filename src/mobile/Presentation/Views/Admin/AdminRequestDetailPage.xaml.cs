using ShushineStudio.Mobile.Presentation.ViewModels.Admin;

namespace ShushineStudio.Mobile.Presentation.Views.Admin;

public partial class AdminRequestDetailPage : ContentPage
{
    public AdminRequestDetailPage(AdminRequestDetailViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
