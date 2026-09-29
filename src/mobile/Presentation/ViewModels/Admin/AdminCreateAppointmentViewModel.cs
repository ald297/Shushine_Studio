using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ShushineStudio.Mobile.Data.Dtos;
using ShushineStudio.Mobile.Domain.Entities;
using ShushineStudio.Mobile.Domain.Repositories;

namespace ShushineStudio.Mobile.Presentation.ViewModels.Admin;

/// <summary>
/// ViewModel para la creación de citas presenciales / Walk-in desde el panel administrativo.
/// Conectado al endpoint oficial POST /api/citas/walkin en Spring Boot.
/// </summary>
public partial class AdminCreateAppointmentViewModel : BaseViewModel
{
    private readonly IServicioRepository _servicioRepository;
    private readonly IEstilistaRepository _estilistaRepository;
    private readonly IReservaRepository _reservaRepository;

    [ObservableProperty]
    private string nombreCliente = string.Empty;

    [ObservableProperty]
    private string telefonoCliente = string.Empty;

    [ObservableProperty]
    private ObservableCollection<Servicio> servicios = new();

    [ObservableProperty]
    private Servicio? servicioSeleccionado;

    [ObservableProperty]
    private ObservableCollection<Estilista> estilistas = new();

    [ObservableProperty]
    private Estilista? estilistaSeleccionado;

    [ObservableProperty]
    private DateTime fechaCita = DateTime.Today;

    [ObservableProperty]
    private TimeSpan horaInicio = new(9, 0, 0);

    [ObservableProperty]
    private string metodoPago = "Efectivo";

    [ObservableProperty]
    private string notas = string.Empty;

    [ObservableProperty]
    private bool isConflict = false;

    public List<string> MetodosPagoDisponibles { get; } = new()
    {
        "Efectivo",
        "Tarjeta",
        "Transferencia"
    };

    public AdminCreateAppointmentViewModel(
        IServicioRepository servicioRepository,
        IEstilistaRepository estilistaRepository,
        IReservaRepository reservaRepository)
    {
        _servicioRepository = servicioRepository;
        _estilistaRepository = estilistaRepository;
        _reservaRepository = reservaRepository;

        Title = "Nueva Cita";
    }

    [RelayCommand]
    public async Task CargarCatalogosAsync()
    {
        if (IsBusy) return;

        try
        {
            IsBusy = true;
            ErrorMessage = string.Empty;
            IsConflict = false;

            // Cargar servicios reales
            var listaServicios = (await _servicioRepository.GetServiciosAsync()).Where(s => s.Activo).ToList();
            Servicios.Clear();
            foreach (var s in listaServicios)
            {
                Servicios.Add(s);
            }
            if (Servicios.Count > 0 && ServicioSeleccionado == null)
            {
                ServicioSeleccionado = Servicios[0];
            }

            // Cargar estilistas reales
            var listaEstilistas = (await _estilistaRepository.GetEstilistasAsync()).Where(e => e.Activo).ToList();
            Estilistas.Clear();
            foreach (var e in listaEstilistas)
            {
                Estilistas.Add(e);
            }
            if (Estilistas.Count > 0 && EstilistaSeleccionado == null)
            {
                EstilistaSeleccionado = Estilistas[0];
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = "No se pudieron cargar los catálogos del salón.";
            System.Diagnostics.Debug.WriteLine($"[AdminCreateAppointmentViewModel] Error: {ex.Message}");
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private async Task GuardarCitaAsync()
    {
        if (IsBusy) return;

        if (string.IsNullOrWhiteSpace(NombreCliente))
        {
            ErrorMessage = "Por favor ingresa el nombre de la clienta.";
            return;
        }

        if (ServicioSeleccionado == null)
        {
            ErrorMessage = "Por favor selecciona un servicio de belleza.";
            return;
        }

        if (EstilistaSeleccionado == null)
        {
            ErrorMessage = "Por favor selecciona una profesional.";
            return;
        }

        try
        {
            IsBusy = true;
            ErrorMessage = string.Empty;
            IsConflict = false;

            var horaStr = $"{HoraInicio.Hours:D2}:{HoraInicio.Minutes:D2}";
            var dto = new CitaWalkinRequestDto
            {
                NombreCliente = NombreCliente.Trim(),
                TelefonoCliente = !string.IsNullOrWhiteSpace(TelefonoCliente) ? TelefonoCliente.Trim() : null,
                EstilistaId = (int)EstilistaSeleccionado.Id,
                FechaCita = FechaCita.ToString("yyyy-MM-dd"),
                HoraInicio = horaStr,
                ServicioIds = new List<int> { (int)ServicioSeleccionado.Id },
                MetodoPagoPreferente = MetodoPago,
                Notas = !string.IsNullOrWhiteSpace(Notas) ? Notas.Trim() : "Cita presencial registrada en recepción."
            };

            var nuevaCita = await _reservaRepository.CrearCitaWalkinAsync(dto);

            if (nuevaCita != null)
            {
                if (Application.Current?.MainPage != null)
                {
                    await Application.Current.MainPage.DisplayAlert(
                        "Cita Creada",
                        $"Cita {nuevaCita.CodigoCita} para {NombreCliente} registrada exitosamente para las {horaStr}.",
                        "Aceptar"
                    );
                }

                await Shell.Current.GoToAsync("..");
            }
        }
        catch (InvalidOperationException ex)
        {
            // Conflicto de disponibilidad (HTTP 409)
            IsConflict = true;
            ErrorMessage = ex.Message;
        }
        catch (Exception ex)
        {
            ErrorMessage = ex.Message;
            System.Diagnostics.Debug.WriteLine($"[AdminCreateAppointmentViewModel] Error: {ex.Message}");
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
