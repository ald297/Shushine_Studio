using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ShushineStudio.Mobile.Domain.Repositories;

namespace ShushineStudio.Mobile.Presentation.ViewModels.Auth;

public partial class LoginViewModel : BaseViewModel
{
    private readonly IAuthRepository _authRepository;

    [ObservableProperty]
    private string email = string.Empty;

    [ObservableProperty]
    private string password = string.Empty;

    [ObservableProperty]
    private bool isPasswordHidden = true;

    public LoginViewModel(IAuthRepository authRepository)
    {
        _authRepository = authRepository;
        Title = "Iniciar Sesión";
    }

    [RelayCommand]
    private void TogglePasswordVisibility()
    {
        IsPasswordHidden = !IsPasswordHidden;
    }

    [RelayCommand]
    private async Task LoginAsync()
    {
        if (IsBusy) return;

        if (string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(Password))
        {
            if (Application.Current?.MainPage != null)
                await Application.Current.MainPage.DisplayAlert("Campos Incompletos", "Por favor ingresa tu correo y contraseña.", "Aceptar");
            return;
        }

        try
        {
            IsBusy = true;
            ErrorMessage = null;

            var success = await _authRepository.LoginAsync(Email.Trim(), Password);
            if (success)
            {
                // Navegar a la pantalla principal del Shell
                await Shell.Current.GoToAsync("//MainTabs/CatalogPage");
            }
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private async Task NavigateToRegisterAsync()
    {
        await Shell.Current.GoToAsync("RegisterPage");
    }
}
