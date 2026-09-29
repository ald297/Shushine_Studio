using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ShushineStudio.Mobile.Data.Services;
using ShushineStudio.Mobile.Domain.Repositories;

namespace ShushineStudio.Mobile.Presentation.ViewModels.Admin;

public partial class AdminCreateProfessionalViewModel : ObservableObject
{
    private readonly IAuthRepository _authRepository;

    [ObservableProperty]
    private string title = "Nuevo Profesional";

    [ObservableProperty]
    private bool isAuthorized = true;

    [ObservableProperty]
    private bool isUnauthorized = false;

    // Formulario preparado con campos de la entidad real Estilista
    [ObservableProperty]
    private string nombre = string.Empty;

    [ObservableProperty]
    private string apellido = string.Empty;

    [ObservableProperty]
    private string especialidad = string.Empty;

    [ObservableProperty]
    private string biografia = string.Empty;

    [ObservableProperty]
    private string colorAgenda = "#C5A059";

    [ObservableProperty]
    private bool activo = true;

    [ObservableProperty]
    private string avisoBackend = "La creación de profesionales no está disponible en el backend actual. La API de Shushine Studio cuenta actualmente con endpoints de consulta y disponibilidad para el equipo operativo.";

    private readonly ITokenStorageService _tokenStorageService;

    public AdminCreateProfessionalViewModel(IAuthRepository authRepository, ITokenStorageService tokenStorageService)
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
    private async Task GuardarAsync()
    {
        if (Application.Current?.MainPage != null)
        {
            await Application.Current.MainPage.DisplayAlert(
                "Operación No Disponible",
                "La creación de profesionales no está disponible en el backend actual.\n\nPara incorporar nuevos estilistas al equipo, contacte al administrador de la base de datos o espere a que la API habilite el endpoint POST correspondiente.",
                "Entendido"
            );
        }
    }

    [RelayCommand]
    private async Task CancelarAsync()
    {
        await Shell.Current.GoToAsync("..");
    }

    [RelayCommand]
    private async Task RegresarAsync()
    {
        await Shell.Current.GoToAsync("..");
    }
}
