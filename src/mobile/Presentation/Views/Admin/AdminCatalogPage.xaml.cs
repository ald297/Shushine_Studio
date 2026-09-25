using ShushineStudio.Mobile.Presentation.ViewModels.Admin;

namespace ShushineStudio.Mobile.Presentation.Views.Admin;

public partial class AdminCatalogPage : ContentPage
{
    public AdminCatalogPage(AdminCatalogViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
