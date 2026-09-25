using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ShushineStudio.Mobile.Domain.Repositories;

namespace ShushineStudio.Mobile.Presentation.ViewModels.Profile;

/// <summary>
/// ViewModel reactivo para la visualización y edición del perfil de cliente (US-2.03 / Wireframe Pág. 14).
/// Gestiona datos personales, nivel de fidelidad, ficha técnica capilar y cierre de sesión.
/// </summary>
public partial class ProfileViewModel : BaseViewModel
{
    private readonly IAuthRepository _authRepository;

    [ObservableProperty]
    private string nombre = "Camila Calderón";

    [ObservableProperty]
    private string email = "camila.calderon@shushinestudio.com";

    [ObservableProperty]
    private string telefono = "7890-1234";

    [ObservableProperty]
    private string fechaNacimiento = "14 de Octubre";

    [ObservableProperty]
    private string nivelFidelidad = "Nivel Oro (15% Desc. Exclusivo)";

    [ObservableProperty]
    private int puntosFidelidad = 450;

    [ObservableProperty]
    private string tipoCabello = "Ondulado / Cuero cabelludo sensible";

    [ObservableProperty]
    private string alergias = "Sin amoníaco / Sensible a fragancias fuertes";

    [ObservableProperty]
    private string estilistaPreferido = "Sofía Ramos (Colorista Senior)";

    [ObservableProperty]
    private string notasPreferencias = "Prefiero lavado con agua tibia y secado suave con toalla de microfibra.";

    [ObservableProperty]
    private bool isEditing = false;

    public ProfileViewModel(IAuthRepository authRepository)
    {
        _authRepository = authRepository;
        Title = "Mi Perfil & Ficha";
    }

    [RelayCommand]
    private void ToggleEdit()
    {
        IsEditing = !IsEditing;
    }

    [RelayCommand]
    private async Task GuardarCambiosAsync()
    {
        if (IsBusy) return;

        if (string.IsNullOrWhiteSpace(Nombre) || string.IsNullOrWhiteSpace(Telefono))
        {
            await ShowAlertAsync("Campos Requeridos", "El nombre y teléfono no pueden quedar vacíos.");
            return;
        }

        try
        {
            IsBusy = true;

            // Simular guardado persistente local / backend
            await Task.Delay(600);

            IsEditing = false;

            await ShowAlertAsync(
                "¡Perfil Actualizado!", 
                "Tus datos de contacto y preferencias estéticas han sido guardados correctamente en el salón.", 
                "Continuar"
            );
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
            "¿Estás segura de que deseas salir de tu cuenta en Shushine Studio?", 
            "Cerrar Sesión", 
            "Cancelar"
        );

        if (confirm)
        {
            await _authRepository.LogoutAsync();
            await Shell.Current.GoToAsync("//LoginPage");
        }
    }

    private static async Task ShowAlertAsync(string title, string message, string cancel = "Aceptar")
    {
        if (Application.Current?.MainPage != null)
        {
            await Application.Current.MainPage.DisplayAlert(title, message, cancel);
        }
    }
}
