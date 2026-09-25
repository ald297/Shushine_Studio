using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ShushineStudio.Mobile.Domain.Entities;
using ShushineStudio.Mobile.Domain.UseCases;

namespace ShushineStudio.Mobile.Presentation.ViewModels.Catalog;

/// <summary>
/// ViewModel reactivo para el catálogo de servicios (US-3.01 / Wireframe Pág. 7).
/// Permite búsqueda rápida por nombre y filtrado por chips de categorías.
/// </summary>
public partial class CatalogViewModel : BaseViewModel
{
    private readonly GetServiciosCatalogUseCase _getCatalogUseCase;
    private List<Servicio> _todosLosServicios = new();

    [ObservableProperty]
    private ObservableCollection<Servicio> servicios = new();

    [ObservableProperty]
    private ObservableCollection<string> categorias = new() { "Todos", "Cabello", "Uñas", "Maquillaje", "Spa" };

    [ObservableProperty]
    private string categoriaSeleccionada = "Todos";

    [ObservableProperty]
    private string searchText = string.Empty;

    public CatalogViewModel(GetServiciosCatalogUseCase getCatalogUseCase)
    {
        _getCatalogUseCase = getCatalogUseCase;
        Title = "Catálogo de Belleza";
        _ = LoadServiciosAsync();
    }

    [RelayCommand]
    public async Task LoadServiciosAsync()
    {
        if (IsBusy) return;

        try
        {
            IsBusy = true;
            ErrorMessage = null;

            var items = await _getCatalogUseCase.ExecuteAsync();
            _todosLosServicios = items.ToList();

            AplicarFiltros();
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
    private void FilterByCategoria(string categoria)
    {
        if (string.IsNullOrWhiteSpace(categoria)) return;
        CategoriaSeleccionada = categoria;
        AplicarFiltros();
    }

    partial void OnSearchTextChanged(string value)
    {
        AplicarFiltros();
    }

    private void AplicarFiltros()
    {
        var filtrados = _todosLosServicios.AsEnumerable();

        // 1. Filtrar por categoría
        if (!string.IsNullOrWhiteSpace(CategoriaSeleccionada) && CategoriaSeleccionada != "Todos")
        {
            filtrados = filtrados.Where(s => 
                string.Equals(s.CategoriaNombre, CategoriaSeleccionada, StringComparison.OrdinalIgnoreCase));
        }

        // 2. Filtrar por texto de búsqueda
        if (!string.IsNullOrWhiteSpace(SearchText))
        {
            var query = SearchText.Trim().ToLowerInvariant();
            filtrados = filtrados.Where(s => 
                s.Nombre.ToLowerInvariant().Contains(query) || 
                (s.Descripcion != null && s.Descripcion.ToLowerInvariant().Contains(query)));
        }

        Servicios.Clear();
        foreach (var item in filtrados)
        {
            Servicios.Add(item);
        }
    }

    [RelayCommand]
    private async Task SelectServicioAsync(Servicio servicio)
    {
        if (servicio == null) return;
        
        await Shell.Current.GoToAsync($"ServiceDetailPage?servicioId={servicio.Id}");
    }
}
