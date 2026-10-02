using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ShushineStudio.Mobile.Domain.Entities;
using ShushineStudio.Mobile.Domain.Repositories;

namespace ShushineStudio.Mobile.Presentation.ViewModels.Admin;

/// <summary>
/// ViewModel para el Detalle Administrativo y Operativo de una Cita.
/// Permite gestionar los estados reales de la cita en Spring Boot:
/// - Confirmar ('Confirmed')
/// - Iniciar Tratamiento ('InProgress')
/// - Finalizar / Completar ('Completed')
/// - Cancelar Cita ('Cancelled' con motivo)
/// Muestra claramente las funciones no disponibles en backend (Reprogramación).
/// </summary>
public partial class AdminAppointmentDetailViewModel : BaseViewModel, IQueryAttributable
{
    private readonly IReservaRepository _reservaRepository;

    [ObservableProperty]
    private Reserva? reserva;

    [ObservableProperty]
    private long citaId;

    [ObservableProperty]
    private string codigoCita = string.Empty;

    [ObservableProperty]
    private string clienteNombre = string.Empty;

    [ObservableProperty]
    private string clienteTelefono = string.Empty;

    [ObservableProperty]
    private string servicioNombre = string.Empty;

    [ObservableProperty]
    private string estilistaNombre = string.Empty;

    [ObservableProperty]
    private string fechaCita = string.Empty;

    [ObservableProperty]
    private string horarioTexto = string.Empty;

    [ObservableProperty]
    private string estado = "Confirmed";

    [ObservableProperty]
    private string estadoDisplay = "Confirmada";

    [ObservableProperty]
    private string estadoColor = "#2E7D32";

    [ObservableProperty]
    private string totalFormateado = "$0.00";

    [ObservableProperty]
    private string subtotalFormateado = "$0.00";

    [ObservableProperty]
    private string ivaFormateado = "$0.00";

    [ObservableProperty]
    private string metodoPago = "Efectivo";

    [ObservableProperty]
    private string estadoPago = "Pending";

    [ObservableProperty]
    private string notas = string.Empty;

    // Habilitadores de acciones según el estado actual
    [ObservableProperty]
    private bool puedeConfirmar;

    [ObservableProperty]
    private bool puedeIniciar;

    [ObservableProperty]
    private bool puedeCompletar;

    [ObservableProperty]
    private bool puedeCancelar;

    [ObservableProperty]
    private bool citaFinalizada;

    public AdminAppointmentDetailViewModel(IReservaRepository reservaRepository)
    {
        _reservaRepository = reservaRepository;
        Title = "Detalle de Cita";
    }

    public async void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (query.TryGetValue("reserva", out var resObj) && resObj is Reserva r)
        {
            CargarDatos(r);
        }
        else if (query.TryGetValue("citaId", out var idObj) && idObj is long id)
        {
            CitaId = id;
            await CargarCitaPorIdAsync(id);
        }
    }

    private async Task CargarCitaPorIdAsync(long id)
    {
        if (IsBusy) return;

        try
        {
            IsBusy = true;
            var c = await _reservaRepository.ObtenerCitaPorIdAsync(id);
            if (c != null)
            {
                CargarDatos(c);
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = "No se pudo cargar la información de la cita.";
            System.Diagnostics.Debug.WriteLine($"[AdminAppointmentDetailViewModel] Error: {ex.Message}");
        }
        finally
        {
            IsBusy = false;
        }
    }

    private void CargarDatos(Reserva r)
    {
        Reserva = r;
        CitaId = r.Id;
        CodigoCita = r.CodigoCita;
        ClienteNombre = !string.IsNullOrWhiteSpace(r.ClienteNombre) ? r.ClienteNombre : "Cliente General";
        ClienteTelefono = !string.IsNullOrWhiteSpace(r.ClienteTelefono) ? r.ClienteTelefono : "No registrado";
        ServicioNombre = r.ServicioNombre;
        EstilistaNombre = r.EstilistaNombre;
        FechaCita = r.FechaHoraInicio != DateTime.MinValue
            ? r.FechaHoraInicio.ToString("dddd, dd 'de' MMMM yyyy")
            : r.FechaCita;
        HorarioTexto = r.HorarioTexto;
        Estado = r.Estado;
        EstadoDisplay = r.EstadoDisplay;
        EstadoColor = r.EstadoColor;
        TotalFormateado = r.TotalFormateado;
        SubtotalFormateado = r.SubtotalFormateado;
        IvaFormateado = r.IvaFormateado;
        MetodoPago = !string.IsNullOrWhiteSpace(r.MetodoPagoPreferente) ? r.MetodoPagoPreferente : "Efectivo";
        EstadoPago = !string.IsNullOrWhiteSpace(r.EstadoPago) ? r.EstadoPago : "Pending";
        Notas = !string.IsNullOrWhiteSpace(r.Notas) ? r.Notas : "Sin notas adicionales para esta cita.";

        ActualizarReglasDeEstado();
    }

    [ObservableProperty]
    private bool puedeRegistrarPago;

    public bool EstaPagado => EstadoPago?.Equals("Paid", StringComparison.OrdinalIgnoreCase) == true
                           || EstadoPago?.Equals("Pagado", StringComparison.OrdinalIgnoreCase) == true
                           || EstadoPago?.Equals("Aprobado", StringComparison.OrdinalIgnoreCase) == true;

    public string EstadoPagoTexto => EstaPagado ? "Pagado" : "Pago pendiente";
    public string EstadoPagoColor => EstaPagado ? "#2E7D32" : "#E65100";
    public string MetodoPagoTexto => MetodoPago;

    private void ActualizarReglasDeEstado()
    {
        var est = Estado?.ToUpperInvariant() ?? string.Empty;

        // Reglas de negocio del backend Spring Boot
        CitaFinalizada = est is "COMPLETED" or "COMPLETADA" or "CANCELLED" or "CANCELADA";

        PuedeConfirmar = est is "PENDING" or "PENDIENTE";
        PuedeIniciar = est is "CONFIRMED" or "CONFIRMADA";
        PuedeCompletar = est is "INPROGRESS" or "IN_PROGRESS" or "EN_PROCESO";
        PuedeCancelar = !CitaFinalizada;
        PuedeRegistrarPago = !EstaPagado && !CitaFinalizada;

        OnPropertyChanged(nameof(EstaPagado));
        OnPropertyChanged(nameof(EstadoPagoTexto));
        OnPropertyChanged(nameof(EstadoPagoColor));
        OnPropertyChanged(nameof(MetodoPagoTexto));
    }

    [RelayCommand]
    private async Task RegistrarPagoEfectivoAsync()
    {
        if (EstaPagado)
        {
            if (Application.Current?.MainPage != null)
            {
                await Application.Current.MainPage.DisplayAlert("Pago Registrado", "Esta cita ya figura como pagada.", "OK");
            }
            return;
        }

        if (Application.Current?.MainPage == null) return;

        var confirmar = await Application.Current.MainPage.DisplayAlert(
            "Registrar Pago",
            $"¿Confirmas que recibiste el pago en Efectivo por {TotalFormateado}?",
            "Sí, Registrar",
            "Cancelar"
        );

        if (!confirmar) return;

        try
        {
            IsBusy = true;
            var monto = Reserva?.Total ?? 0m;
            var ok = await _reservaRepository.RegistrarPagoAsync(CitaId, monto, "Efectivo");
            if (ok)
            {
                EstadoPago = "Paid";
                MetodoPago = "Efectivo";
                ActualizarReglasDeEstado();

                await Application.Current.MainPage.DisplayAlert(
                    "Pago Exitoso",
                    $"El pago en Efectivo ha sido registrado correctamente para la cita {CodigoCita}. Estado: Pagado — Efectivo.",
                    "Aceptar"
                );
            }
            else
            {
                await Application.Current.MainPage.DisplayAlert(
                    "Error",
                    "No se pudo registrar el pago en el servidor. Intente nuevamente.",
                    "OK"
                );
            }
        }
        catch (Exception ex)
        {
            await Application.Current.MainPage.DisplayAlert("Error", ex.Message, "OK");
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private async Task ConfirmarCitaAsync()
    {
        await CambiarEstadoAsync("Confirmed", "Cita confirmada por administración.");
    }

    [RelayCommand]
    private async Task IniciarTratamientoAsync()
    {
        await CambiarEstadoAsync("InProgress", "Cita iniciada en sala de atención.");
    }

    [RelayCommand]
    private async Task FinalizarCitaAsync()
    {
        await CambiarEstadoAsync("Completed", "Tratamiento completado satisfactoriamente.");
    }

    [RelayCommand]
    private async Task CancelarCitaAsync()
    {
        if (Application.Current?.MainPage == null) return;

        var confirmar = await Application.Current.MainPage.DisplayAlert(
            "Cancelar Cita",
            "¿Estás seguro de que deseas anular esta reserva en el sistema?",
            "Sí, Cancelar",
            "No"
        );

        if (!confirmar) return;

        var motivo = await Application.Current.MainPage.DisplayPromptAsync(
            "Motivo de Cancelación",
            "Ingresa el motivo (opcional):",
            "Aceptar",
            "Omitir",
            "Cancelada por administración"
        );

        await CambiarEstadoAsync("Cancelled", motivo);
    }

    private async Task CambiarEstadoAsync(string nuevoEstado, string? motivo)
    {
        if (IsBusy || CitaId <= 0) return;

        try
        {
            IsBusy = true;
            ErrorMessage = string.Empty;

            var resultado = await _reservaRepository.CambiarEstadoCitaAsync(CitaId, nuevoEstado, motivo);
            if (resultado != null)
            {
                CargarDatos(resultado);

                if (Application.Current?.MainPage != null)
                {
                    await Application.Current.MainPage.DisplayAlert(
                        "Operación Exitosa",
                        $"La cita {CodigoCita} ha sido actualizada a '{EstadoDisplay}'.",
                        "Aceptar"
                    );
                }
            }
            else
            {
                ErrorMessage = "El servidor no pudo actualizar el estado de la cita.";
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = ex.Message;
            System.Diagnostics.Debug.WriteLine($"[AdminAppointmentDetailViewModel] Error: {ex.Message}");
        }
        finally
        {
            IsBusy = false;
        }
    }


    [RelayCommand]
    private async Task RegresarAsync()
    {
        await Shell.Current.GoToAsync("..");
    }
}
