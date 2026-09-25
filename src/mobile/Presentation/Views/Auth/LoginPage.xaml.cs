using ShushineStudio.Mobile.Presentation.ViewModels.Auth;

namespace ShushineStudio.Mobile.Presentation.Views.Auth;

public partial class LoginPage : ContentPage
{
	public LoginPage(LoginViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;
	}
}
