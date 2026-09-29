using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ShushineStudio.Mobile.Domain.Entities;
using ShushineStudio.Mobile.Domain.Repositories;

namespace ShushineStudio.Mobile.Presentation.ViewModels.Reviews;

/// <summary>
/// ViewModel para la pantalla de 'Mis Reseñas'.
/// Carga citas completadas pendientes de calificar desde el backend real de citas,
/// e informa que la consulta de reseñas registradas estará disponible con el endpoint del backend.
/// </summary>
public partial class MyReviewsViewModel : BaseViewModel
{
    private readonly IReservaRepository _reservaRepository;

    [ObservableProperty]
    private ObservableCollection<Reserva> citasPendientesCalificar = new();

    [ObservableProperty]
    private ObservableCollection<Resena> resenasPublicadas = new();

    [ObservableProperty]
    private bool tieneCitasPendientes;

    [ObservableProperty]
    private bool tieneResenasPublicadas;

    // No existe endpoint GET /api/resenas actualmente
    public bool BackendResenasDisponible => false;

    public MyReviewsViewModel(IReservaRepository reservaRepository)
    {
        _reservaRepository = reservaRepository;
        Title = "Mis Reseñas";
    }

    [RelayCommand]
    public async Task CargarDatosAsync()
    {
        if (IsBusy) return;

        try
        {
            IsBusy = true;
            ErrorMessage = string.Empty;

            // 1. Obtener citas reales del cliente autenticado
            var citas = await _reservaRepository.GetMisCitasAsync();

            // Filtrar citas completadas para sugerir calificación (100% datos reales)
            var completadas = citas.Where(c =>
                c.Estado?.Equals("COMPLETADA", StringComparison.OrdinalIgnoreCase) == true ||
                c.Estado?.Equals("COMPLETED", StringComparison.OrdinalIgnoreCase) == true
            ).ToList();

            CitasPendientesCalificar.Clear();
            foreach (var cita in completadas)
            {
                CitasPendientesCalificar.Add(cita);
            }

            TieneCitasPendientes = CitasPendientesCalificar.Count > 0;

            // 2. Reseñas publicadas: Al no existir endpoint de consulta en el backend,
            // no se insertan datos falsos ni simulados.
            ResenasPublicadas.Clear();
            TieneResenasPublicadas = false;
        }
        catch (Exception ex)
        {
            ErrorMessage = "No se pudieron sincronizar las citas completadas.";
            System.Diagnostics.Debug.WriteLine($"[MyReviewsViewModel] Error: {ex.Message}");
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private async Task IrACalificarAsync(Reserva cita)
    {
        if (cita == null) return;

        await Shell.Current.GoToAsync("LeaveReviewPage", new Dictionary<string, object>
        {
            { "reserva", cita }
        });
    }

    [RelayCommand]
    private async Task IrADetalleResenaAsync(Resena resena)
    {
        if (resena == null) return;

        await Shell.Current.GoToAsync("ReviewDetailPage", new Dictionary<string, object>
        {
            { "resenaId", resena.Id }
        });
    }

    [RelayCommand]
    private async Task RegresarAsync()
    {
        await Shell.Current.GoToAsync("..");
    }
}
