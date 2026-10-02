using ShushineStudio.Mobile.Presentation.ViewModels.Admin;

namespace ShushineStudio.Mobile.Presentation.Views.Admin;

public partial class AdminProfessionalDetailPage : ContentPage
{
    public AdminProfessionalDetailPage(AdminProfessionalDetailViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
