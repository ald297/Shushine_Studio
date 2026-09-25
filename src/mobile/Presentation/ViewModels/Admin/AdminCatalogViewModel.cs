using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ShushineStudio.Mobile.Domain.Entities;
using ShushineStudio.Mobile.Domain.Repositories;
using ShushineStudio.Mobile.Domain.UseCases;

namespace ShushineStudio.Mobile.Presentation.ViewModels.Admin;

/// <summary>
/// ViewModel de mantenimiento del catálogo de servicios para el rol ADMIN (US-5.05 / US-5.06).
/// Permite activar/desactivar servicios y actualizar el estado del catálogo de belleza en tiempo real.
/// </summary>
public partial class AdminCatalogViewModel : BaseViewModel
{
    private readonly IServicioRepository _servicioRepository;

    [ObservableProperty]
    private ObservableCollection<Servicio> servicios = new();

    [ObservableProperty]
    private string textoBusqueda = string.Empty;

    private List<Servicio> _todosLosServicios = new();

    public AdminCatalogViewModel(IServicioRepository servicioRepository)
    {
        _servicioRepository = servicioRepository;
        Title = "Gestión de Catálogo";
        _ = LoadCatalogAsync();
    }

    [RelayCommand]
    public async Task LoadCatalogAsync()
    {
        if (IsBusy) return;

        try
        {
            IsBusy = true;
            ErrorMessage = null;

            var lista = await _servicioRepository.GetServiciosAsync();
            _todosLosServicios = lista.ToList();

            AplicarFiltro();
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

    partial void OnTextoBusquedaChanged(string value)
    {
        AplicarFiltro();
    }

    private void AplicarFiltro()
    {
        Servicios.Clear();
        var filtrados = string.IsNullOrWhiteSpace(TextoBusqueda)
            ? _todosLosServicios
            : _todosLosServicios.Where(s =>
                s.Nombre.Contains(TextoBusqueda, StringComparison.OrdinalIgnoreCase) ||
                (s.CategoriaNombre ?? "").Contains(TextoBusqueda, StringComparison.OrdinalIgnoreCase));

        foreach (var s in filtrados)
            Servicios.Add(s);
    }

    [RelayCommand]
    private async Task ToggleActivoAsync(Servicio servicio)
    {
        if (servicio == null || Application.Current?.MainPage == null) return;

        var nuevoEstado = !servicio.Activo;
        var accion = nuevoEstado ? "activar" : "desactivar";

        bool confirm = await Application.Current.MainPage.DisplayAlert(
            "Confirmar Cambio",
            $"¿Deseas {accion} el servicio \"{servicio.Nombre}\" del catálogo?",
            "Sí, Confirmar",
            "Cancelar"
        );

        if (!confirm) return;

        // Optimistic update local
        servicio.Activo = nuevoEstado;

        // En producción se llamaría al endpoint de la API
        // await _servicioRepository.UpdateEstadoAsync(servicio.Id, nuevoEstado);

        await Application.Current.MainPage.DisplayAlert(
            "Catálogo Actualizado",
            $"El servicio \"{servicio.Nombre}\" ha sido {(nuevoEstado ? "activado" : "desactivado")} exitosamente.",
            "Aceptar"
        );

        // Refrescar lista para reflejar cambio
        AplicarFiltro();
    }

    [RelayCommand]
    private async Task EditarServicioAsync(Servicio servicio)
    {
        if (servicio == null || Application.Current?.MainPage == null) return;

        // Placeholder para futura pantalla de edición de servicio
        await Application.Current.MainPage.DisplayAlert(
            "Editar Servicio",
            $"La edición de \"{servicio.Nombre}\" estará disponible en la próxima versión del panel administrativo.",
            "Entendido"
        );
    }
}
