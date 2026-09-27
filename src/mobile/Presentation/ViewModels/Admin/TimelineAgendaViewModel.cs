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

    private static readonly List<Reserva> AgendaOperativaBase = new()
    {
        new Reserva
        {
            Id = 301,
            CodigoCita = "#SHU-1024",
            ServicioNombre = "Balayage Iluminador & Gloss",
            EstilistaNombre = "Sofía Ramos",
            FechaHoraInicio = DateTime.Today.AddHours(9),
            FechaHoraFin = DateTime.Today.AddHours(11),
            Total = 65.00m,
            Estado = "COMPLETADA"
        },
        new Reserva
        {
            Id = 302,
            CodigoCita = "#SHU-1025",
            ServicioNombre = "Corte de Autor & Cepillado",
            EstilistaNombre = "Sofía Ramos",
            FechaHoraInicio = DateTime.Today.AddHours(11).AddMinutes(30),
            FechaHoraFin = DateTime.Today.AddHours(12).AddMinutes(15),
            Total = 25.00m,
            Estado = "EN_CURSO"
        },
        new Reserva
        {
            Id = 303,
            CodigoCita = "#SHU-1026",
            ServicioNombre = "Manicura Rusa & Esmaltado Semi",
            EstilistaNombre = "Valentina Gómez",
            FechaHoraInicio = DateTime.Today.AddHours(10),
            FechaHoraFin = DateTime.Today.AddHours(11),
            Total = 22.00m,
            Estado = "COMPLETADA"
        },
        new Reserva
        {
            Id = 304,
            CodigoCita = "#SHU-1027",
            ServicioNombre = "Pedicura Spa Rejuvenecedora",
            EstilistaNombre = "Valentina Gómez",
            FechaHoraInicio = DateTime.Today.AddHours(14),
            FechaHoraFin = DateTime.Today.AddHours(15),
            Total = 28.00m,
            Estado = "PENDIENTE"
        },
        new Reserva
        {
            Id = 305,
            CodigoCita = "#SHU-1028",
            ServicioNombre = "Lifting de Pestañas & Keratina",
            EstilistaNombre = "Camila Torres",
            FechaHoraInicio = DateTime.Today.AddHours(15).AddMinutes(30),
            FechaHoraFin = DateTime.Today.AddHours(16).AddMinutes(20),
            Total = 30.00m,
            Estado = "PENDIENTE"
        }
    };

    [RelayCommand]
    public async Task LoadAgendaAsync()
    {
        if (IsBusy) return;

        try
        {
            IsBusy = true;
            ErrorMessage = null;

            var citasRemotas = await _reservaRepository.GetMisCitasAsync();
            var todasLasCitas = citasRemotas.ToList();

            // Incluir citas operativas si la fecha coincide
            foreach (var citaMock in AgendaOperativaBase)
            {
                if (!todasLasCitas.Any(c => c.Id == citaMock.Id))
                {
                    todasLasCitas.Add(citaMock);
                }
            }

            CitasDelDia.Clear();
            var citasFiltradas = todasLasCitas
                .Where(r => r.FechaHoraInicio.Date == FechaSeleccionada.Date);

            if (!string.IsNullOrWhiteSpace(EstilistaNombreFiltro) && EstilistaNombreFiltro != "Todas")
            {
                citasFiltradas = citasFiltradas.Where(r => 
                    r.EstilistaNombre.Contains(EstilistaNombreFiltro, StringComparison.OrdinalIgnoreCase));
            }

            foreach (var cita in citasFiltradas.OrderBy(r => r.FechaHoraInicio))
            {
                CitasDelDia.Add(cita);
            }
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
    private void FiltrarPorEstilista(string estilista)
    {
        if (string.IsNullOrWhiteSpace(estilista)) return;
        EstilistaNombreFiltro = estilista;
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
        // US-5.03: Navegar al formulario de registro rápido de cliente presencial
        await Shell.Current.GoToAsync("WalkInPage");
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
