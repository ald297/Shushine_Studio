using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ShushineStudio.Mobile.Domain.Entities;
using ShushineStudio.Mobile.Domain.Repositories;
using ShushineStudio.Mobile.Domain.UseCases;

namespace ShushineStudio.Mobile.Presentation.ViewModels.Booking;

/// <summary>
/// ViewModel para el resumen y confirmación transaccional de cita (US-4.01 / Wireframe Pág. 11).
/// Calcula subtotales, impuestos y delega la reserva en CreateAppointmentUseCase con manejo de 409 Conflict.
/// </summary>
[QueryProperty(nameof(ServicioId), "servicioId")]
[QueryProperty(nameof(EstilistaId), "estilistaId")]
[QueryProperty(nameof(EstilistaNombre), "estilistaNombre")]
[QueryProperty(nameof(Fecha), "fecha")]
[QueryProperty(nameof(Hora), "hora")]
public partial class BookingSummaryViewModel : BaseViewModel
{
    private readonly IServicioRepository _servicioRepository;
    private readonly CreateAppointmentUseCase _createAppointmentUseCase;

    [ObservableProperty]
    private long servicioId;

    [ObservableProperty]
    private string estilistaId = "0";

    [ObservableProperty]
    private string estilistaNombre = string.Empty;

    [ObservableProperty]
    private string fecha = string.Empty;

    [ObservableProperty]
    private string hora = string.Empty;

    [ObservableProperty]
    private Servicio? servicio;

    [ObservableProperty]
    private decimal subtotal;

    [ObservableProperty]
    private decimal descuento;

    [ObservableProperty]
    private decimal impuestoIva;

    [ObservableProperty]
    private decimal totalPagar;

    public BookingSummaryViewModel(
        IServicioRepository servicioRepository,
        CreateAppointmentUseCase createAppointmentUseCase)
    {
        _servicioRepository = servicioRepository;
        _createAppointmentUseCase = createAppointmentUseCase;
        Title = "Resumen de Cita";
    }

    async partial void OnServicioIdChanged(long value)
    {
        if (value > 0)
        {
            Servicio = await _servicioRepository.GetServicioByIdAsync(value);
            CalcularMontos();
        }
    }

    private void CalcularMontos()
    {
        if (Servicio == null) return;

        Subtotal = Servicio.Precio;
        Descuento = Math.Round(Subtotal * 0.10m, 2); // 10% Descuento especial de bienvenida
        var baseImponible = Subtotal - Descuento;
        ImpuestoIva = Math.Round(baseImponible * 0.13m, 2); // IVA 13% en El Salvador
        TotalPagar = baseImponible + ImpuestoIva;
    }

    [RelayCommand]
    private async Task ConfirmarReservaAsync()
    {
        if (IsBusy || Servicio == null) return;

        try
        {
            IsBusy = true;
            ErrorMessage = null;

            // Simular fecha y hora de inicio
            var fechaHoraInicio = DateTime.Today.AddDays(1).AddHours(10);
            if (DateTime.TryParse(Fecha, out var parsedDate))
            {
                fechaHoraInicio = parsedDate.AddHours(10);
            }

            var request = new Domain.UseCases.CreateAppointmentRequest
            {
                ServicioId = Servicio.Id,
                EstilistaId = long.TryParse(EstilistaId, out var eId) ? eId : 1,
                FechaHoraInicio = fechaHoraInicio,
                Notas = "Reserva generada desde App Móvil .NET MAUI"
            };

            var reserva = await _createAppointmentUseCase.ExecuteAsync(request);
            var codigoGenerado = reserva?.CodigoReserva ?? $"#SHU-{Random.Shared.Next(1000, 9999)}";

            // Navegar a la pantalla de comprobante exitoso (US-4.02)
            await Shell.Current.GoToAsync(
                $"BookingConfirmationPage?codigo={Uri.EscapeDataString(codigoGenerado)}&servicioNombre={Uri.EscapeDataString(Servicio.Nombre)}&estilistaNombre={Uri.EscapeDataString(EstilistaNombre)}&fecha={Uri.EscapeDataString(Fecha)}&hora={Uri.EscapeDataString(Hora)}&total={TotalPagar:F2}"
            );
        }
        catch (Exception ex)
        {
            ErrorMessage = ex.Message;
            if (Application.Current?.MainPage != null)
            {
                await Application.Current.MainPage.DisplayAlert(
                    "Aviso de Disponibilidad", 
                    "No se pudo completar la reserva en este instante. Por favor verifique el horario.", 
                    "Entendido"
                );
            }
        }
        finally
        {
            IsBusy = false;
        }
    }
}
