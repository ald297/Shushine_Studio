using ShushineStudio.Mobile.Presentation.ViewModels.Admin;

namespace ShushineStudio.Mobile.Presentation.Views.Admin;

public partial class AdminProductsPage : ContentPage
{
    public AdminProductsPage(AdminProductsViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
