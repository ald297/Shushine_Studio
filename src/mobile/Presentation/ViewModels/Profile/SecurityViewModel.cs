using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace ShushineStudio.Mobile.Presentation.ViewModels.Profile;

/// <summary>
/// ViewModel de Seguridad. No existe endpoint de cambio de contraseña en el backend.
/// Controladores existentes: AuthController solo tiene /login, /registro, /me.
/// Esta pantalla es UI preparada.
/// </summary>
public partial class SecurityViewModel : BaseViewModel
{
    [ObservableProperty]
    private string contrasenaActual = string.Empty;

    [ObservableProperty]
    private string contrasenaNueva = string.Empty;

    [ObservableProperty]
    private string contrasenaConfirmar = string.Empty;

    [ObservableProperty]
    private bool mostrarContrasenaActual = false;

    [ObservableProperty]
    private bool mostrarContrasenaNueva = false;

    [ObservableProperty]
    private bool mostrarContrasenaConfirmar = false;

    // No existe endpoint de cambio de contraseña
    public bool BackendCambioContrasenaDisponible => false;

    public SecurityViewModel()
    {
        Title = "Seguridad";
    }

    [RelayCommand]
    private void ToggleMostrarActual() => MostrarContrasenaActual = !MostrarContrasenaActual;

    [RelayCommand]
    private void ToggleMostrarNueva() => MostrarContrasenaNueva = !MostrarContrasenaNueva;

    [RelayCommand]
    private void ToggleMostrarConfirmar() => MostrarContrasenaConfirmar = !MostrarContrasenaConfirmar;

    [RelayCommand]
    private async Task CambiarContrasenaAsync()
    {
        // Validaciones de UI
        if (string.IsNullOrWhiteSpace(ContrasenaActual) ||
            string.IsNullOrWhiteSpace(ContrasenaNueva) ||
            string.IsNullOrWhiteSpace(ContrasenaConfirmar))
        {
            ErrorMessage = "Todos los campos son requeridos.";
            return;
        }

        if (ContrasenaNueva != ContrasenaConfirmar)
        {
            ErrorMessage = "Las contraseñas nuevas no coinciden.";
            return;
        }

        if (ContrasenaNueva.Length < 8)
        {
            ErrorMessage = "La contraseña debe tener al menos 8 caracteres.";
            return;
        }

        // UI preparada — no existe endpoint de cambio de contraseña
        if (Application.Current?.MainPage != null)
        {
            await Application.Current.MainPage.DisplayAlert(
                "Función no disponible",
                "El cambio de contraseña requiere soporte en el backend. Esta función estará disponible cuando se implemente el endpoint correspondiente.",
                "Entendido"
            );
        }
    }

    [RelayCommand]
    private async Task RegresarAsync()
        => await Shell.Current.GoToAsync("..");
}
