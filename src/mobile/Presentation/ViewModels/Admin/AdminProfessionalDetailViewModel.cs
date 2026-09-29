using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ShushineStudio.Mobile.Domain.Entities;
using ShushineStudio.Mobile.Domain.Repositories;

namespace ShushineStudio.Mobile.Presentation.ViewModels.Admin;

public partial class AdminProfessionalDetailViewModel : ObservableObject, IQueryAttributable
{
    private readonly IEstilistaRepository _estilistaRepository;

    [ObservableProperty]
    private string title = "Detalle de Profesional";

    [ObservableProperty]
    private Estilista? estilista;

    [ObservableProperty]
    private long estilistaId;

    [ObservableProperty]
    private string nombreCompleto = string.Empty;

    [ObservableProperty]
    private string especialidadPrincipal = string.Empty;

    [ObservableProperty]
    private string biografia = "Sin biografía registrada.";

    [ObservableProperty]
    private string colorAgenda = "#C5A059";

    [ObservableProperty]
    private bool activo = true;

    [ObservableProperty]
    private string estadoTexto = "Activo";

    [ObservableProperty]
    private string estadoBadgeBgColor = "#E8F5E9";

    [ObservableProperty]
    private string estadoBadgeTextColor = "#2E7D32";

    [ObservableProperty]
    private string iniciales = "ES";

    // Disponibilidad en Tiempo Real
    [ObservableProperty]
    private DateTime fechaDisponibilidad = DateTime.Today;

    [ObservableProperty]
    private string fechaDisponibilidadTexto = string.Empty;

    [ObservableProperty]
    private bool isBusyDisponibilidad;

    [ObservableProperty]
    private bool tieneFranjas = false;

    [ObservableProperty]
    private bool noTieneFranjas = true;

    [ObservableProperty]
    private bool isBusy;

    [ObservableProperty]
    private string errorMessage = string.Empty;

    public ObservableCollection<FranjaHoraria> FranjasDisponibles { get; } = new();

    public AdminProfessionalDetailViewModel(IEstilistaRepository estilistaRepository)
    {
        _estilistaRepository = estilistaRepository;
        ActualizarTextoFecha();
    }

    public async void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (query.TryGetValue("estilista", out var estObj) && estObj is Estilista est)
        {
            CargarDatosEstilista(est);
            await ConsultarDisponibilidadAsync();
        }
        else if (query.TryGetValue("estilistaId", out var idObj) && idObj is long id)
        {
            EstilistaId = id;
            await CargarPorIdAsync(id);
        }
    }

    private async Task CargarPorIdAsync(long id)
    {
        if (IsBusy) return;

        try
        {
            IsBusy = true;
            var e = await _estilistaRepository.GetEstilistaByIdAsync(id);
            if (e != null)
            {
                CargarDatosEstilista(e);
                await ConsultarDisponibilidadAsync();
            }
            else
            {
                ErrorMessage = "No se encontró el estilista solicitado en el servidor.";
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = "Error al consultar la información del estilista.";
            System.Diagnostics.Debug.WriteLine($"[AdminProfessionalDetailViewModel] Error: {ex.Message}");
        }
        finally
        {
            IsBusy = false;
        }
    }

    private void CargarDatosEstilista(Estilista e)
    {
        Estilista = e;
        EstilistaId = e.Id;
        NombreCompleto = !string.IsNullOrWhiteSpace(e.NombreCompleto) ? e.NombreCompleto : "Estilista";
        EspecialidadPrincipal = !string.IsNullOrWhiteSpace(e.EspecialidadPrincipal) ? e.EspecialidadPrincipal : "General";
        Biografia = !string.IsNullOrWhiteSpace(e.Biografia) ? e.Biografia : "Sin biografía registrada.";
        ColorAgenda = !string.IsNullOrWhiteSpace(e.ColorAgenda) ? e.ColorAgenda : "#C5A059";
        Activo = e.Activo;
        EstadoTexto = e.Activo ? "Activo" : "Inactivo";
        EstadoBadgeBgColor = e.Activo ? "#E8F5E9" : "#FFEBEE";
        EstadoBadgeTextColor = e.Activo ? "#2E7D32" : "#C62828";
        Iniciales = e.Iniciales;
    }

    private void ActualizarTextoFecha()
    {
        FechaDisponibilidadTexto = FechaDisponibilidad.ToString("dddd, dd 'de' MMMM yyyy");
    }

    [RelayCommand]
    private async Task ConsultarDisponibilidadAsync()
    {
        if (EstilistaId <= 0) return;

        try
        {
            IsBusyDisponibilidad = true;
            ActualizarTextoFecha();

            var disp = await _estilistaRepository.GetDisponibilidadHorariaAsync(EstilistaId, FechaDisponibilidad, 30);

            FranjasDisponibles.Clear();
            if (disp?.Franjas != null && disp.Franjas.Count > 0)
            {
                foreach (var f in disp.Franjas)
                {
                    FranjasDisponibles.Add(f);
                }
            }

            TieneFranjas = FranjasDisponibles.Count > 0;
            NoTieneFranjas = !TieneFranjas;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[AdminProfessionalDetailViewModel] Error disponibilidad: {ex.Message}");
            FranjasDisponibles.Clear();
            TieneFranjas = false;
            NoTieneFranjas = true;
        }
        finally
        {
            IsBusyDisponibilidad = false;
        }
    }

    [RelayCommand]
    private async Task DiaAnteriorAsync()
    {
        FechaDisponibilidad = FechaDisponibilidad.AddDays(-1);
        await ConsultarDisponibilidadAsync();
    }

    [RelayCommand]
    private async Task DiaSiguienteAsync()
    {
        FechaDisponibilidad = FechaDisponibilidad.AddDays(1);
        await ConsultarDisponibilidadAsync();
    }

    [RelayCommand]
    private async Task VerAgendaAsync()
    {
        await Shell.Current.GoToAsync("TimelineAgendaPage", new Dictionary<string, object>
        {
            { "estilistaId", EstilistaId }
        });
    }

    [RelayCommand]
    private async Task AgendarCitaAsync()
    {
        await Shell.Current.GoToAsync("AdminCreateAppointmentPage", new Dictionary<string, object>
        {
            { "estilistaPredefinidoId", EstilistaId }
        });
    }

    [RelayCommand]
    private async Task EditarProfesionalAsync()
    {
        if (Application.Current?.MainPage != null)
        {
            await Application.Current.MainPage.DisplayAlert(
                "Función Preparada",
                "La edición de profesionales estará disponible cuando el backend habilite esta operación.",
                "Entendido"
            );
        }
    }

    [RelayCommand]
    private async Task ActivarDesactivarAsync()
    {
        if (Application.Current?.MainPage != null)
        {
            await Application.Current.MainPage.DisplayAlert(
                "Función Preparada",
                "La activación/desactivación de profesionales estará disponible cuando el backend habilite esta operación.",
                "Entendido"
            );
        }
    }

    [RelayCommand]
    private async Task RegresarAsync()
    {
        await Shell.Current.GoToAsync("..");
    }
}
