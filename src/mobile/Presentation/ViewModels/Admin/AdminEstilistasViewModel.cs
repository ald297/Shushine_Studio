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

    [ObservableProperty]
    private bool isCreandoNuevo;

    [ObservableProperty]
    private string nuevoNombre = string.Empty;

    [ObservableProperty]
    private string nuevaEspecialidad = string.Empty;

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
            _todosEstilistas = lista?.ToList() ?? new List<Estilista>();

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

            MostrarToast("Disponibilidad Actualizada", mensaje);
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
    private void AbrirModalNuevo()
    {
        NuevoNombre = string.Empty;
        NuevaEspecialidad = string.Empty;
        IsCreandoNuevo = true;
    }

    [RelayCommand]
    private void CerrarModalNuevo()
    {
        IsCreandoNuevo = false;
    }

    [RelayCommand]
    private async Task GuardarNuevoEstilistaAsync()
    {
        if (string.IsNullOrWhiteSpace(NuevoNombre))
        {
            MostrarToast("Validación", "Por favor ingresa el nombre del estilista.");
            return;
        }

        if (string.IsNullOrWhiteSpace(NuevaEspecialidad))
        {
            MostrarToast("Validación", "Por favor ingresa la especialidad.");
            return;
        }

        try
        {
            IsBusy = true;
            var nuevo = new Estilista
            {
                Id = _todosEstilistas.Any() ? _todosEstilistas.Max(e => e.Id) + 1 : 1,
                NombreCompleto = NuevoNombre.Trim(),
                EspecialidadPrincipal = NuevaEspecialidad.Trim(),
                Activo = true,
                ColorAgenda = "#D48B96"
            };

            await _estilistaRepository.CrearEstilistaAsync(nuevo);

            _todosEstilistas.Insert(0, nuevo);
            ActualizarContadores();
            AplicarFiltro();
            IsCreandoNuevo = false;

            MostrarToast("Estilista Registrado", $"{nuevo.NombreCompleto} ha sido añadido al equipo del salón.");
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
    private async Task EliminarEstilistaAsync(Estilista estilista)
    {
        if (estilista == null) return;

        if (Application.Current?.MainPage != null)
        {
            bool confirmar = await Application.Current.MainPage.DisplayAlert(
                "Remover Estilista",
                $"¿Estás seguro de que deseas retirar a {estilista.NombreCompleto} del salón?",
                "Sí, remover",
                "Cancelar"
            );

            if (!confirmar) return;
        }

        try
        {
            IsBusy = true;
            await _estilistaRepository.EliminarEstilistaAsync(estilista.Id);

            _todosEstilistas.Remove(estilista);
            ActualizarContadores();
            AplicarFiltro();

            MostrarToast("Equipo Actualizado", $"{estilista.NombreCompleto} fue retirado del equipo.");
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
