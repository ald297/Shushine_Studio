using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ShushineStudio.Mobile.Data.Services;
using ShushineStudio.Mobile.Domain.Repositories;

namespace ShushineStudio.Mobile.Presentation.ViewModels.Admin;

public partial class AdminReviewsViewModel : ObservableObject
{
    private readonly IAuthRepository _authRepository;
    private readonly ITokenStorageService _tokenStorageService;

    [ObservableProperty]
    private string title = "Reseñas";

    [ObservableProperty]
    private bool isAuthorized = true;

    [ObservableProperty]
    private bool isUnauthorized = false;

    [ObservableProperty]
    private string avisoBackend = "La moderación y consulta centralizada de reseñas no está disponible en la API actual. No se simulan calificaciones ni comentarios falsos para preservar la autenticidad del servicio.";

    public AdminReviewsViewModel(
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
    private async Task VerDetalleResenaAsync()
    {
        if (Application.Current?.MainPage != null)
        {
            await Application.Current.MainPage.DisplayAlert(
                "Función Preparada",
                "La moderación y respuesta pública de reseñas estará disponible cuando el backend habilite el módulo correspondiente (/api/resenas).",
                "Entendido"
            );
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
