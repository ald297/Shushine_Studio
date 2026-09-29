using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ShushineStudio.Mobile.Data.Services;
using ShushineStudio.Mobile.Domain.Entities;
using ShushineStudio.Mobile.Domain.Repositories;

namespace ShushineStudio.Mobile.Presentation.ViewModels.Admin;

public partial class AdminServicesViewModel : ObservableObject
{
    private readonly IServicioRepository _servicioRepository;
    private readonly IAuthRepository _authRepository;
    private readonly ITokenStorageService _tokenStorageService;

    [ObservableProperty]
    private string title = "Gestión de Servicios";

    [ObservableProperty]
    private bool isAuthorized = true;

    [ObservableProperty]
    private bool isUnauthorized = false;

    [ObservableProperty]
    private bool isBusy;

    [ObservableProperty]
    private string? errorMessage;

    [ObservableProperty]
    private bool hasError;

    [ObservableProperty]
    private bool estaVacio;

    [ObservableProperty]
    private string textoBusqueda = string.Empty;

    [ObservableProperty]
    private string categoriaSeleccionada = "Todos";

    [ObservableProperty]
    private string filtroEstado = "Todos"; // "Todos", "Activos", "Inactivos"

    [ObservableProperty]
    private int totalServicios;

    [ObservableProperty]
    private int totalActivos;

    [ObservableProperty]
    private int totalInactivos;

    public ObservableCollection<Servicio> Servicios { get; } = new();
    public ObservableCollection<string> Categorias { get; } = new();

    private List<Servicio> _todosLosServicios = new();

    public AdminServicesViewModel(
        IServicioRepository servicioRepository,
        IAuthRepository authRepository,
        ITokenStorageService tokenStorageService)
    {
        _servicioRepository = servicioRepository;
        _authRepository = authRepository;
        _tokenStorageService = tokenStorageService;

        _ = InicializarAsync();
    }

    private async Task InicializarAsync()
    {
        await VerificarRolAdminAsync();
        if (IsAuthorized)
        {
            await CargarCategoriasAsync();
            await CargarServiciosAsync();
        }
    }

    private async Task VerificarRolAdminAsync()
    {
        try
        {
            var rol = await _tokenStorageService.GetRoleAsync();
            var currentUser = await _authRepository.GetCurrentUserAsync();
            if (currentUser != null && !string.IsNullOrEmpty(currentUser.Rol))
            {
                rol = currentUser.Rol;
            }

            var esAdmin = !string.IsNullOrEmpty(rol) && rol.ToUpperInvariant().Contains("ADMIN");
            IsAuthorized = esAdmin;
            IsUnauthorized = !esAdmin;
        }
        catch
        {
            IsAuthorized = false;
            IsUnauthorized = true;
        }
    }

    [RelayCommand]
    public async Task CargarCategoriasAsync()
    {
        try
        {
            var cats = await _servicioRepository.GetCategoriasAsync();
            Categorias.Clear();
            foreach (var c in cats)
            {
                Categorias.Add(c);
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[AdminServicesViewModel] Error categorias: {ex.Message}");
        }
    }

    [RelayCommand]
    public async Task CargarServiciosAsync()
    {
        if (IsBusy || !IsAuthorized) return;

        try
        {
            IsBusy = true;
            ErrorMessage = null;
            HasError = false;

            var lista = await _servicioRepository.GetServiciosAsync();
            _todosLosServicios = lista.ToList();

            ActualizarContadores();
            AplicarFiltros();
        }
        catch (Exception ex)
        {
            HasError = true;
            ErrorMessage = "Error al conectar con la API de servicios: " + ex.Message;
        }
        finally
        {
            IsBusy = false;
        }
    }

    partial void OnTextoBusquedaChanged(string value) => AplicarFiltros();
    partial void OnCategoriaSeleccionadaChanged(string value) => AplicarFiltros();
    partial void OnFiltroEstadoChanged(string value) => AplicarFiltros();

    [RelayCommand]
    private void SeleccionarFiltroEstado(string estado)
    {
        FiltroEstado = estado;
    }

    [RelayCommand]
    private void SeleccionarCategoria(string categoria)
    {
        CategoriaSeleccionada = categoria;
    }

    private void ActualizarContadores()
    {
        TotalServicios = _todosLosServicios.Count;
        TotalActivos = _todosLosServicios.Count(s => s.Activo);
        TotalInactivos = _todosLosServicios.Count(s => !s.Activo);
    }

    private void AplicarFiltros()
    {
        Servicios.Clear();
        IEnumerable<Servicio> filtrados = _todosLosServicios;

        // Filtro por Estado
        if (FiltroEstado == "Activos")
        {
            filtrados = filtrados.Where(s => s.Activo);
        }
        else if (FiltroEstado == "Inactivos")
        {
            filtrados = filtrados.Where(s => !s.Activo);
        }

        // Filtro por Categoría
        if (!string.IsNullOrWhiteSpace(CategoriaSeleccionada) && CategoriaSeleccionada != "Todos")
        {
            filtrados = filtrados.Where(s => string.Equals(s.CategoriaNombre, CategoriaSeleccionada, StringComparison.OrdinalIgnoreCase));
        }

        // Búsqueda textual local
        if (!string.IsNullOrWhiteSpace(TextoBusqueda))
        {
            var q = TextoBusqueda.Trim().ToLowerInvariant();
            filtrados = filtrados.Where(s =>
                (!string.IsNullOrEmpty(s.Nombre) && s.Nombre.ToLowerInvariant().Contains(q)) ||
                (!string.IsNullOrEmpty(s.Descripcion) && s.Descripcion.ToLowerInvariant().Contains(q)) ||
                (!string.IsNullOrEmpty(s.CategoriaNombre) && s.CategoriaNombre.ToLowerInvariant().Contains(q)) ||
                (!string.IsNullOrEmpty(s.CodigoServicio) && s.CodigoServicio.ToLowerInvariant().Contains(q)));
        }

        foreach (var s in filtrados)
        {
            Servicios.Add(s);
        }

        EstaVacio = Servicios.Count == 0;
    }

    [RelayCommand]
    private async Task IrACrearServicioAsync()
    {
        await Shell.Current.GoToAsync("AdminCreateServicePage");
    }

    [RelayCommand]
    private async Task IrADetalleServicioAsync(Servicio servicio)
    {
        if (servicio == null) return;

        await Shell.Current.GoToAsync("AdminServiceDetailPage", new Dictionary<string, object>
        {
            { "servicio", servicio },
            { "servicioId", servicio.Id }
        });
    }

    [RelayCommand]
    private async Task AlternarEstadoServicioAsync(Servicio servicio)
    {
        if (servicio == null) return;

        try
        {
            IsBusy = true;
            servicio.Activo = !servicio.Activo;
            var exito = await _servicioRepository.ActualizarServicioAsync(servicio);
            if (exito)
            {
                ActualizarContadores();
                AplicarFiltros();
            }
            else
            {
                servicio.Activo = !servicio.Activo; // Revertir
                if (Application.Current?.MainPage != null)
                {
                    await Application.Current.MainPage.DisplayAlert("Error", "No se pudo actualizar el estado del servicio en el servidor.", "Aceptar");
                }
            }
        }
        catch (Exception ex)
        {
            servicio.Activo = !servicio.Activo;
            System.Diagnostics.Debug.WriteLine($"[AdminServicesViewModel] Error alternar estado: {ex.Message}");
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private async Task EliminarServicioAsync(Servicio servicio)
    {
        if (servicio == null) return;

        if (Application.Current?.MainPage != null)
        {
            var confirma = await Application.Current.MainPage.DisplayAlert(
                "Confirmar Eliminación",
                $"¿Está seguro de eliminar el servicio '{servicio.Nombre}' del catálogo del salón?",
                "Eliminar",
                "Cancelar");

            if (!confirma) return;
        }

        try
        {
            IsBusy = true;
            var exito = await _servicioRepository.EliminarServicioAsync(servicio.Id);
            if (exito)
            {
                _todosLosServicios.Remove(servicio);
                ActualizarContadores();
                AplicarFiltros();

                if (Application.Current?.MainPage != null)
                {
                    await Application.Current.MainPage.DisplayAlert("Éxito", "El servicio ha sido eliminado del catálogo.", "Aceptar");
                }
            }
            else
            {
                if (Application.Current?.MainPage != null)
                {
                    await Application.Current.MainPage.DisplayAlert("Aviso", "No se pudo eliminar el servicio. Puede tener citas asociadas en el historial.", "Aceptar");
                }
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[AdminServicesViewModel] Error eliminar: {ex.Message}");
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private async Task RegresarAsync()
    {
        await Shell.Current.GoToAsync("..");
    }

    [RelayCommand]
    private async Task RegresarALoginAsync()
    {
        await Shell.Current.GoToAsync("//LoginPage");
    }
}
