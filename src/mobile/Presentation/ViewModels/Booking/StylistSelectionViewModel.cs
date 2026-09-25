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

    [ObservableProperty]
    private long servicioId;

    [ObservableProperty]
    private Servicio? servicio;

    [ObservableProperty]
    private ObservableCollection<Estilista> estilistas = new();

    [ObservableProperty]
    private Estilista? estilistaSeleccionado;

    [ObservableProperty]
    private bool esCualquieraDisponible = true;

    public StylistSelectionViewModel(IServicioRepository servicioRepository)
    {
        _servicioRepository = servicioRepository;
        Title = "Selecciona tu Estilista";
        CargarEstilistas();
    }

    async partial void OnServicioIdChanged(long value)
    {
        if (value > 0)
        {
            Servicio = await _servicioRepository.GetServicioByIdAsync(value);
        }
    }

    private void CargarEstilistas()
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
    }

    [RelayCommand]
    private void SeleccionarEstilista(Estilista estilista)
    {
        if (estilista == null) return;
        EsCualquieraDisponible = false;
        EstilistaSeleccionado = estilista;
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
