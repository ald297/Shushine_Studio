using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ShushineStudio.Mobile.Domain.Entities;
using ShushineStudio.Mobile.Domain.Repositories;

namespace ShushineStudio.Mobile.Presentation.ViewModels.Admin;

/// <summary>
/// Modelo reactivo para el chip selector de estilista en la Agenda.
/// </summary>
public class EstilistaChipItem : ObservableObject
{
    public int? Id { get; set; }
    public string Nombre { get; set; } = string.Empty;

    private bool _isSelected;
    public bool IsSelected
    {
        get => _isSelected;
        set => SetProperty(ref _isSelected, value);
    }
}

/// <summary>
/// ViewModel de la Agenda Operativa del Salón.
/// Conectado directamente a GET /api/citas/timeline?fecha=YYYY-MM-DD del backend.
/// Cero datos simulados ni citas mock.
/// </summary>
public partial class TimelineAgendaViewModel : BaseViewModel, IQueryAttributable
{
    private readonly IReservaRepository _reservaRepository;
    private readonly IEstilistaRepository _estilistaRepository;

    [ObservableProperty]
    private DateTime fechaSeleccionada = DateTime.Today;

    [ObservableProperty]
    private string fechaTexto = string.Empty;

    [ObservableProperty]
    private int? estilistaSeleccionadoId = null;

    [ObservableProperty]
    private string estadoSeleccionado = "Todos";

    [ObservableProperty]
    private ObservableCollection<EstilistaChipItem> estilistasFiltro = new();

    [ObservableProperty]
    private ObservableCollection<Reserva> citasDelDia = new();

    [ObservableProperty]
    private ObservableCollection<Reserva> citasFiltradas = new();

    [ObservableProperty]
    private bool estaVacio = false;

    [ObservableProperty]
    private int totalCitas = 0;

    public TimelineAgendaViewModel(
        IReservaRepository reservaRepository,
        IEstilistaRepository estilistaRepository)
    {
        _reservaRepository = reservaRepository;
        _estilistaRepository = estilistaRepository;

        Title = "Agenda";
        ActualizarFechaTexto();
    }

    public async void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (query.TryGetValue("estilistaId", out var idObj))
        {
            if (idObj is int intId)
            {
                EstilistaSeleccionadoId = intId;
            }
            else if (idObj is long longId)
            {
                EstilistaSeleccionadoId = (int)longId;
            }
            else if (int.TryParse(idObj?.ToString(), out var parsedId))
            {
                EstilistaSeleccionadoId = parsedId;
            }

            await CargarEstilistasAsync();
            await LoadAgendaAsync();
        }
    }

    public async Task InicializarAsync()
    {
        await CargarEstilistasAsync();
        await LoadAgendaAsync();
    }

    private void ActualizarFechaTexto()
    {
        FechaTexto = FechaSeleccionada.ToString("dddd, dd 'de' MMMM yyyy");
    }

    public async Task CargarEstilistasAsync()
    {
        try
        {
            var estilistas = (await _estilistaRepository.GetEstilistasAsync()).ToList();
            EstilistasFiltro.Clear();

            // Opción "Todos"
            EstilistasFiltro.Add(new EstilistaChipItem
            {
                Id = null,
                Nombre = "Todos",
                IsSelected = EstilistaSeleccionadoId == null
            });

            foreach (var est in estilistas)
            {
                EstilistasFiltro.Add(new EstilistaChipItem
                {
                    Id = (int)est.Id,
                    Nombre = est.NombreCompleto,
                    IsSelected = EstilistaSeleccionadoId == (int)est.Id
                });
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[TimelineAgendaViewModel] Error al cargar estilistas: {ex.Message}");
        }
    }

    [RelayCommand]
    public async Task LoadAgendaAsync()
    {
        if (IsBusy) return;

        try
        {
            IsBusy = true;
            ErrorMessage = string.Empty;
            ActualizarFechaTexto();

            // Consulta de citas reales para la fecha y estilista seleccionados
            var citas = (await _reservaRepository.GetTimelineCitasAsync(FechaSeleccionada, EstilistaSeleccionadoId)).ToList();

            CitasDelDia.Clear();
            foreach (var c in citas)
            {
                CitasDelDia.Add(c);
            }

            AplicarFiltroEstado();
        }
        catch (Exception ex)
        {
            ErrorMessage = "No se pudo sincronizar la agenda con el servidor.";
            System.Diagnostics.Debug.WriteLine($"[TimelineAgendaViewModel] Error: {ex.Message}");
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private async Task DiaAnteriorAsync()
    {
        FechaSeleccionada = FechaSeleccionada.AddDays(-1);
        await LoadAgendaAsync();
    }

    [RelayCommand]
    private async Task DiaSiguienteAsync()
    {
        FechaSeleccionada = FechaSeleccionada.AddDays(1);
        await LoadAgendaAsync();
    }

    [RelayCommand]
    private async Task IrAHoyAsync()
    {
        FechaSeleccionada = DateTime.Today;
        await LoadAgendaAsync();
    }

    [RelayCommand]
    private async Task FiltrarPorEstilistaAsync(EstilistaChipItem chip)
    {
        if (chip == null) return;

        EstilistaSeleccionadoId = chip.Id;

        foreach (var item in EstilistasFiltro)
        {
            item.IsSelected = item.Id == chip.Id;
        }

        await LoadAgendaAsync();
    }

    [RelayCommand]
    private void FiltrarPorEstado(string estado)
    {
        EstadoSeleccionado = estado;
        AplicarFiltroEstado();
    }

    private void AplicarFiltroEstado()
    {
        CitasFiltradas.Clear();

        var query = CitasDelDia.AsEnumerable();

        if (!string.IsNullOrEmpty(EstadoSeleccionado) && EstadoSeleccionado != "Todos")
        {
            var estNorm = EstadoSeleccionado.ToUpperInvariant();
            query = query.Where(c =>
            {
                var estCita = c.Estado?.ToUpperInvariant() ?? string.Empty;
                return estNorm switch
                {
                    "CONFIRMADAS" => estCita is "CONFIRMED" or "CONFIRMADA",
                    "EN PROCESO" => estCita is "INPROGRESS" or "IN_PROGRESS" or "EN_PROCESO",
                    "COMPLETADAS" => estCita is "COMPLETED" or "COMPLETADA",
                    "CANCELADAS" => estCita is "CANCELLED" or "CANCELADA",
                    _ => true
                };
            });
        }

        foreach (var c in query.OrderBy(c => c.HoraInicio))
        {
            CitasFiltradas.Add(c);
        }

        TotalCitas = CitasFiltradas.Count;
        EstaVacio = CitasFiltradas.Count == 0;
    }

    [RelayCommand]
    private async Task IrADetalleCitaAsync(Reserva reserva)
    {
        if (reserva == null) return;

        // Navegación al detalle administrativo pasando la información real
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
    private async Task IrAGestionCitasAsync()
    {
        await Shell.Current.GoToAsync("AdminAppointmentsPage");
    }

    [RelayCommand]
    private async Task RegresarAsync()
    {
        await Shell.Current.GoToAsync("..");
    }
}
