using System.Text.RegularExpressions;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ShushineStudio.Mobile.Domain.Repositories;

namespace ShushineStudio.Mobile.Presentation.ViewModels.Auth;

/// <summary>
/// ViewModel reactivo para el inicio de sesión de clientes (US-2.02 / Wireframe Pág. 6).
/// Valida formato de credenciales, autentica contra la Web API y gestiona sesión segura.
/// </summary>
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

        // 1. Validar campos no vacíos
        if (string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(Password))
        {
            await ShowAlertAsync(
                "Campos Incompletos", 
                "Por favor, ingresa tu usuario o correo electrónico y tu contraseña."
            );
            return;
        }

        try
        {
            IsBusy = true;
            ErrorMessage = null;

            var success = await _authRepository.LoginAsync(Email.Trim(), Password);
            if (success)
            {
                // Limpiar contraseña de memoria tras autenticación exitosa
                Password = string.Empty;

                // Obtener usuario autenticado para determinar el rol oficial
                var currentUser = await _authRepository.GetCurrentUserAsync();
                var rol = currentUser?.Rol?.ToUpperInvariant() ?? "CLIENTE";

                if (rol.Contains("ADMIN"))
                {
                    if (Shell.Current is AppShell appShell)
                    {
                        appShell.SwitchToRole("ADMIN");
                    }
                    else
                    {
                        await Shell.Current.GoToAsync("//AdminTabs/AdminDashboardPage");
                    }
                }
                else
                {
                    if (Shell.Current is AppShell appShell)
                    {
                        appShell.SwitchToRole("CLIENTE");
                    }
                    else
                    {
                        await Shell.Current.GoToAsync("//MainTabs/CatalogPage");
                    }
                }
            }
            else
            {
                await ShowAlertAsync(
                    "Credenciales Incorrectas", 
                    "El usuario o contraseña no coinciden con ninguna cuenta registrada. Por favor verifica tus datos."
                );
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = ex.Message;
            await ShowAlertAsync("Atención", ex.Message);
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private async Task ForgotPasswordAsync()
    {
        await ShowAlertAsync(
            "Recuperar Contraseña", 
            "Para restablecer tu contraseña, ingresa al portal web del salón o solicita asistencia en recepción al (+503) 2400-0000.", 
            "Entendido"
        );
    }

    [RelayCommand]
    private async Task NavigateToRegisterAsync()
    {
        await Shell.Current.GoToAsync("RegisterPage");
    }

    private static async Task ShowAlertAsync(string title, string message, string cancel = "Aceptar")
    {
        if (Application.Current?.MainPage != null)
        {
            await Application.Current.MainPage.DisplayAlert(title, message, cancel);
        }
    }
}
