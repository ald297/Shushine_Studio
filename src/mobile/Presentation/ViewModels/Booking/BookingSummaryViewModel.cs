using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ShushineStudio.Mobile.Domain.Entities;
using ShushineStudio.Mobile.Domain.Repositories;
using ShushineStudio.Mobile.Domain.UseCases;

namespace ShushineStudio.Mobile.Presentation.ViewModels.Booking;

/// <summary>
/// ViewModel para el resumen y confirmación transaccional de cita (US-4.01 / Wireframe Pág. 11).
/// Refleja únicamente los valores reales del catálogo y backend sin descuentos ni impuestos inventados.
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
    private decimal totalPagar;

    public bool HasErrorMessage => !string.IsNullOrWhiteSpace(ErrorMessage);

    public string TotalPagarFormateado => $"${TotalPagar:N2}";

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

        // Auditoría de datos reales: el total a pagar corresponde al precio real del servicio
        TotalPagar = Servicio.Precio;
        OnPropertyChanged(nameof(TotalPagarFormateado));
    }

    [RelayCommand]
    private async Task VolverAsync()
    {
        await Shell.Current.GoToAsync("..");
    }

    [RelayCommand]
    private async Task ConfirmarReservaAsync()
    {
        if (IsBusy || Servicio == null) return;

        try
        {
            IsBusy = true;
            ErrorMessage = null;
            OnPropertyChanged(nameof(HasErrorMessage));

            var fechaHoraInicio = DateTime.Today.AddDays(1).AddHours(10);
            if (DateTime.TryParse($"{Fecha} {Hora}", out var fullParsed))
            {
                fechaHoraInicio = fullParsed;
            }
            else if (DateTime.TryParse(Fecha, out var parsedDate))
            {
                fechaHoraInicio = parsedDate.AddHours(10);
            }

            var estilistaIdLong = long.TryParse(EstilistaId, out var eId) && eId > 0 ? (long?)eId : 1;

            var reserva = await _createAppointmentUseCase.ExecuteAsync(
                Servicio.Id,
                estilistaIdLong,
                fechaHoraInicio,
                "Reserva generada desde App Móvil .NET MAUI"
            );

            // Obtener el código o identificador real devuelto por la API sin fabricar códigos ficticios
            string codigoGenerado = string.Empty;
            if (reserva != null)
            {
                if (!string.IsNullOrWhiteSpace(reserva.CodigoCita))
                    codigoGenerado = reserva.CodigoCita;
                else if (reserva.Id > 0)
                    codigoGenerado = $"Cita #{reserva.Id}";
            }

            // Navegar a la pantalla de comprobante exitoso (US-4.02)
            await Shell.Current.GoToAsync(
                $"BookingConfirmationPage?codigo={Uri.EscapeDataString(codigoGenerado)}&servicioNombre={Uri.EscapeDataString(Servicio.Nombre)}&estilistaNombre={Uri.EscapeDataString(EstilistaNombre)}&fecha={Uri.EscapeDataString(Fecha)}&hora={Uri.EscapeDataString(Hora)}&total={TotalPagar:F2}"
            );
        }
        catch (Exception ex)
        {
            ErrorMessage = ex.Message;
            OnPropertyChanged(nameof(HasErrorMessage));
            if (Application.Current?.MainPage != null)
            {
                await Application.Current.MainPage.DisplayAlert(
                    "Aviso de Reserva", 
                    ex.Message, 
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
