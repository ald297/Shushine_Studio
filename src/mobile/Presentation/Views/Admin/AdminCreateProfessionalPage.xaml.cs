using ShushineStudio.Mobile.Presentation.ViewModels.Admin;

namespace ShushineStudio.Mobile.Presentation.Views.Admin;

public partial class AdminCreateProfessionalPage : ContentPage
{
    public AdminCreateProfessionalPage(AdminCreateProfessionalViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
