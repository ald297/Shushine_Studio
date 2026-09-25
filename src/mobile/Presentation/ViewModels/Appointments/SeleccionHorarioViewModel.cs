using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ShushineStudio.Mobile.Domain.Entities;
using ShushineStudio.Mobile.Domain.Repositories;
using ShushineStudio.Mobile.Domain.UseCases;

namespace ShushineStudio.Mobile.Presentation.ViewModels.Appointments;

[QueryProperty(nameof(ServicioIdString), "servicioId")]
public partial class SeleccionHorarioViewModel : BaseViewModel
{
    private readonly IServicioRepository _servicioRepository;
    private readonly GetEstilistasUseCase _getEstilistasUseCase;
    private readonly GetDisponibilidadUseCase _getDisponibilidadUseCase;

    [ObservableProperty]
    private string? servicioIdString;

    [ObservableProperty]
    private long servicioId;

    [ObservableProperty]
    private Servicio? servicioSeleccionado;

    [ObservableProperty]
    private ObservableCollection<Estilista> estilistas = new();

    [ObservableProperty]
    private Estilista? estilistaSeleccionado;

    [ObservableProperty]
    private DateTime fechaSeleccionada = DateTime.Today;

    [ObservableProperty]
    private DateTime fechaMinima = DateTime.Today;

    [ObservableProperty]
    private DateTime fechaMaxima = DateTime.Today.AddDays(30);

    [ObservableProperty]
    private ObservableCollection<FranjaHoraria> franjasHorarias = new();

    [ObservableProperty]
    private FranjaHoraria? franjaSeleccionada;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasMensajeEstado))]
    private string? mensajeEstado;

    public bool HasMensajeEstado => !string.IsNullOrEmpty(MensajeEstado);

    [ObservableProperty]
    private bool hasFranjas;

    [ObservableProperty]
    private bool canContinuar;

    public SeleccionHorarioViewModel(
        IServicioRepository servicioRepository,
        GetEstilistasUseCase getEstilistasUseCase,
        GetDisponibilidadUseCase getDisponibilidadUseCase)
    {
        _servicioRepository = servicioRepository;
        _getEstilistasUseCase = getEstilistasUseCase;
        _getDisponibilidadUseCase = getDisponibilidadUseCase;
        Title = "Seleccionar Horario";
    }

    partial void OnServicioIdStringChanged(string? value)
    {
        if (long.TryParse(value, out var id))
        {
            ServicioId = id;
            _ = LoadDatosInicialesAsync();
        }
    }

    partial void OnEstilistaSeleccionadoChanged(Estilista? value)
    {
        if (value != null && ServicioId > 0 && !IsBusy)
        {
            _ = LoadDisponibilidadAsync();
        }
    }

    partial void OnFechaSeleccionadaChanged(DateTime value)
    {
        if (EstilistaSeleccionado != null && ServicioId > 0 && !IsBusy)
        {
            _ = LoadDisponibilidadAsync();
        }
    }

    [RelayCommand]
    public async Task LoadDatosInicialesAsync()
    {
        if (ServicioId <= 0) return;

        try
        {
            IsBusy = true;
            ErrorMessage = null;
            MensajeEstado = null;

            // 1. Obtener detalles del servicio seleccionado
            ServicioSeleccionado = await _servicioRepository.GetServicioByIdAsync(ServicioId);

            // 2. Obtener lista de estilistas activos
            var estilistasLista = await _getEstilistasUseCase.ExecuteAsync();
            Estilistas.Clear();
            foreach (var est in estilistasLista.Where(e => e.Activo))
            {
                Estilistas.Add(est);
            }

            // Seleccionar por defecto el primer estilista si no hay selección
            if (Estilistas.Count > 0 && EstilistaSeleccionado == null)
            {
                EstilistaSeleccionado = Estilistas[0];
            }

            // 3. Consultar disponibilidad con el estilista y fecha seleccionada
            if (EstilistaSeleccionado != null)
            {
                await ConsultarDisponibilidadInternaAsync();
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
    public async Task LoadDisponibilidadAsync()
    {
        if (EstilistaSeleccionado == null || ServicioId <= 0) return;

        try
        {
            IsBusy = true;
            ErrorMessage = null;
            await ConsultarDisponibilidadInternaAsync();
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

    private async Task ConsultarDisponibilidadInternaAsync()
    {
        FranjaSeleccionada = null;
        CanContinuar = false;
        MensajeEstado = null;

        var disponibilidad = await _getDisponibilidadUseCase.ExecuteAsync(
            EstilistaSeleccionado!.Id,
            FechaSeleccionada,
            ServicioId);

        FranjasHorarias.Clear();
        if (disponibilidad?.Franjas != null && disponibilidad.Franjas.Count > 0)
        {
            foreach (var franja in disponibilidad.Franjas)
            {
                FranjasHorarias.Add(franja);
            }
            HasFranjas = true;
        }
        else
        {
            HasFranjas = false;
            MensajeEstado = "No hay horarios disponibles para esta fecha. Intente seleccionando otro día o estilista.";
        }
    }

    [RelayCommand]
    private void SelectFranja(FranjaHoraria franja)
    {
        if (franja == null) return;

        if (!franja.Disponible)
        {
            MensajeEstado = !string.IsNullOrEmpty(franja.MotivoNoDisponible)
                ? franja.MotivoNoDisponible
                : "El horario seleccionado no está disponible.";
            return;
        }

        FranjaSeleccionada = franja;
        MensajeEstado = null;
        CanContinuar = ServicioSeleccionado != null && EstilistaSeleccionado != null && FranjaSeleccionada != null;
    }

    [RelayCommand]
    private void SelectEstilista(Estilista estilista)
    {
        if (estilista == null || estilista.Id == EstilistaSeleccionado?.Id) return;
        EstilistaSeleccionado = estilista;
    }

    [RelayCommand]
    private async Task ContinuarAsync()
    {
        if (!CanContinuar || ServicioSeleccionado == null || EstilistaSeleccionado == null || FranjaSeleccionada == null)
        {
            if (Application.Current?.MainPage != null)
            {
                await Application.Current.MainPage.DisplayAlert(
                    "Selección Incompleta", 
                    "Por favor, asegúrate de seleccionar una estilista, fecha y un turno disponible.", 
                    "Aceptar"
                );
            }
            return;
        }

        var fechaStr = FechaSeleccionada.ToString("yyyy-MM-dd");
        var route = $"BookingSummaryPage?servicioId={ServicioSeleccionado.Id}&estilistaId={EstilistaSeleccionado.Id}&fecha={fechaStr}&horaInicio={FranjaSeleccionada.HoraInicio}";
        await Shell.Current.GoToAsync(route);
    }
}
