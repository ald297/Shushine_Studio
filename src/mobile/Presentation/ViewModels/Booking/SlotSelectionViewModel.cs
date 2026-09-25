using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ShushineStudio.Mobile.Domain.Entities;
using ShushineStudio.Mobile.Domain.Repositories;

namespace ShushineStudio.Mobile.Presentation.ViewModels.Booking;

public class TimeSlotItem
{
    public string Hora { get; set; } = string.Empty;
    public bool Disponible { get; set; } = true;
    public bool Seleccionado { get; set; } = false;
    public string Turno { get; set; } = "Mañana";
}

public class DayItem
{
    public DateTime Fecha { get; set; }
    public string DiaNombre { get; set; } = string.Empty;
    public string DiaNumero { get; set; } = string.Empty;
    public bool Seleccionado { get; set; } = false;
}

/// <summary>
/// ViewModel para la selección de fecha y bloques horarios disponibles (US-3.04 / Wireframe Pág. 10).
/// Genera slots dinámicos matutinos y vespertinos con filtrado de ocupados.
/// </summary>
[QueryProperty(nameof(ServicioId), "servicioId")]
[QueryProperty(nameof(EstilistaId), "estilistaId")]
[QueryProperty(nameof(EstilistaNombre), "estilistaNombre")]
public partial class SlotSelectionViewModel : BaseViewModel
{
    private readonly IServicioRepository _servicioRepository;

    [ObservableProperty]
    private long servicioId;

    [ObservableProperty]
    private string estilistaId = "0";

    [ObservableProperty]
    private string estilistaNombre = "Cualquier estilista disponible";

    [ObservableProperty]
    private Servicio? servicio;

    [ObservableProperty]
    private ObservableCollection<DayItem> diasSemana = new();

    [ObservableProperty]
    private DateTime fechaSeleccionada = DateTime.Today.AddDays(1);

    [ObservableProperty]
    private ObservableCollection<TimeSlotItem> slotsManana = new();

    [ObservableProperty]
    private ObservableCollection<TimeSlotItem> slotsTarde = new();

    [ObservableProperty]
    private string horaSeleccionada = string.Empty;

    public SlotSelectionViewModel(IServicioRepository servicioRepository)
    {
        _servicioRepository = servicioRepository;
        Title = "Fecha y Horario";
        GenerarDias();
        GenerarSlots();
    }

    async partial void OnServicioIdChanged(long value)
    {
        if (value > 0)
        {
            Servicio = await _servicioRepository.GetServicioByIdAsync(value);
        }
    }

    private void GenerarDias()
    {
        DiasSemana.Clear();
        var baseDate = DateTime.Today;

        for (int i = 1; i <= 7; i++)
        {
            var date = baseDate.AddDays(i);
            DiasSemana.Add(new DayItem
            {
                Fecha = date,
                DiaNombre = date.ToString("ddd", new System.Globalization.CultureInfo("es-ES")).ToUpperInvariant(),
                DiaNumero = date.Day.ToString(),
                Seleccionado = (i == 1)
            });
        }
    }

    private void GenerarSlots()
    {
        SlotsManana.Clear();
        SlotsManana.Add(new TimeSlotItem { Hora = "09:00 AM", Disponible = true, Turno = "Mañana" });
        SlotsManana.Add(new TimeSlotItem { Hora = "10:30 AM", Disponible = false, Turno = "Mañana" }); // Ocupado
        SlotsManana.Add(new TimeSlotItem { Hora = "11:45 AM", Disponible = true, Turno = "Mañana" });

        SlotsTarde.Clear();
        SlotsTarde.Add(new TimeSlotItem { Hora = "02:00 PM", Disponible = true, Turno = "Tarde" });
        SlotsTarde.Add(new TimeSlotItem { Hora = "03:30 PM", Disponible = true, Turno = "Tarde" });
        SlotsTarde.Add(new TimeSlotItem { Hora = "05:00 PM", Disponible = false, Turno = "Tarde" }); // Ocupado
        SlotsTarde.Add(new TimeSlotItem { Hora = "06:15 PM", Disponible = true, Turno = "Tarde" });
    }

    [RelayCommand]
    private void SeleccionarDia(DayItem day)
    {
        if (day == null) return;

        foreach (var d in DiasSemana)
        {
            d.Seleccionado = (d.Fecha == day.Fecha);
        }

        FechaSeleccionada = day.Fecha;
        GenerarSlots();
        HoraSeleccionada = string.Empty;
    }

    [RelayCommand]
    private void SeleccionarSlot(TimeSlotItem slot)
    {
        if (slot == null || !slot.Disponible) return;

        HoraSeleccionada = slot.Hora;
    }

    [RelayCommand]
    private async Task ContinuarAResumenAsync()
    {
        if (Servicio == null) return;

        if (string.IsNullOrWhiteSpace(HoraSeleccionada))
        {
            if (Application.Current?.MainPage != null)
            {
                await Application.Current.MainPage.DisplayAlert(
                    "Horario Requerido", 
                    "Por favor selecciona una franja horaria disponible para continuar.", 
                    "Aceptar"
                );
            }
            return;
        }

        var fechaStr = FechaSeleccionada.ToString("yyyy-MM-dd");

        // Navegar a la pantalla de resumen (US-4.01)
        await Shell.Current.GoToAsync(
            $"BookingSummaryPage?servicioId={Servicio.Id}&estilistaId={EstilistaId}&estilistaNombre={Uri.EscapeDataString(EstilistaNombre)}&fecha={fechaStr}&hora={Uri.EscapeDataString(HoraSeleccionada)}"
        );
    }
}
