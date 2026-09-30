using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ShushineStudio.Mobile.Domain.Repositories;

namespace ShushineStudio.Mobile.Presentation.ViewModels.Admin;

/// <summary>
/// ViewModel para el Perfil Administrativo de Shushine Studio.
/// Carga datos reales desde GET /api/auth/me y métricas operativas del servidor.
/// Organizado estrictamente en secciones:
/// - Mi Información (Nombre, Apellido, Correo, Teléfono, Usuario)
/// - Cuenta (Rol, Estado, Seguridad)
/// - Preferencias Locales
/// - Aplicación
/// - Sesión (Cierre de sesión seguro)
/// </summary>
public partial class AdminProfileViewModel : BaseViewModel
{
    private readonly IAuthRepository _authRepository;
    private readonly IServicioRepository _servicioRepository;
    private readonly IEstilistaRepository _estilistaRepository;
    private readonly IReservaRepository _reservaRepository;

    // ────────────────────────────────────────────────────────────
    // 1. Mi Información (Datos Reales desde GET /api/auth/me)
    // ────────────────────────────────────────────────────────────
    [ObservableProperty]
    private string nombre = string.Empty;

    [ObservableProperty]
    private string apellido = string.Empty;

    [ObservableProperty]
    private string nombreCompleto = "Administrador General";

    [ObservableProperty]
    private string login = "admin";

    [ObservableProperty]
    private string correo = "admin@shushinestudio.com";

    [ObservableProperty]
    private string telefono = "No registrado";

    [ObservableProperty]
    private string rol = "Administrador";

    [ObservableProperty]
    private string estadoCuenta = "Activa / Operativa";

    [ObservableProperty]
    private string iniciales = "AD";

    // ────────────────────────────────────────────────────────────
    // 2. Métricas Operativas del Salón (Servidor Real)
    // ────────────────────────────────────────────────────────────
    [ObservableProperty]
    private int totalServicios;

    [ObservableProperty]
    private int totalEstilistas;

    [ObservableProperty]
    private int totalCitasHoy;

    // ────────────────────────────────────────────────────────────
    // 3. Preferencias Locales
    // ────────────────────────────────────────────────────────────
    [ObservableProperty]
    private bool notificacionesPush = true;

    [ObservableProperty]
    private bool sonidoNotificaciones = true;

    [ObservableProperty]
    private bool recordatoriosAgenda = true;

    // ────────────────────────────────────────────────────────────
    // 4. Información de la Aplicación
    // ────────────────────────────────────────────────────────────
    public string AppNombre => "Shushine Studio";
    public string AppVersion => "1.1.0";
    public string AppInfo => "Sistema de Gestión y Reservas de Salón de Belleza • Clean Architecture + MVVM";

    public AdminProfileViewModel(
        IAuthRepository authRepository,
        IServicioRepository servicioRepository,
        IEstilistaRepository estilistaRepository,
        IReservaRepository reservaRepository)
    {
        _authRepository = authRepository;
        _servicioRepository = servicioRepository;
        _estilistaRepository = estilistaRepository;
        _reservaRepository = reservaRepository;

        Title = "Perfil Administrador";
        CargarPreferenciasLocales();
        _ = CargarDatosAdminAsync();
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

    [RelayCommand]
    public async Task CargarDatosAdminAsync()
    {
        if (IsBusy) return;

        try
        {
            IsBusy = true;
            ErrorMessage = string.Empty;

            // 1. Cargar usuario autenticado desde backend (GET /api/auth/me)
            var user = await _authRepository.GetCurrentUserAsync();
            if (user != null)
            {
                var partes = user.NombreCompleto?.Split(' ', 2, StringSplitOptions.RemoveEmptyEntries) ?? [];
                Nombre = !string.IsNullOrWhiteSpace(user.Nombre) ? user.Nombre : (partes.Length > 0 ? partes[0] : "Admin");
                Apellido = !string.IsNullOrWhiteSpace(user.Apellido) ? user.Apellido : (partes.Length > 1 ? partes[1] : string.Empty);
                NombreCompleto = !string.IsNullOrWhiteSpace(user.NombreCompleto) ? user.NombreCompleto : $"{Nombre} {Apellido}".Trim();
                Login = !string.IsNullOrWhiteSpace(user.Login) ? user.Login : "admin";
                Correo = !string.IsNullOrWhiteSpace(user.Email) ? user.Email : $"{Login}@shushinestudio.com";
                Telefono = !string.IsNullOrWhiteSpace(user.Telefono) ? user.Telefono : "No registrado";
                Rol = "Administrador";
                EstadoCuenta = user.Activo ? "Activa / Operativa" : "Inactiva";

                // Iniciales generadas localmente
                if (!string.IsNullOrWhiteSpace(user.Iniciales))
                {
                    Iniciales = user.Iniciales;
                }
                else
                {
                    var initPartes = NombreCompleto.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                    Iniciales = string.Concat(initPartes.Take(2).Select(p => p[0])).ToUpperInvariant();
                }
            }

            // 2. Cargar contadores operativos reales
            var servicios = await _servicioRepository.GetServiciosAsync();
            TotalServicios = servicios.Count();

            var estilistas = await _estilistaRepository.GetTodosEstilistasAsync();
            TotalEstilistas = estilistas.Count();

            var metricas = await _reservaRepository.GetDashboardMetricasAsync();
            if (metricas != null)
            {
                TotalCitasHoy = (int)metricas.TotalCitasHoy;
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = "Error al sincronizar datos de perfil.";
            System.Diagnostics.Debug.WriteLine($"[AdminProfileViewModel] Error: {ex.Message}");
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private async Task IrAEditarPerfilAsync()
    {
        await Shell.Current.GoToAsync("EditProfilePage");
    }

    [RelayCommand]
    private async Task IrASeguridadAsync()
    {
        await Shell.Current.GoToAsync("SecurityPage");
    }

    [RelayCommand]
    private async Task IrAPreferenciasAsync()
    {
        await Shell.Current.GoToAsync("PreferencesPage");
    }

    [RelayCommand]
    private async Task IrAInfoAppAsync()
    {
        await Shell.Current.GoToAsync("AppInfoPage");
    }

    [RelayCommand]
    private async Task CerrarSesionAsync()
    {
        if (Application.Current?.MainPage == null) return;

        bool confirm = await Application.Current.MainPage.DisplayAlert(
            "Cerrar Sesión",
            "¿Estás seguro de que deseas cerrar tu sesión de Administrador?",
            "Sí, salir",
            "Cancelar"
        );

        if (confirm)
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
}
