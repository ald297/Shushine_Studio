using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ShushineStudio.Mobile.Data.Services;
using ShushineStudio.Mobile.Domain.Entities;
using ShushineStudio.Mobile.Domain.Repositories;

namespace ShushineStudio.Mobile.Presentation.ViewModels.Admin;

/// <summary>
/// ViewModel principal para la pantalla de Gestión de Citas del Administrador.
/// Permite consultar todas las citas reales del salón, filtrar por estado real del backend,
/// buscar por cliente o código y navegar al detalle operativo de cada cita.
/// </summary>
public partial class AdminAppointmentsViewModel : BaseViewModel
{
    private readonly IReservaRepository _reservaRepository;
    private readonly IAuthRepository _authRepository;
    private readonly ITokenStorageService _tokenStorageService;

    [ObservableProperty]
    private bool isUnauthorized = false;

    [ObservableProperty]
    private bool isAuthorized = true;

    [ObservableProperty]
    private string textoBusqueda = string.Empty;

    [ObservableProperty]
    private string estadoFiltro = "Todas";

    [ObservableProperty]
    private ObservableCollection<Reserva> citasCompletas = new();

    [ObservableProperty]
    private ObservableCollection<Reserva> citasFiltradas = new();

    [ObservableProperty]
    private bool estaVacio = false;

    [ObservableProperty]
    private int totalCitas = 0;

    public AdminAppointmentsViewModel(
        IReservaRepository reservaRepository,
        IAuthRepository authRepository,
        ITokenStorageService tokenStorageService)
    {
        _reservaRepository = reservaRepository;
        _authRepository = authRepository;
        _tokenStorageService = tokenStorageService;

        Title = "Gestión de Citas";
    }

    [RelayCommand]
    public async Task LoadCitasAsync()
    {
        if (IsBusy) return;

        try
        {
            IsBusy = true;
            ErrorMessage = string.Empty;

            // 1. Verificación Estricta de Rol ADMIN
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
                ErrorMessage = "Acceso denegado: Esta pantalla está reservada para personal administrativo.";
                return;
            }

            IsUnauthorized = false;
            IsAuthorized = true;

            // 2. Consulta de Citas Reales del Salón desde /api/citas
            var lista = (await _reservaRepository.ObtenerTodasCitasAdminPaginadasAsync(0, 100)).ToList();

            CitasCompletas.Clear();
            foreach (var c in lista)
            {
                CitasCompletas.Add(c);
            }

            AplicarFiltros();
        }
        catch (Exception ex)
        {
            ErrorMessage = "No se pudieron sincronizar las citas con el servidor.";
            System.Diagnostics.Debug.WriteLine($"[AdminAppointmentsViewModel] Error: {ex.Message}");
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private void FiltrarPorEstado(string estado)
    {
        EstadoFiltro = estado;
        AplicarFiltros();
    }

    partial void OnTextoBusquedaChanged(string value)
    {
        AplicarFiltros();
    }

    private void AplicarFiltros()
    {
        var query = CitasCompletas.AsEnumerable();

        // 1. Filtro por Estado Real del Backend
        if (!string.IsNullOrEmpty(EstadoFiltro) && EstadoFiltro != "Todas")
        {
            var estNorm = EstadoFiltro.ToUpperInvariant();
            query = query.Where(c =>
            {
                var estCita = c.Estado?.ToUpperInvariant() ?? string.Empty;
                return estNorm switch
                {
                    "CONFIRMADAS" => estCita is "CONFIRMED" or "CONFIRMADA",
                    "EN PROCESO" => estCita is "INPROGRESS" or "IN_PROGRESS" or "EN_PROCESO",
                    "COMPLETADAS" => estCita is "COMPLETED" or "COMPLETADA",
                    "CANCELADAS" => estCita is "CANCELLED" or "CANCELADA",
                    "PENDIENTES" => estCita is "PENDING" or "PENDIENTE",
                    _ => true
                };
            });
        }

        // 2. Filtro por Búsqueda (Cliente, Código, Estilista, Servicio)
        if (!string.IsNullOrWhiteSpace(TextoBusqueda))
        {
            var txt = TextoBusqueda.Trim().ToLowerInvariant();
            query = query.Where(c =>
                (c.ClienteNombre != null && c.ClienteNombre.ToLowerInvariant().Contains(txt)) ||
                (c.CodigoCita != null && c.CodigoCita.ToLowerInvariant().Contains(txt)) ||
                (c.EstilistaNombre != null && c.EstilistaNombre.ToLowerInvariant().Contains(txt)) ||
                (c.ServicioNombre != null && c.ServicioNombre.ToLowerInvariant().Contains(txt))
            );
        }

        CitasFiltradas.Clear();
        foreach (var item in query.OrderByDescending(c => c.FechaHoraInicio))
        {
            CitasFiltradas.Add(item);
        }

        TotalCitas = CitasFiltradas.Count;
        EstaVacio = CitasFiltradas.Count == 0;
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
    private async Task NuevaCitaWalkInAsync()
    {
        await Shell.Current.GoToAsync("AdminCreateAppointmentPage");
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
