using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ShushineStudio.Mobile.Domain.Entities;
using ShushineStudio.Mobile.Domain.Repositories;

namespace ShushineStudio.Mobile.Presentation.ViewModels.Booking;

/// <summary>
/// ViewModel para la selección de estilista o auto-asignación (US-3.03 / Wireframe Pág. 9).
/// Permite elegir un profesional específico o la opción 'Cualquiera disponible'.
/// </summary>
[QueryProperty(nameof(ServicioId), "servicioId")]
public partial class StylistSelectionViewModel : BaseViewModel
{
    private readonly IServicioRepository _servicioRepository;
    private readonly IEstilistaRepository _estilistaRepository;

    [ObservableProperty]
    private long servicioId;

    [ObservableProperty]
    private Servicio? servicio;

    [ObservableProperty]
    private ObservableCollection<Estilista> estilistas = new();

    [ObservableProperty]
    private Estilista? estilistaSeleccionado;

    [ObservableProperty]
    private long estilistaSeleccionadoId;

    [ObservableProperty]
    private bool esCualquieraDisponible = true;

    public StylistSelectionViewModel(
        IServicioRepository servicioRepository,
        IEstilistaRepository estilistaRepository)
    {
        _servicioRepository = servicioRepository;
        _estilistaRepository = estilistaRepository;
        Title = "Selecciona tu Estilista";
        _ = CargarEstilistasAsync();
    }

    async partial void OnServicioIdChanged(long value)
    {
        if (value > 0)
        {
            Servicio = await _servicioRepository.GetServicioByIdAsync(value);
        }
    }

    private async Task CargarEstilistasAsync()
    {
        try
        {
            var list = await _estilistaRepository.GetEstilistasAsync();
            if (list != null && list.Any())
            {
                Estilistas.Clear();
                foreach (var e in list)
                    Estilistas.Add(e);
                return;
            }
        }
        catch
        {
            // En caso de fallo o modo sin conexion
        }

        CargarEstilistasFallback();
    }

    private void CargarEstilistasFallback()
    {
        Estilistas.Clear();
        Estilistas.Add(new Estilista
        {
            Id = 1,
            NombreCompleto = "Sofía Ramos",
            Especialidad = "Colorista Senior & Balayage",
            Disponible = true
        });
        Estilistas.Add(new Estilista
        {
            Id = 2,
            NombreCompleto = "Valentina Gómez",
            Especialidad = "Master en Uñas Esculpidas & Spa",
            Disponible = true
        });
        Estilistas.Add(new Estilista
        {
            Id = 3,
            NombreCompleto = "Andrea Morales",
            Especialidad = "Corte de Autor & Visagismo",
            Disponible = true
        });
    }

    [RelayCommand]
    private void ElegirCualquiera()
    {
        EsCualquieraDisponible = true;
        EstilistaSeleccionado = null;
        EstilistaSeleccionadoId = 0;
    }

    [RelayCommand]
    private void SeleccionarEstilista(Estilista estilista)
    {
        if (estilista == null) return;
        EsCualquieraDisponible = false;
        EstilistaSeleccionado = estilista;
        EstilistaSeleccionadoId = estilista.Id;
    }

    [RelayCommand]
    private async Task ContinuarAFechaHoraAsync()
    {
        if (Servicio == null) return;

        var estilistaIdParam = EsCualquieraDisponible || EstilistaSeleccionado == null 
            ? "0" 
            : EstilistaSeleccionado.Id.ToString();

        var estilistaNombreParam = EsCualquieraDisponible || EstilistaSeleccionado == null 
            ? "Cualquier estilista disponible" 
            : EstilistaSeleccionado.NombreCompleto;

        await Shell.Current.GoToAsync(
            $"SlotSelectionPage?servicioId={Servicio.Id}&estilistaId={estilistaIdParam}&estilistaNombre={Uri.EscapeDataString(estilistaNombreParam)}"
        );
    }
}
