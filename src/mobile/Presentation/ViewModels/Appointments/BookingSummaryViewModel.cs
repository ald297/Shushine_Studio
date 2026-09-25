using System.Collections.ObjectModel;
using System.Globalization;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ShushineStudio.Mobile.Domain.Entities;
using ShushineStudio.Mobile.Domain.Repositories;
using ShushineStudio.Mobile.Domain.UseCases;

namespace ShushineStudio.Mobile.Presentation.ViewModels.Appointments;

[QueryProperty(nameof(ServicioIdString), "servicioId")]
[QueryProperty(nameof(EstilistaIdString), "estilistaId")]
[QueryProperty(nameof(FechaString), "fecha")]
[QueryProperty(nameof(HoraInicio), "horaInicio")]
public partial class BookingSummaryViewModel : BaseViewModel
{
    private readonly IServicioRepository _servicioRepository;
    private readonly IEstilistaRepository _estilistaRepository;
    private readonly CreateAppointmentUseCase _createAppointmentUseCase;

    [ObservableProperty]
    private string? servicioIdString;

    [ObservableProperty]
    private string? estilistaIdString;

    [ObservableProperty]
    private string? fechaString;

    [ObservableProperty]
    private string horaInicio = string.Empty;

    [ObservableProperty]
    private string horaFin = string.Empty;

    [ObservableProperty]
    private long servicioId;

    [ObservableProperty]
    private long estilistaId;

    [ObservableProperty]
    private DateTime fechaCita = DateTime.Today;

    [ObservableProperty]
    private Servicio? servicio;

    [ObservableProperty]
    private Estilista? estilista;

    [ObservableProperty]
    private decimal subtotal;

    [ObservableProperty]
    private decimal iva;

    [ObservableProperty]
    private decimal total;

    [ObservableProperty]
    private string subtotalFormateado = "$0.00";

    [ObservableProperty]
    private string ivaFormateado = "$0.00";

    [ObservableProperty]
    private string totalFormateado = "$0.00";

    [ObservableProperty]
    private string horarioCompleto = string.Empty;

    [ObservableProperty]
    private ObservableCollection<string> metodosPago = new()
    {
        "Efectivo",
        "Tarjeta de Crédito / Débito",
        "Transferencia / QR"
    };

    [ObservableProperty]
    private string metodoPagoSeleccionado = "Efectivo";

    [ObservableProperty]
    private string? notas;

    [ObservableProperty]
    private bool reservaExitosa;

    [ObservableProperty]
    private Reserva? reservaCreada;

    [ObservableProperty]
    private string codigoCitaCreada = string.Empty;

    public bool HasErrorMessage => !string.IsNullOrEmpty(ErrorMessage);

    public BookingSummaryViewModel(
        IServicioRepository servicioRepository,
        IEstilistaRepository estilistaRepository,
        CreateAppointmentUseCase createAppointmentUseCase)
    {
        _servicioRepository = servicioRepository;
        _estilistaRepository = estilistaRepository;
        _createAppointmentUseCase = createAppointmentUseCase;
        Title = "Resumen de Cita";
    }

    partial void OnServicioIdStringChanged(string? value) => VerificarYRefrescar();
    partial void OnEstilistaIdStringChanged(string? value) => VerificarYRefrescar();
    partial void OnFechaStringChanged(string? value) => VerificarYRefrescar();
    partial void OnHoraInicioChanged(string value) => VerificarYRefrescar();

    private void VerificarYRefrescar()
    {
        if (!string.IsNullOrWhiteSpace(ServicioIdString) &&
            !string.IsNullOrWhiteSpace(EstilistaIdString) &&
            !string.IsNullOrWhiteSpace(FechaString) &&
            !string.IsNullOrWhiteSpace(HoraInicio))
        {
            _ = LoadResumenAsync();
        }
    }

    [RelayCommand]
    public async Task LoadResumenAsync()
    {
        if (IsBusy) return;

        try
        {
            IsBusy = true;
            ErrorMessage = null;
            ReservaExitosa = false;

            // 1. Parsear IDs numéricos y fecha
            if (long.TryParse(ServicioIdString, out var sId))
                ServicioId = sId;

            if (long.TryParse(EstilistaIdString, out var eId))
                EstilistaId = eId;

            if (DateTime.TryParseExact(FechaString, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var dt))
                FechaCita = dt;
            else if (DateTime.TryParse(FechaString, out var dtFallback))
                FechaCita = dtFallback;

            // 2. Obtener datos del servicio y estilista
            Servicio = await _servicioRepository.GetServicioByIdAsync(ServicioId);
            Estilista = await _estilistaRepository.GetEstilistaByIdAsync(EstilistaId);

            // 3. Cálculos de desglose financiero (13% IVA El Salvador)
            if (Servicio != null)
            {
                Subtotal = Servicio.Precio;
                Iva = Math.Round(Subtotal * 0.13m, 2);
                Total = Subtotal + Iva;

                SubtotalFormateado = $"${Subtotal:N2}";
                IvaFormateado = $"${Iva:N2}";
                TotalFormateado = $"${Total:N2}";

                // 4. Calcular hora fin estimada
                if (TimeSpan.TryParse(HoraInicio, out var tsInicio))
                {
                    var tsFin = tsInicio.Add(TimeSpan.FromMinutes(Servicio.DuracionMinutos));
                    HoraFin = tsFin.ToString(@"hh\:mm");
                }
                else
                {
                    HoraFin = HoraInicio;
                }

                HorarioCompleto = $"{FechaCita:dd/MM/yyyy} • {HoraInicio} - {HoraFin}";
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
    public async Task ConfirmarReservaAsync()
    {
        if (IsBusy) return;

        try
        {
            IsBusy = true;
            ErrorMessage = null;

            // Mapeo seguro del método de pago para el backend
            var metodoBackend = "Efectivo";
            if (MetodoPagoSeleccionado.Contains("Tarjeta"))
                metodoBackend = "Tarjeta";
            else if (MetodoPagoSeleccionado.Contains("Transferencia") || MetodoPagoSeleccionado.Contains("QR"))
                metodoBackend = "Transferencia";

            var serviciosIds = new List<int> { (int)ServicioId };

            // Invocación del Caso de Uso transaccional
            var nuevaReserva = await _createAppointmentUseCase.ExecuteAsync(
                EstilistaId,
                FechaCita,
                HoraInicio,
                serviciosIds,
                Notas,
                metodoBackend
            );

            if (nuevaReserva != null)
            {
                ReservaCreada = nuevaReserva;
                CodigoCitaCreada = nuevaReserva.CodigoCita;
                ReservaExitosa = true;

                if (Application.Current?.MainPage != null)
                {
                    await Application.Current.MainPage.DisplayAlert(
                        "¡Cita Confirmada!",
                        $"Tu reservación ha sido registrada exitosamente.\n\nCódigo: {nuevaReserva.CodigoCita}\nFecha: {nuevaReserva.FechaCita}\nHora: {nuevaReserva.HoraInicio}\nTotal a Pagar: {nuevaReserva.TotalFormateado}",
                        "Ver Mis Citas"
                    );
                }

                // Redirigir a la pestaña de Mis Citas
                await Shell.Current.GoToAsync("//MainTabs/MyAppointmentsPage");
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
    private async Task VolverAsync()
    {
        await Shell.Current.GoToAsync("..");
    }

    [RelayCommand]
    private async Task IrAMisCitasAsync()
    {
        await Shell.Current.GoToAsync("//MainTabs/MyAppointmentsPage");
    }
}
