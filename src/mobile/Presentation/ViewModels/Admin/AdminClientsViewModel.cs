using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ShushineStudio.Mobile.Data.Services;
using ShushineStudio.Mobile.Domain.Entities;
using ShushineStudio.Mobile.Domain.Repositories;

namespace ShushineStudio.Mobile.Presentation.ViewModels.Admin;

public partial class AdminClientsViewModel : ObservableObject
{
    private readonly IClienteRepository _clienteRepository;
    private readonly IAuthRepository _authRepository;
    private readonly ITokenStorageService _tokenStorageService;

    private List<Cliente> _todosLosClientes = new();

    [ObservableProperty]
    private string title = "Clientes";

    [ObservableProperty]
    private string subtitulo = "Directorio de clientes";

    [ObservableProperty]
    private bool isUnauthorized = false;

    [ObservableProperty]
    private bool isAuthorized = true;

    [ObservableProperty]
    private bool isBusy;

    [ObservableProperty]
    private string errorMessage = string.Empty;

    [ObservableProperty]
    private bool hasError = false;

    [ObservableProperty]
    private string textoBusqueda = string.Empty;

    [ObservableProperty]
    private int totalClientes;

    [ObservableProperty]
    private bool estaVacio;

    public ObservableCollection<Cliente> Clientes { get; } = new();

    public AdminClientsViewModel(
        IClienteRepository clienteRepository,
        IAuthRepository authRepository,
        ITokenStorageService tokenStorageService)
    {
        _clienteRepository = clienteRepository;
        _authRepository = authRepository;
        _tokenStorageService = tokenStorageService;
    }

    public async Task InicializarAsync()
    {
        await CargarPermisosAsync();
        if (IsAuthorized)
        {
            await CargarClientesAsync();
        }
    }

    private async Task CargarPermisosAsync()
    {
        try
        {
            var rol = await _tokenStorageService.GetRoleAsync();
            var currentUser = await _authRepository.GetCurrentUserAsync();
            if (currentUser != null && !string.IsNullOrEmpty(currentUser.Rol))
            {
                rol = currentUser.Rol;
            }

            if (string.IsNullOrEmpty(rol) || !rol.ToUpperInvariant().Contains("ADMIN"))
            {
                IsUnauthorized = true;
                IsAuthorized = false;
                ErrorMessage = "Acceso denegado: Esta pantalla está reservada exclusivamente para administradores.";
                return;
            }

            IsUnauthorized = false;
            IsAuthorized = true;
        }
        catch
        {
            IsUnauthorized = true;
            IsAuthorized = false;
        }
    }

    [RelayCommand]
    public async Task CargarClientesAsync()
    {
        if (IsBusy || !IsAuthorized) return;

        try
        {
            IsBusy = true;
            ErrorMessage = string.Empty;
            HasError = false;

            var lista = await _clienteRepository.ObtenerClientesAsync(0, 100);
            _todosLosClientes = lista ?? new List<Cliente>();

            AplicarFiltroBusqueda();
        }
        catch (Exception ex)
        {
            ErrorMessage = "Error al conectar con el servidor para consultar clientes.";
            HasError = true;
            System.Diagnostics.Debug.WriteLine($"[AdminClientsViewModel] Error: {ex.Message}");
            Clientes.Clear();
            EstaVacio = true;
        }
        finally
        {
            IsBusy = false;
        }
    }

    partial void OnTextoBusquedaChanged(string value)
    {
        AplicarFiltroBusqueda();
    }

    private void AplicarFiltroBusqueda()
    {
        Clientes.Clear();

        IEnumerable<Cliente> resultado = _todosLosClientes;

        if (!string.IsNullOrWhiteSpace(TextoBusqueda))
        {
            var query = TextoBusqueda.Trim().ToLowerInvariant();
            resultado = resultado.Where(c =>
                (!string.IsNullOrWhiteSpace(c.NombreCompleto) && c.NombreCompleto.ToLowerInvariant().Contains(query)) ||
                (!string.IsNullOrWhiteSpace(c.Telefono) && c.Telefono.Contains(query))
            );
        }

        foreach (var c in resultado)
        {
            Clientes.Add(c);
        }

        TotalClientes = Clientes.Count;
        EstaVacio = Clientes.Count == 0 && !IsBusy;
    }

    [RelayCommand]
    private async Task SeleccionarClienteAsync(Cliente cliente)
    {
        if (cliente == null) return;

        await Shell.Current.GoToAsync("AdminClientDetailPage", new Dictionary<string, object>
        {
            { "cliente", cliente },
            { "clienteId", cliente.Id }
        });
    }

    [RelayCommand]
    private async Task NuevoClienteAsync()
    {
        await Shell.Current.GoToAsync("AdminCreateClientPage");
    }

    [RelayCommand]
    private async Task RegresarAsync()
    {
        await Shell.Current.GoToAsync("..");
    }

    [RelayCommand]
    private async Task RegresarALoginAsync()
    {
        await _authRepository.LogoutAsync();
        await Shell.Current.GoToAsync("//LoginPage");
    }
}
