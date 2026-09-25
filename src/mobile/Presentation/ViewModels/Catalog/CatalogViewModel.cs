using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ShushineStudio.Mobile.Domain.Entities;
using ShushineStudio.Mobile.Domain.UseCases;

namespace ShushineStudio.Mobile.Presentation.ViewModels.Catalog;

public partial class CatalogViewModel : BaseViewModel
{
    private readonly GetServiciosCatalogUseCase _getCatalogUseCase;

    [ObservableProperty]
    private ObservableCollection<Servicio> servicios = new();

    [ObservableProperty]
    private ObservableCollection<string> categorias = new() { "Todos", "Cabello", "Uñas", "Maquillaje", "Spa" };

    [ObservableProperty]
    private string categoriaSeleccionada = "Todos";

    public CatalogViewModel(GetServiciosCatalogUseCase getCatalogUseCase)
    {
        _getCatalogUseCase = getCatalogUseCase;
        Title = "Catálogo de Belleza";
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
            Servicios.Clear();
            foreach (var item in items)
            {
                Servicios.Add(item);
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
    private async Task FilterByCategoriaAsync(string categoria)
    {
        CategoriaSeleccionada = categoria;
        await LoadServiciosAsync();
    }

    [RelayCommand]
    private async Task SelectServicioAsync(Servicio servicio)
    {
        if (servicio == null) return;
        
        await Shell.Current.GoToAsync($"SeleccionHorarioPage?servicioId={servicio.Id}");
    }
}
