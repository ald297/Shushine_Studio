using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ShushineStudio.Mobile.Data.Services;
using ShushineStudio.Mobile.Domain.Repositories;

namespace ShushineStudio.Mobile.Presentation.ViewModels.Admin;

public partial class AdminRequestsViewModel : ObservableObject
{
    private readonly IAuthRepository _authRepository;
    private readonly ITokenStorageService _tokenStorageService;

    [ObservableProperty]
    private string title = "Solicitudes";

    [ObservableProperty]
    private bool isAuthorized = true;

    [ObservableProperty]
    private bool isUnauthorized = false;

    [ObservableProperty]
    private string avisoBackend = "El módulo de solicitudes personalizadas de diseño y cotizaciones no está disponible en la API actual. No se simulan cotizaciones ni conversiones ficticias a citas.";

    public AdminRequestsViewModel(
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
    private async Task VerDetalleSolicitudAsync()
    {
        await Shell.Current.GoToAsync("AdminRequestDetailPage");
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
