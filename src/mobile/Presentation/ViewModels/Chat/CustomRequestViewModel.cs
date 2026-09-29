using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;

namespace ShushineStudio.Mobile.Presentation.ViewModels.Chat;

/// <summary>
/// ViewModel para Solicitudes Personalizadas del cliente.
///
/// ESTADO BACKEND: UI preparada — backend no disponible.
/// No existe ningún endpoint, entidad (Solicitud, SolicitudPersonalizada, Cotizacion),
/// DTO ni repositorio en el backend actual (Spring Boot).
/// Esta pantalla queda lista para integrarse cuando el backend implemente el módulo.
/// </summary>
public partial class CustomRequestViewModel : ObservableObject
{
    // ────────────────────────────────────────────────────────────
    // Estado de UI
    // ────────────────────────────────────────────────────────────

    [ObservableProperty]
    private bool _isBackendAvailable = false;
    // ↑ false porque no existe endpoint de solicitudes en el backend actual.

    [ObservableProperty]
    private bool _isLoading = false;

    [ObservableProperty]
    private bool _isSubmitting = false;

    [ObservableProperty]
    private string _description = string.Empty;

    [ObservableProperty]
    private bool _hasError = false;

    [ObservableProperty]
    private string _errorMessage = string.Empty;

    [ObservableProperty]
    private bool _hasSuccessMessage = false;

    [ObservableProperty]
    private string _successMessage = string.Empty;

    // Historial de solicitudes — vacío porque no existe backend real
    public ObservableCollection<CustomRequestItem> Requests { get; } = new();

    public bool HasRequests => Requests.Count > 0;
    public bool ShowEmptyState => !IsLoading && !HasError && !HasRequests;
    public bool ShowUnavailableBanner => !IsBackendAvailable;

    // Propiedades negadas para IsVisible (el proyecto no usa InvertedBoolConverter)
    public bool IsNotSubmitting => !IsSubmitting;

    // ────────────────────────────────────────────────────────────
    // Comandos
    // ────────────────────────────────────────────────────────────

    [RelayCommand]
    private async Task SubmitRequestAsync()
    {
        if (string.IsNullOrWhiteSpace(Description)) return;
        if (IsSubmitting) return;

        // UI preparada — backend no disponible.
        // Cuando el backend esté disponible, aquí se enviará la solicitud al repositorio.
        IsSubmitting = true;
        await Task.Delay(600);
        IsSubmitting = false;

        ErrorMessage = "Esta función estará disponible próximamente.";
        HasError = true;

        await Task.Delay(3000);
        HasError = false;
        ErrorMessage = string.Empty;
    }

    [RelayCommand]
    private async Task AttachImageAsync()
    {
        // UI preparada — backend no disponible.
        // No existe mecanismo real de subida de imágenes (Supabase Storage, multipart API).
        await Shell.Current.DisplayAlert(
            "Función no disponible",
            "La carga de imágenes de referencia estará disponible cuando el backend lo soporte.",
            "Entendido");
    }

    [RelayCommand]
    private async Task GoBackAsync()
    {
        await Shell.Current.GoToAsync("..");
    }

    [RelayCommand]
    private void DismissError()
    {
        HasError = false;
        ErrorMessage = string.Empty;
    }
}

/// <summary>
/// Modelo local de solicitud personalizada para la vista.
/// Preparado para cuando el backend implemente el módulo de solicitudes.
/// Los estados son conceptuales — no provienen de un backend real.
/// </summary>
public class CustomRequestItem
{
    public string Description { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public string StatusDisplay { get; set; } = string.Empty;
    public string DateDisplay => CreatedAt.ToString("dd/MM/yyyy");
}
