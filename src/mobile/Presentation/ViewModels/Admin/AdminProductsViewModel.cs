using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ShushineStudio.Mobile.Data.Services;
using ShushineStudio.Mobile.Domain.Entities;
using ShushineStudio.Mobile.Domain.Repositories;

namespace ShushineStudio.Mobile.Presentation.ViewModels.Admin;

public partial class AdminProductsViewModel : ObservableObject
{
    private readonly IAuthRepository _authRepository;
    private readonly ITokenStorageService _tokenStorageService;
    private readonly IProductoRepository _productoRepository;

    private List<Producto> _todosLosProductos = new();

    [ObservableProperty]
    private string title = "Gestión de Productos";

    [ObservableProperty]
    private bool isAuthorized = true;

    [ObservableProperty]
    private bool isUnauthorized = false;

    [ObservableProperty]
    private bool isLoading = false;

    [ObservableProperty]
    private bool hasError = false;

    [ObservableProperty]
    private string errorMessage = string.Empty;

    [ObservableProperty]
    private string textoBusqueda = string.Empty;

    public ObservableCollection<Producto> Productos { get; } = new();

    public bool HasProductos => Productos.Count > 0;
    public bool ShowEmptyState => !IsLoading && !HasError && !HasProductos;

    public AdminProductsViewModel(
        IAuthRepository authRepository,
        ITokenStorageService tokenStorageService,
        IProductoRepository productoRepository)
    {
        _authRepository = authRepository;
        _tokenStorageService = tokenStorageService;
        _productoRepository = productoRepository;

        _ = InicializarAsync();
    }

    private async Task InicializarAsync()
    {
        await VerificarRolAdminAsync();
        if (IsAuthorized)
        {
            await CargarProductosAsync();
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
    public async Task CargarProductosAsync()
    {
        if (IsLoading) return;

        try
        {
            IsLoading = true;
            HasError = false;
            ErrorMessage = string.Empty;

            _todosLosProductos = await _productoRepository.GetProductosAdminAsync();
            AplicarFiltros();
        }
        catch (Exception ex)
        {
            HasError = true;
            ErrorMessage = "Error al consultar productos: " + ex.Message;
        }
        finally
        {
            IsLoading = false;
            OnPropertyChanged(nameof(ShowEmptyState));
        }
    }

    partial void OnTextoBusquedaChanged(string value)
    {
        AplicarFiltros();
    }

    private void AplicarFiltros()
    {
        Productos.Clear();
        var lista = _todosLosProductos.AsEnumerable();

        if (!string.IsNullOrWhiteSpace(TextoBusqueda))
        {
            var busqueda = TextoBusqueda.Trim().ToLowerInvariant();
            lista = lista.Where(p =>
                (p.Nombre != null && p.Nombre.ToLowerInvariant().Contains(busqueda)) ||
                (p.Marca != null && p.Marca.ToLowerInvariant().Contains(busqueda)) ||
                (p.CodigoProducto != null && p.CodigoProducto.ToLowerInvariant().Contains(busqueda)));
        }

        foreach (var p in lista)
        {
            Productos.Add(p);
        }

        OnPropertyChanged(nameof(HasProductos));
        OnPropertyChanged(nameof(ShowEmptyState));
    }

    [RelayCommand]
    private async Task NuevoProductoAsync()
    {
        await Shell.Current.GoToAsync("AdminProductDetailPage?id=0");
    }

    [RelayCommand]
    private async Task VerDetalleProductoAsync(Producto? producto)
    {
        if (producto == null) return;
        await Shell.Current.GoToAsync($"AdminProductDetailPage?id={producto.Id}");
    }

    [RelayCommand]
    private async Task CambiarEstadoAsync(Producto? producto)
    {
        if (producto == null) return;

        try
        {
            var nuevoEstado = !producto.Activo;
            var ok = await _productoRepository.CambiarEstadoAsync(producto.Id, nuevoEstado);
            if (ok)
            {
                producto.Activo = nuevoEstado;
                AplicarFiltros();
            }
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Error", "No se pudo cambiar el estado: " + ex.Message, "OK");
        }
    }

    [RelayCommand]
    private async Task AjustarStockAsync(Producto? producto)
    {
        if (producto == null) return;

        var respuesta = await Shell.Current.DisplayPromptAsync(
            "Modificar Stock",
            $"Stock actual de {producto.Nombre}: {producto.StockActual}\nIngresa el nuevo stock:",
            "Actualizar", "Cancelar",
            initialValue: producto.StockActual.ToString(),
            keyboard: Keyboard.Numeric);

        if (int.TryParse(respuesta, out int nuevoStock) && nuevoStock >= 0)
        {
            try
            {
                var actualizado = await _productoRepository.ActualizarStockAsync(producto.Id, nuevoStock);
                producto.StockActual = actualizado.StockActual;
                producto.StockBajo = actualizado.StockBajo;
                AplicarFiltros();
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error", "No se pudo actualizar el stock: " + ex.Message, "OK");
            }
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
        await _authRepository.LogoutAsync();
        if (Shell.Current is AppShell appShell)
        {
            appShell.SwitchToLogin();
        }
        else
        {
            await Shell.Current.GoToAsync("//LoginPage");
        }
    }
}
