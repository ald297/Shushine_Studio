using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ShushineStudio.Mobile.Domain.Entities;
using ShushineStudio.Mobile.Domain.Repositories;

namespace ShushineStudio.Mobile.Presentation.ViewModels.Admin;

/// <summary>
/// ViewModel para el registro rápido de clientas presenciales sin cita (US-5.03 / Wireframe Pág. 16).
/// Permite a recepción ingresar una atención espontánea ocupando slots libres en la agenda diaria.
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
    private string notas = "Cliente presencial sin cita en salón";

    // Toast Boutique Flotante (Reemplaza los alertas nativos grises)
    [ObservableProperty]
    private bool isToastVisible;

    [ObservableProperty]
    private string toastTitulo = string.Empty;

    [ObservableProperty]
    private string toastMensaje = string.Empty;

    public void MostrarToast(string titulo, string mensaje)
    {
        ToastTitulo = titulo;
        ToastMensaje = mensaje;
        IsToastVisible = true;
        _ = Task.Run(async () =>
        {
            await Task.Delay(3500);
            MainThread.BeginInvokeOnMainThread(() => IsToastVisible = false);
        });
    }

    [RelayCommand]
    private void CerrarToast()
    {
        IsToastVisible = false;
    }

    public WalkInViewModel(
        IServicioRepository servicioRepository,
        IEstilistaRepository estilistaRepository,
        IReservaRepository reservaRepository)
    {
        _servicioRepository = servicioRepository;
        _estilistaRepository = estilistaRepository;
        _reservaRepository = reservaRepository;
        Title = "Atención Sin Cita";
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
            MostrarToast("Nombre Requerido", "Por favor ingresa el nombre de la clienta en salón.");
            return;
        }

        if (ServicioSeleccionado == null)
        {
            MostrarToast("Servicio Requerido", "Selecciona el tratamiento que se realizará.");
            return;
        }

        var estilistaId = EstilistaSeleccionado?.Id ?? 1;
        var horaStr = $"{HoraAtencion.Hours:D2}:{HoraAtencion.Minutes:D2}";
        var notasCompletas = $"Sin Cita: {NombreCliente.Trim()} (Tel: {Telefono?.Trim()}). {Notas?.Trim()}";

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
                MostrarToast(
                    "Atención Registrada",
                    $"Cita {reserva.CodigoCita} para {NombreCliente} programada para hoy a las {horaStr}."
                );

                await Task.Delay(1500);
                await Shell.Current.GoToAsync("..");
            }
            else
            {
                MostrarToast("Error", "No se pudo registrar la atención. Intenta de nuevo.");
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = ex.Message;
            MostrarToast("Error al Agendar", ex.Message);
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
}
