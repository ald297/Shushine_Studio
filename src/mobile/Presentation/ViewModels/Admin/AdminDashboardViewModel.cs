using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ShushineStudio.Mobile.Data.Dtos;
using ShushineStudio.Mobile.Data.Services;
using ShushineStudio.Mobile.Domain.Entities;
using ShushineStudio.Mobile.Domain.Repositories;

namespace ShushineStudio.Mobile.Presentation.ViewModels.Admin;

/// <summary>
/// ViewModel del Dashboard Administrativo de Shushine Studio.
/// Conectado estrictamente a los endpoints reales de Spring Boot:
/// - GET /api/admin/dashboard (KPIs y métricas consolidadas)
/// - GET /api/citas/timeline (citas operativas del día)
/// Cero datos ficticios ni métricas simuladas.
/// </summary>
public partial class AdminDashboardViewModel : BaseViewModel
{
    private readonly IReservaRepository _reservaRepository;
    private readonly IAuthRepository _authRepository;
    private readonly ITokenStorageService _tokenStorageService;

    // Control de Acceso Estricto
    [ObservableProperty]
    private bool isUnauthorized = false;

    [ObservableProperty]
    private bool isAuthorized = true;

    // Encabezado
    [ObservableProperty]
    private string nombreAdmin = "Administrador";

    [ObservableProperty]
    private string fechaHoy = DateTime.Today.ToString("dddd, dd 'de' MMMM yyyy");

    // Métricas Reales del Backend (/api/admin/dashboard)
    [ObservableProperty]
    private long totalCitasHoy = 0;

    [ObservableProperty]
    private long totalCitasSemana = 0;

    [ObservableProperty]
    private decimal ingresosHoy = 0m;

    [ObservableProperty]
    private decimal ingresosMes = 0m;

    [ObservableProperty]
    private long citasEnProceso = 0;

    [ObservableProperty]
    private long citasCompletadas = 0;

    [ObservableProperty]
    private long citasCanceladas = 0;

    [ObservableProperty]
    private long estilistasActivos = 0;

    // Estados de Carga y Visualización
    [ObservableProperty]
    private bool metricasCargadas = false;

    [ObservableProperty]
    private bool estaVacio = false;

    [ObservableProperty]
    private ObservableCollection<Reserva> citasDelDia = new();

    public AdminDashboardViewModel(
        IReservaRepository reservaRepository,
        IAuthRepository authRepository,
        ITokenStorageService tokenStorageService)
    {
        _reservaRepository = reservaRepository;
        _authRepository = authRepository;
        _tokenStorageService = tokenStorageService;

        Title = "Dashboard";
    }

    [RelayCommand]
    public async Task LoadDashboardAsync()
    {
        if (IsBusy) return;

        try
        {
            IsBusy = true;
            ErrorMessage = string.Empty;

            // 1. Verificación Estricta de Rol Administrativo
            var rol = await _tokenStorageService.GetRoleAsync();
            var currentUser = await _authRepository.GetCurrentUserAsync();

            if (currentUser != null && !string.IsNullOrEmpty(currentUser.Rol))
            {
                rol = currentUser.Rol;
            }

            if (string.IsNullOrEmpty(rol) || !rol.ToUpperInvariant().Contains("ADMIN"))
            {
                IsUnauthorized = true;
                IsAuthorized = false;
                ErrorMessage = "Acceso denegado: Esta pantalla está reservada exclusivamente para personal con rol administrativo.";
                return;
            }

            IsUnauthorized = false;
            IsAuthorized = true;

            if (currentUser != null && !string.IsNullOrWhiteSpace(currentUser.NombreCompleto))
            {
                NombreAdmin = currentUser.NombreCompleto;
            }
            else if (currentUser != null && !string.IsNullOrWhiteSpace(currentUser.Nombre))
            {
                NombreAdmin = currentUser.Nombre;
            }

            // 2. Consulta de Métricas Consolidadas Reales del Servidor (GET /api/admin/dashboard)
            var metricas = await _reservaRepository.GetDashboardMetricasAsync();
            if (metricas != null)
            {
                TotalCitasHoy = metricas.TotalCitasHoy;
                TotalCitasSemana = metricas.TotalCitasSemana;
                IngresosHoy = metricas.IngresosHoy;
                IngresosMes = metricas.IngresosMes;
                CitasEnProceso = metricas.CitasEnProceso;
                CitasCompletadas = metricas.CitasCompletadas;
                CitasCanceladas = metricas.CitasCanceladas;
                EstilistasActivos = metricas.EstilistasActivos;
                MetricasCargadas = true;
            }
            else
            {
                // Si el microservicio devuelve vacío o sin conexión, se reportan en cero real
                MetricasCargadas = false;
            }

            // 3. Consulta de Citas del Día Reales del Servidor (GET /api/citas/timeline)
            var hoy = DateTime.Today;
            var citas = (await _reservaRepository.GetTimelineCitasAsync(hoy)).ToList();

            CitasDelDia.Clear();
            foreach (var c in citas)
            {
                CitasDelDia.Add(c);
            }

            EstaVacio = CitasDelDia.Count == 0;
        }
        catch (Exception ex)
        {
            ErrorMessage = "No se pudo sincronizar el dashboard con el servidor.";
            System.Diagnostics.Debug.WriteLine($"[AdminDashboardViewModel] Error: {ex.Message}");
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private async Task IrAAgendaAsync()
    {
        if (Shell.Current is AppShell appShell)
        {
            appShell.SwitchToAdminTab(1); // Índice 1 es la pestaña funcional Agenda en AdminTabBar
        }
        else
        {
            await Shell.Current.GoToAsync("//AdminTabs/TimelineAgendaPage");
        }
    }

    [RelayCommand]
    private async Task IrANotificacionesAsync()
    {
        await Shell.Current.GoToAsync("NotificationCenterPage");
    }

    [RelayCommand]
    private async Task IrAGestionCitasAsync()
    {
        await Shell.Current.GoToAsync("AdminAppointmentsPage");
    }

    [RelayCommand]
    private async Task IrAClientesAsync()
    {
        await Shell.Current.GoToAsync("AdminClientsPage");
    }

    [RelayCommand]
    private async Task IrAProfesionalesAsync()
    {
        await Shell.Current.GoToAsync("AdminProfessionalsPage");
    }

    [RelayCommand]
    private async Task IrAServiciosAsync()
    {
        await Shell.Current.GoToAsync("AdminServicesPage");
    }

    [RelayCommand]
    private async Task IrAProductosAsync()
    {
        await Shell.Current.GoToAsync("AdminProductsPage");
    }

    [RelayCommand]
    private async Task IrAMensajesAsync()
    {
        await Shell.Current.GoToAsync("AdminMessagesPage");
    }

    [RelayCommand]
    private async Task IrASolicitudesAsync()
    {
        await Shell.Current.GoToAsync("AdminRequestsPage");
    }

    [RelayCommand]
    private async Task IrAResenasAsync()
    {
        await Shell.Current.GoToAsync("AdminReviewsPage");
    }

    [RelayCommand]
    private async Task IrAReportesAsync()
    {
        await Shell.Current.GoToAsync("AdminReportsPage");
    }

    [RelayCommand]
    private async Task IrAConfiguracionAsync()
    {
        await Shell.Current.GoToAsync("AdminSettingsPage");
    }

    [RelayCommand]
    private async Task NuevaCitaWalkInAsync()
    {
        await Shell.Current.GoToAsync("AdminCreateAppointmentPage");
    }

    [RelayCommand]
    private async Task IrADetalleCitaAsync(Reserva reserva)
    {
        if (reserva == null) return;

        await Shell.Current.GoToAsync("AdminAppointmentDetailPage", new Dictionary<string, object>
        {
            { "reserva", reserva },
            { "citaId", reserva.Id }
        });
    }

    [RelayCommand]
    private async Task CerrarSesionAdminAsync()
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
