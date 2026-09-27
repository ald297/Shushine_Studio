using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ShushineStudio.Mobile.Domain.Entities;
using ShushineStudio.Mobile.Domain.Repositories;

namespace ShushineStudio.Mobile.Presentation.ViewModels.Admin;

/// <summary>
/// ViewModel para el registro rápido de clientes presenciales Walk-in (US-5.03 / Wireframe Pág. 16).
/// Permite a recepción ingresar una cita espontánea ocupando slots libres en la agenda diaria.
/// </summary>
public partial class WalkInViewModel : BaseViewModel
{
    private readonly IServicioRepository _servicioRepository;
    private readonly IEstilistaRepository _estilistaRepository;
    private readonly IReservaRepository _reservaRepository;

    [ObservableProperty]
    private string nombreCliente = string.Empty;

    [ObservableProperty]
    private string telefono = string.Empty;

    [ObservableProperty]
    private ObservableCollection<Servicio> servicios = new();

    [ObservableProperty]
    private Servicio? servicioSeleccionado;

    [ObservableProperty]
    private ObservableCollection<Estilista> estilistas = new();

    [ObservableProperty]
    private Estilista? estilistaSeleccionado;

    [ObservableProperty]
    private TimeSpan horaAtencion = DateTime.Now.TimeOfDay;

    [ObservableProperty]
    private string notas = "Cliente presencial Walk-in";

    public WalkInViewModel(
        IServicioRepository servicioRepository,
        IEstilistaRepository estilistaRepository,
        IReservaRepository reservaRepository)
    {
        _servicioRepository = servicioRepository;
        _estilistaRepository = estilistaRepository;
        _reservaRepository = reservaRepository;
        Title = "Nueva Cita Walk-In";
        _ = CargarDatosInicialesAsync();
    }

    [RelayCommand]
    public async Task CargarDatosInicialesAsync()
    {
        if (IsBusy) return;

        try
        {
            IsBusy = true;
            ErrorMessage = null;

            // 1. Cargar servicios
            var servList = await _servicioRepository.GetServiciosAsync();
            Servicios.Clear();
            foreach (var s in servList.Where(s => s.Activo))
            {
                Servicios.Add(s);
            }
            if (Servicios.Count > 0)
                ServicioSeleccionado = Servicios[0];

            // 2. Cargar estilistas
            var estList = await _estilistaRepository.GetEstilistasAsync();
            Estilistas.Clear();
            foreach (var e in estList.Where(e => e.Activo))
            {
                Estilistas.Add(e);
            }
            if (Estilistas.Count > 0)
                EstilistaSeleccionado = Estilistas[0];
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
    private async Task RegistrarWalkInAsync()
    {
        if (IsBusy) return;

        if (string.IsNullOrWhiteSpace(NombreCliente))
        {
            await MostrarAlertaAsync("Nombre Requerido", "Por favor ingresa el nombre del cliente presencial.");
            return;
        }

        if (ServicioSeleccionado == null)
        {
            await MostrarAlertaAsync("Servicio Requerido", "Selecciona el tratamiento que se realizará.");
            return;
        }

        var estilistaId = EstilistaSeleccionado?.Id ?? 1;
        var horaStr = $"{HoraAtencion.Hours:D2}:{HoraAtencion.Minutes:D2}";
        var notasCompletas = $"Walk-in: {NombreCliente.Trim()} (Tel: {Telefono?.Trim()}). {Notas?.Trim()}";

        try
        {
            IsBusy = true;
            ErrorMessage = null;

            var reserva = await _reservaRepository.CrearReservaAsync(
                estilistaId: estilistaId,
                fechaCita: DateTime.Today,
                horaInicio: horaStr,
                servicioIds: new List<int> { (int)ServicioSeleccionado.Id },
                notas: notasCompletas,
                metodoPago: "Efectivo"
            );

            if (reserva != null)
            {
                await MostrarAlertaAsync(
                    "Cita Registrada con Éxito",
                    $"La cita presencial {reserva.CodigoCita} para {NombreCliente} ha sido agendada para hoy a las {horaStr}."
                );

                // Volver a la agenda timeline
                await Shell.Current.GoToAsync("..");
            }
            else
            {
                await MostrarAlertaAsync("Error", "No se pudo registrar la cita. Por favor intenta de nuevo.");
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = ex.Message;
            await MostrarAlertaAsync("Error al Agendar", ex.Message);
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private async Task CancelarAsync()
    {
        await Shell.Current.GoToAsync("..");
    }

    private static async Task MostrarAlertaAsync(string title, string message)
    {
        if (Application.Current?.MainPage != null)
        {
            await Application.Current.MainPage.DisplayAlert(title, message, "Aceptar");
        }
    }
}
