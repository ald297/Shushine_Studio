using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ShushineStudio.Mobile.Data.Services;
using ShushineStudio.Mobile.Domain.Repositories;

namespace ShushineStudio.Mobile.Presentation.ViewModels.Admin;

public partial class AdminProductsViewModel : ObservableObject
{
    private readonly IAuthRepository _authRepository;
    private readonly ITokenStorageService _tokenStorageService;

    [ObservableProperty]
    private string title = "Gestión de Productos";

    [ObservableProperty]
    private bool isAuthorized = true;

    [ObservableProperty]
    private bool isUnauthorized = false;

    [ObservableProperty]
    private string textoBusqueda = string.Empty;

    [ObservableProperty]
    private string filtroEstado = "Todos";

    [ObservableProperty]
    private string avisoBackend = "El catálogo de productos de venta e inventario no está implementado en la API del servidor. Actualmente el sistema opera enfocado en la gestión de servicios y citas de salón.";

    public AdminProductsViewModel(
        IAuthRepository authRepository,
        ITokenStorageService tokenStorageService)
    {
        _authRepository = authRepository;
        _tokenStorageService = tokenStorageService;

        _ = VerificarRolAdminAsync();
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
    private async Task NuevoProductoAsync()
    {
        if (Application.Current?.MainPage != null)
        {
            await Application.Current.MainPage.DisplayAlert(
                "Función Preparada",
                "La creación de productos no está disponible en el backend actual.\n\nEstará habilitada cuando el servidor incorpore el controlador y modelo de inventario correspondiente.",
                "Entendido"
            );
        }
    }

    [RelayCommand]
    private async Task VerDetalleProductoAsync()
    {
        await Shell.Current.GoToAsync("AdminProductDetailPage");
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
