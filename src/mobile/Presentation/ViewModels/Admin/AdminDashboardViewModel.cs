using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ShushineStudio.Mobile.Domain.Entities;
using ShushineStudio.Mobile.Domain.Repositories;

namespace ShushineStudio.Mobile.Presentation.ViewModels.Admin;

/// <summary>
/// ViewModel del Dashboard administrativo operativo (US-5.01 / Wireframe Pág. 15).
/// Expone KPIs del día: citas totales, capacidad ocupada, ingresos proyectados y reservas recientes.
/// </summary>
public partial class AdminDashboardViewModel : BaseViewModel
{
    private readonly IReservaRepository _reservaRepository;

    [ObservableProperty]
    private int citasHoy;

    [ObservableProperty]
    private int capacidadTotal = 20; // Total de slots del día configurables

    [ObservableProperty]
    private double porcentajeOcupado;

    [ObservableProperty]
    private decimal ingresosProyectados;

    [ObservableProperty]
    private int citasPendientes;

    [ObservableProperty]
    private int citasCompletadas;

    [ObservableProperty]
    private int citasCanceladas;

    [ObservableProperty]
    private ObservableCollection<Reserva> reservasRecientes = new();

    [ObservableProperty]
    private string fechaHoy = DateTime.Today.ToString("dddd, dd 'de' MMMM 'de' yyyy");

    public AdminDashboardViewModel(IReservaRepository reservaRepository)
    {
        _reservaRepository = reservaRepository;
        Title = "Dashboard Operativo";
        _ = LoadDashboardAsync();
    }

    [RelayCommand]
    public async Task LoadDashboardAsync()
    {
        if (IsBusy) return;

        try
        {
            IsBusy = true;
            ErrorMessage = null;

            var todasLasCitas = await _reservaRepository.GetMisCitasAsync();
            var hoy = DateTime.Today;
            var citasDelDia = todasLasCitas
                .Where(r => r.FechaHoraInicio.Date == hoy)
                .ToList();

            CitasHoy = citasDelDia.Count;
            CitasPendientes = citasDelDia.Count(r => r.Estado == "PENDIENTE");
            CitasCompletadas = citasDelDia.Count(r => r.Estado == "COMPLETADA");
            CitasCanceladas = citasDelDia.Count(r => r.Estado == "CANCELADA");

            PorcentajeOcupado = CapacidadTotal > 0
                ? Math.Round((double)CitasHoy / CapacidadTotal * 100, 1)
                : 0;

            IngresosProyectados = citasDelDia
                .Where(r => r.Estado != "CANCELADA")
                .Sum(r => r.Total);

            ReservasRecientes.Clear();
            foreach (var reserva in todasLasCitas.Take(5))
                ReservasRecientes.Add(reserva);
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

    [RelayCommand]
    private async Task VerAgendaAsync()
    {
        await Shell.Current.GoToAsync("TimelineAgendaPage");
    }

    [RelayCommand]
    private async Task NuevaReservaWalkInAsync()
    {
        await Shell.Current.GoToAsync("//MainTabs/CatalogPage");
    }
}
