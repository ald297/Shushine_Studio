using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ShushineStudio.Mobile.Domain.Entities;
using ShushineStudio.Mobile.Domain.Repositories;

namespace ShushineStudio.Mobile.Presentation.ViewModels.Admin;

/// <summary>
/// ViewModel de la agenda de citas en formato timeline por estilista (US-5.02 / Wireframe Pág. 16).
/// Permite a recepción ver la distribución horaria y agregar citas walk-in espontáneas.
/// </summary>
public partial class TimelineAgendaViewModel : BaseViewModel
{
    private readonly IReservaRepository _reservaRepository;

    [ObservableProperty]
    private ObservableCollection<Reserva> citasDelDia = new();

    [ObservableProperty]
    private DateTime fechaSeleccionada = DateTime.Today;

    [ObservableProperty]
    private string estilistaNombreFiltro = "Todas";

    // Estilistas disponibles para filtro de columna
    public List<string> Estilistas { get; } = new()
    {
        "Todas",
        "Sofía Ramos",
        "Valentina Gómez",
        "Camila Torres",
        "Recepción Walk-In"
    };

    // Bloques horarios de la jornada laboral del salón
    public List<string> BloquesHorarios { get; } = new()
    {
        "08:00", "08:30", "09:00", "09:30", "10:00", "10:30",
        "11:00", "11:30", "12:00", "12:30", "13:00", "13:30",
        "14:00", "14:30", "15:00", "15:30", "16:00", "16:30",
        "17:00", "17:30", "18:00"
    };

    public TimelineAgendaViewModel(IReservaRepository reservaRepository)
    {
        _reservaRepository = reservaRepository;
        Title = "Agenda del Día";
        _ = LoadAgendaAsync();
    }

    [RelayCommand]
    public async Task LoadAgendaAsync()
    {
        if (IsBusy) return;

        try
        {
            IsBusy = true;
            ErrorMessage = null;

            var todasLasCitas = await _reservaRepository.GetMisCitasAsync();

            CitasDelDia.Clear();
            var citasFiltradas = todasLasCitas
                .Where(r => r.FechaHoraInicio.Date == FechaSeleccionada.Date);

            if (EstilistaNombreFiltro != "Todas")
                citasFiltradas = citasFiltradas.Where(r => r.EstilistaNombre == EstilistaNombreFiltro);

            foreach (var cita in citasFiltradas.OrderBy(r => r.FechaHoraInicio))
                CitasDelDia.Add(cita);
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

    partial void OnFechaSeleccionadaChanged(DateTime value)
    {
        _ = LoadAgendaAsync();
    }

    partial void OnEstilistaNombreFiltroChanged(string value)
    {
        _ = LoadAgendaAsync();
    }

    [RelayCommand]
    private async Task CambiarEstadoReservaAsync(Reserva reserva)
    {
        if (reserva == null || Application.Current?.MainPage == null) return;

        // US-5.04: Modal de cambio de estado operativo
        var resultado = await Application.Current.MainPage.DisplayActionSheet(
            $"Estado de: {reserva.CodigoReserva}",
            "Cancelar",
            null,
            "✅ Marcar como Completada",
            "⏳ Marcar como En Curso",
            "❌ Cancelar Reserva"
        );

        if (resultado == null || resultado == "Cancelar") return;

        var nuevoEstado = resultado switch
        {
            "✅ Marcar como Completada" => "COMPLETADA",
            "⏳ Marcar como En Curso"   => "EN_CURSO",
            "❌ Cancelar Reserva"        => "CANCELADA",
            _                           => reserva.Estado
        };

        reserva.Estado = nuevoEstado;
        await Application.Current.MainPage.DisplayAlert(
            "Estado Actualizado",
            $"La reserva {reserva.CodigoReserva} fue marcada como {nuevoEstado}.",
            "Aceptar"
        );

        await LoadAgendaAsync();
    }

    [RelayCommand]
    private async Task AgregarWalkInAsync()
    {
        // US-5.03: Redirigir al flujo de reserva para crear una cita presencial espontánea
        await Shell.Current.GoToAsync("//MainTabs/CatalogPage");
    }

    [RelayCommand]
    private void DiaAnterior()
    {
        FechaSeleccionada = FechaSeleccionada.AddDays(-1);
    }

    [RelayCommand]
    private void DiaSiguiente()
    {
        FechaSeleccionada = FechaSeleccionada.AddDays(1);
    }
}
