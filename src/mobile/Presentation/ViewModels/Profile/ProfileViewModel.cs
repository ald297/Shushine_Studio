using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ShushineStudio.Mobile.Domain.Repositories;

namespace ShushineStudio.Mobile.Presentation.ViewModels.Profile;

public partial class ProfileViewModel : BaseViewModel
{
    private readonly IAuthRepository _authRepository;

    [ObservableProperty]
    private string nombre = "Camila Calderón";

    [ObservableProperty]
    private string email = "cliente@shushinestudio.com";

    [ObservableProperty]
    private string nivelFidelidad = "Nivel Oro (15% Desc.)";

    [ObservableProperty]
    private string tipoCabello = "Liso / Tratamiento de Keratina";

    public ProfileViewModel(IAuthRepository authRepository)
    {
        _authRepository = authRepository;
        Title = "Mi Perfil & Ficha";
    }

    [RelayCommand]
    public async Task CargarPerfilAsync()
    {
        if (IsBusy) return;

        try
        {
            IsBusy = true;
            var usuario = await _authRepository.GetCurrentUserAsync();
            if (usuario != null)
            {
                if (!string.IsNullOrWhiteSpace(usuario.NombreCompleto))
                    Nombre = usuario.NombreCompleto;
                if (!string.IsNullOrWhiteSpace(usuario.Email))
                    Email = usuario.Email;
                if (!string.IsNullOrWhiteSpace(usuario.NivelFidelidad))
                    NivelFidelidad = $"{usuario.NivelFidelidad} ({usuario.PuntosAcumulados} pts)";
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

    [RelayCommand]
    private async Task CerrarSesionAsync()
    {
        if (Application.Current?.MainPage == null) return;

        bool confirm = await Application.Current.MainPage.DisplayAlert(
            "Cerrar Sesión", 
            "¿Deseas salir de tu cuenta en Shushine Studio?", 
            "Cerrar Sesión", 
            "Cancelar"
        );

        if (confirm)
        {
            await _authRepository.LogoutAsync();
            await Shell.Current.GoToAsync("//LoginPage");
        }
    }
}
