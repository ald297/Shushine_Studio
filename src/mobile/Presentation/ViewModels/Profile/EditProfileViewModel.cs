using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ShushineStudio.Mobile.Domain.Repositories;

namespace ShushineStudio.Mobile.Presentation.ViewModels.Profile;

/// <summary>
/// ViewModel para editar perfil. Los únicos campos soportados por backend (PUT no existe aún)
/// son los que devuelve GET /api/auth/me: nombre, apellido, telefono.
/// IMPORTANTE: No existe endpoint PUT /api/auth/me ni /api/usuarios/{id}.
/// El guardado es UI preparada — backend no disponible para actualización de perfil.
/// </summary>
public partial class EditProfileViewModel : BaseViewModel
{
    private readonly IAuthRepository _authRepository;

    [ObservableProperty]
    private string nombre = string.Empty;

    [ObservableProperty]
    private string apellido = string.Empty;

    [ObservableProperty]
    private string telefono = string.Empty;

    [ObservableProperty]
    private string login = string.Empty;

    [ObservableProperty]
    private bool guardadoExitoso = false;

    // No existe endpoint de actualización de perfil
    public bool BackendActualizacionDisponible => false;

    public EditProfileViewModel(IAuthRepository authRepository)
    {
        _authRepository = authRepository;
        Title = "Editar Perfil";
    }

    [RelayCommand]
    public async Task CargarDatosAsync()
    {
        if (IsBusy) return;
        try
        {
            IsBusy = true;
            ErrorMessage = string.Empty;
            var usuario = await _authRepository.GetCurrentUserAsync();
            if (usuario != null)
            {
                var partes = usuario.NombreCompleto?.Split(' ', 2, StringSplitOptions.RemoveEmptyEntries) ?? [];
                Nombre = partes.Length > 0 ? partes[0] : string.Empty;
                Apellido = partes.Length > 1 ? partes[1] : string.Empty;
                Telefono = usuario.Telefono ?? string.Empty;
                Login = usuario.Login ?? string.Empty;
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = "No se pudo cargar tu información.";
            System.Diagnostics.Debug.WriteLine($"[EditProfileVM] {ex.Message}");
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private async Task GuardarCambiosAsync()
    {
        if (string.IsNullOrWhiteSpace(Nombre))
        {
            ErrorMessage = "El nombre no puede estar vacío.";
            return;
        }
        if (string.IsNullOrWhiteSpace(Telefono))
        {
            ErrorMessage = "El teléfono no puede estar vacío.";
            return;
        }

        // UI preparada — no existe endpoint PUT de actualización de perfil
        if (Application.Current?.MainPage != null)
        {
            await Application.Current.MainPage.DisplayAlert(
                "Función no disponible",
                "La actualización de perfil requiere soporte en el backend (PUT /api/auth/perfil). Esta pantalla está preparada para cuando el endpoint esté disponible.",
                "Entendido"
            );
        }
    }

    [RelayCommand]
    private async Task RegresarAsync()
        => await Shell.Current.GoToAsync("..");
}
