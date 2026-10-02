using ShushineStudio.Mobile.Presentation.ViewModels.Admin;

namespace ShushineStudio.Mobile.Presentation.Views.Admin;

public partial class AdminReviewsPage : ContentPage
{
    public AdminReviewsPage(AdminReviewsViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
