using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ShushineStudio.Mobile.Domain.Repositories;

namespace ShushineStudio.Mobile.Presentation.ViewModels.Admin;

/// <summary>
/// ViewModel para el Perfil Administrativo y Parámetros Operativos del Salón (US-5.01 / Panel Ejecutivo).
/// Ofrece al gerente visibilidad de identidad, configuración operativa, métricas globales y cierre de sesión seguro.
/// </summary>
public partial class AdminProfileViewModel : BaseViewModel
{
    private readonly IAuthRepository _authRepository;
    private readonly IServicioRepository _servicioRepository;
    private readonly IEstilistaRepository _estilistaRepository;
    private readonly IReservaRepository _reservaRepository;

    [ObservableProperty]
    private string nombreGerente = "Alex Fernando Alfaro";

    [ObservableProperty]
    private string emailGerente = "admin@shunshinestudio.com";

    [ObservableProperty]
    private string rol = "ADMINISTRADOR GENERAL";

    [ObservableProperty]
    private string sucursal = "Sede Central • ESFE AGAPE";

    [ObservableProperty]
    private string horarioOperativo = "Lunes a Sábado • 09:00 AM - 08:30 PM";

    [ObservableProperty]
    private string politicaFiscal = "13% IVA Incluido • Normativa El Salvador";

    [ObservableProperty]
    private string backendInfo = "Spring Boot 3.3.3 & Supabase Cloud";

    [ObservableProperty]
    private int totalServicios;

    [ObservableProperty]
    private int totalEstilistas;

    [ObservableProperty]
    private int totalCitasHoy;

    // Toast de Notificación Boutique Flotante
    [ObservableProperty]
    private bool isToastVisible;

    [ObservableProperty]
    private string toastTitulo = string.Empty;

    [ObservableProperty]
    private string toastMensaje = string.Empty;

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

        Title = "Perfil de Administración";
        _ = CargarDatosAdminAsync();
    }

    [RelayCommand]
    public async Task CargarDatosAdminAsync()
    {
        if (IsBusy) return;

        try
        {
            IsBusy = true;

            var user = await _authRepository.GetCurrentUserAsync();
            if (user != null)
            {
                NombreGerente = !string.IsNullOrWhiteSpace(user.NombreCompleto) ? user.NombreCompleto : "Administración Shunshine Studio";
                EmailGerente = !string.IsNullOrWhiteSpace(user.Email) ? user.Email : "admin@shunshinestudio.com";
                Rol = user.Rol == "ADMIN" ? "ADMINISTRADOR GENERAL" : user.Rol;
            }

            var servicios = await _servicioRepository.GetServiciosAsync();
            TotalServicios = servicios.Count();

            var estilistas = await _estilistaRepository.GetTodosEstilistasAsync();
            TotalEstilistas = estilistas.Count();

            var citas = await _reservaRepository.GetTodasCitasAdminAsync();
            TotalCitasHoy = citas.Count(c => c.FechaHoraInicio.Date == DateTime.Today);
        }
        catch (Exception ex)
        {
            ErrorMessage = ex.Message;
        }
        finally
        {
            IsBusy = false;
        }
    }

    public void MostrarToast(string titulo, string mensaje)
    {
        ToastTitulo = titulo;
        ToastMensaje = mensaje;
        IsToastVisible = true;
        _ = Task.Run(async () =>
        {
            await Task.Delay(3500);
            MainThread.BeginInvokeOnMainThread(() => IsToastVisible = false);
        });
    }

    [RelayCommand]
    private void CerrarToast()
    {
        IsToastVisible = false;
    }

    [RelayCommand]
    private async Task CerrarSesionAsync()
    {
        if (Application.Current?.MainPage == null) return;

        bool confirm = await Application.Current.MainPage.DisplayAlert(
            "Cerrar Sesión de Administrador",
            "¿Estás seguro de que deseas salir del panel de administración de Shunshine Studio?",
            "Cerrar Sesión",
            "Cancelar"
        );

        if (confirm)
        {
            await _authRepository.LogoutAsync();
            if (Shell.Current is AppShell appShell)
            {
                appShell.SwitchToLogin();
            }
            await Shell.Current.GoToAsync("//LoginPage");
        }
    }

    [RelayCommand]
    private async Task IrACatalogoAsync()
    {
        await Shell.Current.GoToAsync("//AdminTabs/AdminCatalogPage");
    }

    [RelayCommand]
    private async Task IrAEstilistasAsync()
    {
        await Shell.Current.GoToAsync("//AdminTabs/AdminEstilistasPage");
    }

    [RelayCommand]
    private async Task IrAAgendaAsync()
    {
        await Shell.Current.GoToAsync("//AdminTabs/TimelineAgendaPage");
    }
}
