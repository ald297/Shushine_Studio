using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ShushineStudio.Mobile.Domain.Entities;
using ShushineStudio.Mobile.Domain.Repositories;

namespace ShushineStudio.Mobile.Presentation.ViewModels.Admin;

public partial class AdminClientDetailViewModel : ObservableObject, IQueryAttributable
{
    private readonly IClienteRepository _clienteRepository;

    [ObservableProperty]
    private string title = "Detalle de Cliente";

    [ObservableProperty]
    private Cliente? cliente;

    [ObservableProperty]
    private long clienteId;

    [ObservableProperty]
    private string nombreCompleto = string.Empty;

    [ObservableProperty]
    private string telefono = "No registrado";

    [ObservableProperty]
    private string correo = "No registrado";

    [ObservableProperty]
    private string nivelFidelidad = "Bronce";

    [ObservableProperty]
    private int puntosAcumulados = 0;

    [ObservableProperty]
    private string tipoCliente = "Cliente Registrado";

    [ObservableProperty]
    private string tipoClienteColor = "#9B2D47";

    [ObservableProperty]
    private string iniciales = "CL";

    [ObservableProperty]
    private int totalCitas = 0;

    [ObservableProperty]
    private bool tieneHistorial = false;

    [ObservableProperty]
    private bool noTieneHistorial = true;

    [ObservableProperty]
    private bool isBusy;

    [ObservableProperty]
    private string errorMessage = string.Empty;

    public ObservableCollection<Reserva> HistorialCitas { get; } = new();

    public AdminClientDetailViewModel(IClienteRepository clienteRepository)
    {
        _clienteRepository = clienteRepository;
    }

    public async void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (query.TryGetValue("cliente", out var cObj) && cObj is Cliente c)
        {
            CargarDatosCliente(c);
        }
        else if (query.TryGetValue("clienteId", out var idObj) && idObj is long id)
        {
            ClienteId = id;
            await CargarPorIdAsync(id);
        }
    }

    private async Task CargarPorIdAsync(long id)
    {
        if (IsBusy) return;

        try
        {
            IsBusy = true;
            var c = await _clienteRepository.ObtenerClientePorIdAsync(id);
            if (c != null)
            {
                CargarDatosCliente(c);
            }
            else
            {
                ErrorMessage = "No se encontró el cliente solicitado en el servidor.";
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = "Error al consultar la información del cliente.";
            System.Diagnostics.Debug.WriteLine($"[AdminClientDetailViewModel] Error: {ex.Message}");
        }
        finally
        {
            IsBusy = false;
        }
    }

    private void CargarDatosCliente(Cliente c)
    {
        Cliente = c;
        ClienteId = c.Id;
        NombreCompleto = !string.IsNullOrWhiteSpace(c.NombreCompleto) ? c.NombreCompleto : "Cliente General";
        Telefono = !string.IsNullOrWhiteSpace(c.Telefono) ? c.Telefono : "No registrado";
        Correo = !string.IsNullOrWhiteSpace(c.Correo) ? c.Correo : "No registrado";
        NivelFidelidad = !string.IsNullOrWhiteSpace(c.NivelFidelidad) ? c.NivelFidelidad : "Bronce";
        PuntosAcumulados = c.PuntosAcumulados;
        TipoCliente = c.TipoClienteDisplay;
        TipoClienteColor = c.TipoClienteColor;
        Iniciales = c.Iniciales;
        TotalCitas = c.TotalCitas;

        HistorialCitas.Clear();
        if (c.Citas != null && c.Citas.Count > 0)
        {
            foreach (var cita in c.Citas)
            {
                HistorialCitas.Add(cita);
            }
        }

        TieneHistorial = HistorialCitas.Count > 0;
        NoTieneHistorial = !TieneHistorial;
    }

    [RelayCommand]
    private async Task VerDetalleCitaAsync(Reserva reserva)
    {
        if (reserva == null) return;

        await Shell.Current.GoToAsync("AdminAppointmentDetailPage", new Dictionary<string, object>
        {
            { "reserva", reserva },
            { "citaId", reserva.Id }
        });
    }

    [RelayCommand]
    private async Task NuevaCitaParaClienteAsync()
    {
        await Shell.Current.GoToAsync("AdminCreateAppointmentPage", new Dictionary<string, object>
        {
            { "nombreClientePredefinido", NombreCompleto },
            { "telefonoClientePredefinido", Telefono != "No registrado" ? Telefono : "" }
        });
    }

    [RelayCommand]
    private async Task EditarClienteAsync()
    {
        if (Application.Current?.MainPage != null)
        {
            await Application.Current.MainPage.DisplayAlert(
                "Función Preparada",
                "Esta función estará disponible cuando el backend habilite el endpoint de edición de clientes.",
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
                "Esta función estará disponible cuando el backend habilite el endpoint de activación/desactivación de clientes.",
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
