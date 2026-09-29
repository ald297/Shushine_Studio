using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ShushineStudio.Mobile.Domain.Entities;

namespace ShushineStudio.Mobile.Presentation.ViewModels.Reviews;

/// <summary>
/// ViewModel para la pantalla de Calificar / Dejar Reseña.
/// Recibe la cita completada y valida la calificación antes de enviar.
/// Al no existir endpoint en el backend, informa con transparencia el estado de UI preparada.
/// </summary>
public partial class LeaveReviewViewModel : BaseViewModel, IQueryAttributable
{
    [ObservableProperty]
    private Reserva? cita;

    [ObservableProperty]
    private int estrellas = 5;

    [ObservableProperty]
    private string comentario = string.Empty;

    [ObservableProperty]
    private bool visiblePublica = true;

    [ObservableProperty]
    private bool mostrarModalPosponer;

    // El backend no cuenta con endpoint POST /api/resenas actualmente
    public bool BackendDisponible => false;

    public string ServicioNombre => Cita?.ServicioNombre ?? "Servicio del Atelier";
    public string EstilistaNombre => Cita?.EstilistaNombre ?? "Especialista asignada";
    public string CodigoCita => Cita?.CodigoReserva ?? "#SHU-0000";
    public string FechaTexto => Cita != null && Cita.FechaHoraInicio != DateTime.MinValue
        ? Cita.FechaHoraInicio.ToString("ddd dd MMM yyyy · h:mm tt")
        : "Cita terminada";

    public string NivelFeedback => Estrellas switch
    {
        1 => "Queremos Mejorar",
        2 => "Oportunidad de Mejora",
        3 => "Buena Experiencia",
        4 => "Gran Experiencia",
        5 => "Experiencia Excepcional",
        _ => "Calificación"
    };

    public string FraseFeedback => Estrellas switch
    {
        1 => "Lamentamos que tu visita no fuera perfecta. Revisaremos cada detalle para mejorar.",
        2 => "Tu opinión nos ayuda a perfeccionar la atención y técnicas de nuestras especialistas.",
        3 => "¡Gracias por tu valoración! Seguiremos elevando nuestros estándares de servicio.",
        4 => "¡Muchas gracias! Nos alegra saber que disfrutaste tu momento en el atelier.",
        5 => "¡Nos encanta saber que disfrutaste tu visita! Gracias por ser parte de Shunshine Studio.",
        _ => string.Empty
    };

    public string ContadorCaracteres => $"{Comentario?.Length ?? 0} / 500 caracteres";

    public LeaveReviewViewModel()
    {
        Title = "Calificar Servicio";
    }

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (query.TryGetValue("reserva", out var obj) && obj is Reserva res)
        {
            Cita = res;
            OnPropertyChanged(nameof(ServicioNombre));
            OnPropertyChanged(nameof(EstilistaNombre));
            OnPropertyChanged(nameof(CodigoCita));
            OnPropertyChanged(nameof(FechaTexto));
        }
    }

    [RelayCommand]
    private void SeleccionarEstrellas(string valor)
    {
        if (int.TryParse(valor, out var stars))
        {
            Estrellas = Math.Clamp(stars, 1, 5);
            OnPropertyChanged(nameof(NivelFeedback));
            OnPropertyChanged(nameof(FraseFeedback));
        }
    }

    partial void OnComentarioChanged(string value)
    {
        OnPropertyChanged(nameof(ContadorCaracteres));
    }

    [RelayCommand]
    private async Task EnviarResenaAsync()
    {
        if (IsBusy) return;

        // Validación de calificación
        if (Estrellas < 1 || Estrellas > 5)
        {
            ErrorMessage = "Por favor selecciona una valoración entre 1 y 5 estrellas.";
            return;
        }

        ErrorMessage = string.Empty;

        // Regla: No inventar persistencia ni simular que fue guardada.
        // Al no existir endpoint POST /api/resenas, se muestra diálogo transparente.
        if (Application.Current?.MainPage != null)
        {
            await Application.Current.MainPage.DisplayAlert(
                "Función Preparada",
                "El formulario fue validado correctamente con tu calificación de " + Estrellas + " estrellas. Actualmente el servidor de Shunshine Studio no cuenta con el endpoint para registrar reseñas. La función quedará activa en cuanto se habilite en la API.",
                "Entendido"
            );
        }

        // Navegación de retorno al flujo
        await Shell.Current.GoToAsync("..");
    }

    [RelayCommand]
    private void AbrirModalPosponer()
    {
        MostrarModalPosponer = true;
    }

    [RelayCommand]
    private void CerrarModalPosponer()
    {
        MostrarModalPosponer = false;
    }

    [RelayCommand]
    private async Task ConfirmarPosponerAsync()
    {
        MostrarModalPosponer = false;
        await Shell.Current.GoToAsync("..");
    }

    [RelayCommand]
    private async Task RegresarAsync()
    {
        await Shell.Current.GoToAsync("..");
    }
}
