using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ShushineStudio.Mobile.Domain.Repositories;

namespace ShushineStudio.Mobile.Presentation.ViewModels.Auth;

public partial class RegisterViewModel : BaseViewModel
{
    private readonly IAuthRepository _authRepository;

    [ObservableProperty]
    private string nombre = string.Empty;

    [ObservableProperty]
    private string email = string.Empty;

    [ObservableProperty]
    private string password = string.Empty;

    [ObservableProperty]
    private string telefono = string.Empty;

    public RegisterViewModel(IAuthRepository authRepository)
    {
        _authRepository = authRepository;
        Title = "Crear Cuenta";
    }

    [RelayCommand]
    private async Task RegisterAsync()
    {
        if (IsBusy) return;

        if (string.IsNullOrWhiteSpace(Nombre) || string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(Password))
        {
            if (Application.Current?.MainPage != null)
                await Application.Current.MainPage.DisplayAlert("Campos Obligatorios", "Nombre, correo y contraseña son obligatorios.", "Aceptar");
            return;
        }

        try
        {
            IsBusy = true;
            ErrorMessage = null;

            var success = await _authRepository.RegisterAsync(Nombre.Trim(), Email.Trim(), Password, Telefono.Trim());
            if (success)
            {
                if (Application.Current?.MainPage != null)
                    await Application.Current.MainPage.DisplayAlert("¡Bienvenida a Shushine!", "Tu cuenta ha sido creada exitosamente.", "Continuar");

                await Shell.Current.GoToAsync("//MainTabs/CatalogPage");
            }
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private async Task NavigateBackToLoginAsync()
    {
        await Shell.Current.GoToAsync("..");
    }
}
