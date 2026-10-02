using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ShushineStudio.Mobile.Data.Services;
using ShushineStudio.Mobile.Domain.Entities;
using ShushineStudio.Mobile.Domain.Repositories;

namespace ShushineStudio.Mobile.Presentation.ViewModels.Admin;

public partial class AdminCreateProfessionalViewModel : ObservableObject
{
    private readonly IAuthRepository _authRepository;
    private readonly ITokenStorageService _tokenStorageService;
    private readonly IEstilistaRepository _estilistaRepository;

    [ObservableProperty]
    private string title = "Nuevo Profesional";

    [ObservableProperty]
    private bool isAuthorized = true;

    [ObservableProperty]
    private bool isUnauthorized = false;

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
    private bool isSaving = false;

    public AdminCreateProfessionalViewModel(
        IAuthRepository authRepository,
        ITokenStorageService tokenStorageService,
        IEstilistaRepository estilistaRepository)
    {
        _authRepository = authRepository;
        _tokenStorageService = tokenStorageService;
        _estilistaRepository = estilistaRepository;

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
        if (string.IsNullOrWhiteSpace(Nombre))
        {
            await Shell.Current.DisplayAlert("Validación", "Por favor ingresa el nombre del estilista.", "OK");
            return;
        }

        try
        {
            IsSaving = true;
            var nombreCompleto = string.IsNullOrWhiteSpace(Apellido)
                ? Nombre.Trim()
                : $"{Nombre.Trim()} {Apellido.Trim()}";

            var estilista = new Estilista
            {
                NombreCompleto = nombreCompleto,
                EspecialidadPrincipal = string.IsNullOrWhiteSpace(Especialidad) ? "Estilista Profesional" : Especialidad.Trim(),
                ColorAgenda = string.IsNullOrWhiteSpace(ColorAgenda) ? "#C5A059" : ColorAgenda,
                Activo = Activo
            };

            var exito = await _estilistaRepository.CrearEstilistaAsync(estilista);
            if (exito)
            {
                await Shell.Current.DisplayAlert("Éxito", $"El profesional {nombreCompleto} fue registrado correctamente en la agenda.", "OK");
                await Shell.Current.GoToAsync("..");
            }
            else
            {
                await Shell.Current.DisplayAlert("Error", "No se pudo registrar al profesional en el servidor.", "OK");
            }
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Error", "Error al registrar estilista: " + ex.Message, "OK");
        }
        finally
        {
            IsSaving = false;
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
