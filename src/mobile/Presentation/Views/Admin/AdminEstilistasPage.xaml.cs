using ShushineStudio.Mobile.Presentation.ViewModels.Admin;

namespace ShushineStudio.Mobile.Presentation.Views.Admin;

public partial class AdminEstilistasPage : ContentPage
{
    public AdminEstilistasPage(AdminEstilistasViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
