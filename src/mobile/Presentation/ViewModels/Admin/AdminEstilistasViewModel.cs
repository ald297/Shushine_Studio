using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ShushineStudio.Mobile.Domain.Entities;
using ShushineStudio.Mobile.Domain.Repositories;

namespace ShushineStudio.Mobile.Presentation.ViewModels.Admin;

/// <summary>
/// ViewModel para la administración del equipo de estilistas (Wireframe Pág. 19: Estado del Estilista).
/// Permite monitorear la disponibilidad operativa y activar/inactivar temporalmente profesionales.
/// </summary>
public partial class AdminEstilistasViewModel : BaseViewModel
{
    private readonly IEstilistaRepository _estilistaRepository;

    [ObservableProperty]
    private ObservableCollection<Estilista> estilistas = new();

    [ObservableProperty]
    private string filtroEstado = "Todos"; // "Todos", "Activos", "Inactivos"

    [ObservableProperty]
    private int totalEstilistas;

    [ObservableProperty]
    private int totalActivos;

    [ObservableProperty]
    private int totalInactivos;

    private List<Estilista> _todosEstilistas = new();

    public AdminEstilistasViewModel(IEstilistaRepository estilistaRepository)
    {
        _estilistaRepository = estilistaRepository;
        Title = "Equipo de Estilistas";
        _ = LoadEstilistasAsync();
    }

    [RelayCommand]
    public async Task LoadEstilistasAsync()
    {
        if (IsBusy) return;

        try
        {
            IsBusy = true;
            ErrorMessage = null;

            var lista = await _estilistaRepository.GetTodosEstilistasAsync();
            _todosEstilistas = lista.ToList();

            // Si vino vacía de la API, asegurar lista inicial de estilistas para operar
            if (_todosEstilistas.Count == 0)
            {
                _todosEstilistas = new List<Estilista>
                {
                    new Estilista { Id = 1, NombreCompleto = "Sofía Valenzuela", EspecialidadPrincipal = "Colorista Senior & Balayage", Activo = true, ColorAgenda = "#D48B96" },
                    new Estilista { Id = 2, NombreCompleto = "Mateo Ramos", EspecialidadPrincipal = "Estilista & Cortes de Autor", Activo = true, ColorAgenda = "#C5A059" },
                    new Estilista { Id = 3, NombreCompleto = "Camila Domínguez", EspecialidadPrincipal = "Especialista en Uñas & Spa", Activo = true, ColorAgenda = "#D8B4E2" },
                    new Estilista { Id = 4, NombreCompleto = "Lucía Aguirre", EspecialidadPrincipal = "Estilista & Peinados de Novia", Activo = false, ColorAgenda = "#E8A598" }
                };
            }

            ActualizarContadores();
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

    [RelayCommand]
    public void SeleccionarFiltro(string filtro)
    {
        FiltroEstado = filtro;
        AplicarFiltro();
    }

    private void ActualizarContadores()
    {
        TotalEstilistas = _todosEstilistas.Count;
        TotalActivos = _todosEstilistas.Count(e => e.Activo);
        TotalInactivos = _todosEstilistas.Count(e => !e.Activo);
    }

    private void AplicarFiltro()
    {
        Estilistas.Clear();

        IEnumerable<Estilista> filtrados = _todosEstilistas;

        if (FiltroEstado == "Activos")
        {
            filtrados = filtrados.Where(e => e.Activo);
        }
        else if (FiltroEstado == "Inactivos")
        {
            filtrados = filtrados.Where(e => !e.Activo);
        }

        foreach (var e in filtrados)
        {
            Estilistas.Add(e);
        }
    }

    [RelayCommand]
    private async Task SetEstadoActivoAsync(Estilista estilista)
    {
        if (estilista == null) return;
        await CambiarEstadoAsync(estilista, true);
    }

    [RelayCommand]
    private async Task SetEstadoInactivoAsync(Estilista estilista)
    {
        if (estilista == null) return;
        await CambiarEstadoAsync(estilista, false);
    }

    private async Task CambiarEstadoAsync(Estilista estilista, bool nuevoEstado)
    {
        if (estilista.Activo == nuevoEstado) return;

        try
        {
            IsBusy = true;
            estilista.Activo = nuevoEstado;

            await _estilistaRepository.ActualizarEstadoEstilistaAsync(estilista.Id, nuevoEstado);

            ActualizarContadores();
            AplicarFiltro();

            string mensaje = nuevoEstado 
                ? $"{estilista.NombreCompleto} ahora está activa para recibir citas en salón."
                : $"{estilista.NombreCompleto} fue marcada como Inactiva temporalmente.";

            if (Application.Current?.MainPage != null)
            {
                await Application.Current.MainPage.DisplayAlert("Disponibilidad Actualizada", mensaje, "Aceptar");
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
}
