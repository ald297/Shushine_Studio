using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ShushineStudio.Mobile.Data.Services;
using ShushineStudio.Mobile.Domain.Repositories;

namespace ShushineStudio.Mobile.Presentation.ViewModels.Admin;

public partial class AdminSettingsViewModel : ObservableObject
{
    private readonly IAuthRepository _authRepository;
    private readonly ITokenStorageService _tokenStorageService;

    [ObservableProperty]
    private string title = "Configuración";

    [ObservableProperty]
    private bool isAuthorized = true;

    [ObservableProperty]
    private bool isUnauthorized = false;

    // 1. Datos del Usuario Administrador (Backend /api/auth/me)
    [ObservableProperty]
    private string adminNombre = "Alex Fernando Alfaro";

    [ObservableProperty]
    private string adminLogin = "admin";

    [ObservableProperty]
    private string adminRol = "ADMIN";

    [ObservableProperty]
    private string adminTelefono = "7000-0000";

    // 2. Parámetros del Salón (Lectura)
    [ObservableProperty]
    private string salonNombre = "Shushine Studio";

    [ObservableProperty]
    private string salonSede = "ESFE AGAPE / MEGATEC • Sonsonate";

    [ObservableProperty]
    private string salonHorario = "Lunes a Sábado • 09:00 AM - 08:30 PM";

    [ObservableProperty]
    private string salonRegimenFiscal = "13% IVA Incluido • Normativa El Salvador";

    // 3. Preferencias Locales (Preferences)
    [ObservableProperty]
    private bool notificacionesPush = true;

    [ObservableProperty]
    private bool sonidoNotificaciones = true;

    [ObservableProperty]
    private bool recordatoriosAgenda = true;

    public AdminSettingsViewModel(
        IAuthRepository authRepository,
        ITokenStorageService tokenStorageService)
    {
        _authRepository = authRepository;
        _tokenStorageService = tokenStorageService;

        CargarPreferenciasLocales();
        _ = CargarDatosUsuarioAsync();
    }

    private void CargarPreferenciasLocales()
    {
        NotificacionesPush = Preferences.Default.Get("Admin_NotifPush", true);
        SonidoNotificaciones = Preferences.Default.Get("Admin_SonidoNotif", true);
        RecordatoriosAgenda = Preferences.Default.Get("Admin_RecordatoriosAgenda", true);
    }

    partial void OnNotificacionesPushChanged(bool value) => Preferences.Default.Set("Admin_NotifPush", value);
    partial void OnSonidoNotificacionesChanged(bool value) => Preferences.Default.Set("Admin_SonidoNotif", value);
    partial void OnRecordatoriosAgendaChanged(bool value) => Preferences.Default.Set("Admin_RecordatoriosAgenda", value);

    private async Task CargarDatosUsuarioAsync()
    {
        try
        {
            var rol = await _tokenStorageService.GetRoleAsync();
            var currentUser = await _authRepository.GetCurrentUserAsync();
            if (currentUser != null)
            {
                AdminNombre = !string.IsNullOrWhiteSpace(currentUser.NombreCompleto) 
                    ? currentUser.NombreCompleto 
                    : currentUser.Nombre;
                AdminLogin = !string.IsNullOrWhiteSpace(currentUser.Email) 
                    ? currentUser.Email 
                    : "admin";
                AdminRol = !string.IsNullOrWhiteSpace(currentUser.Rol) 
                    ? currentUser.Rol 
                    : "ADMIN";

                if (!string.IsNullOrEmpty(currentUser.Rol))
                {
                    rol = currentUser.Rol;
                }
            }

            var esAdmin = !string.IsNullOrEmpty(rol) && rol.ToUpperInvariant().Contains("ADMIN");
            IsAuthorized = esAdmin;
            IsUnauthorized = !esAdmin;
        }
        catch
        {
            IsAuthorized = false;
            IsUnauthorized = true;
        }
    }

    [RelayCommand]
    private async Task CambiarPasswordAsync()
    {
        await Shell.Current.GoToAsync("SecurityPage");
    }

    [RelayCommand]
    private async Task CerrarSesionAsync()
    {
        if (Application.Current?.MainPage != null)
        {
            var confirma = await Application.Current.MainPage.DisplayAlert(
                "Cerrar Sesión",
                "¿Está seguro de cerrar la sesión administrativa?",
                "Cerrar Sesión",
                "Cancelar"
            );
            if (!confirma) return;
        }

        await _authRepository.LogoutAsync();
        if (Shell.Current is AppShell appShell)
        {
            appShell.SwitchToLogin();
        }
        else
        {
            await Shell.Current.GoToAsync("//LoginPage");
        }
    }

    [RelayCommand]
    private async Task RegresarAsync()
    {
        await Shell.Current.GoToAsync("..");
    }

    [RelayCommand]
    private async Task RegresarALoginAsync()
    {
        await _authRepository.LogoutAsync();
        if (Shell.Current is AppShell appShell)
        {
            appShell.SwitchToLogin();
        }
        else
        {
            await Shell.Current.GoToAsync("//LoginPage");
        }
    }
}
