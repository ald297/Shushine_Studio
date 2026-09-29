using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using ShushineStudio.Mobile.Data.Dtos;
using ShushineStudio.Mobile.Domain.Entities;
using ShushineStudio.Mobile.Domain.Repositories;
using ShushineStudio.Mobile.Data.Services;

namespace ShushineStudio.Mobile.Data.Repositories;

public class ClienteRepository : IClienteRepository
{
    private readonly HttpClient _httpClient;
    private readonly IReservaRepository _reservaRepository;
    private readonly ITokenStorageService _tokenStorageService;

    public ClienteRepository(
        HttpClient httpClient,
        IReservaRepository reservaRepository,
        ITokenStorageService tokenStorageService)
    {
        _httpClient = httpClient;
        _reservaRepository = reservaRepository;
        _tokenStorageService = tokenStorageService;
    }

    private async Task PrepararHeaderAutenticacionAsync()
    {
        var token = await _tokenStorageService.GetTokenAsync();
        if (!string.IsNullOrEmpty(token))
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }
    }

    public async Task<List<Cliente>> ObtenerClientesAsync(int page = 0, int size = 50)
    {
        await PrepararHeaderAutenticacionAsync();

        try
        {
            // 1. Intentar consultar endpoint dedicado si existe o se habilita en el backend
            var response = await _httpClient.GetAsync($"clientes?page={page}&size={size}");
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                using var doc = JsonDocument.Parse(content);

                JsonElement arrayElement;
                if (doc.RootElement.ValueKind == JsonValueKind.Array)
                {
                    arrayElement = doc.RootElement;
                }
                else if (doc.RootElement.TryGetProperty("content", out var contentElem) && contentElem.ValueKind == JsonValueKind.Array)
                {
                    arrayElement = contentElem;
                }
                else
                {
                    arrayElement = default;
                }

                if (arrayElement.ValueKind == JsonValueKind.Array)
                {
                    var lista = new List<Cliente>();
                    foreach (var elem in arrayElement.EnumerateArray())
                    {
                        var cliente = new Cliente
                        {
                            Id = elem.TryGetProperty("id", out var idProp) ? idProp.GetInt64() : 0,
                            NombreCompleto = elem.TryGetProperty("nombreCompleto", out var ncProp) ? ncProp.GetString() ?? "" :
                                             elem.TryGetProperty("nombre", out var nProp) ? nProp.GetString() ?? "" : "Cliente",
                            Telefono = elem.TryGetProperty("telefono", out var tProp) ? tProp.GetString() : null,
                            Correo = elem.TryGetProperty("correo", out var cProp) ? cProp.GetString() :
                                     elem.TryGetProperty("email", out var eProp) ? eProp.GetString() : null,
                            NivelFidelidad = elem.TryGetProperty("nivelFidelidad", out var nfProp) ? nfProp.GetString() : "Bronce",
                            EsWalkin = elem.TryGetProperty("esWalkin", out var ewProp) && ewProp.GetBoolean()
                        };
                        lista.Add(cliente);
                    }
                    return lista;
                }
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[ClienteRepository] Consulta a /api/clientes no disponible: {ex.Message}");
        }

        // 2. Si /api/clientes no está disponible en el backend actual,
        // derivamos los clientes reales a partir de las citas registradas en el backend
        try
        {
            var citasEnumerable = await _reservaRepository.ObtenerTodasCitasAdminPaginadasAsync(0, 100);
            var citas = citasEnumerable?.ToList() ?? new List<Reserva>();
            if (citas.Count == 0)
            {
                return new List<Cliente>();
            }

            // Agrupar por ID de cliente o por nombre si ID es 0
            var grupos = citas
                .Where(c => !string.IsNullOrWhiteSpace(c.ClienteNombre))
                .GroupBy(c => c.ClienteId > 0 ? c.ClienteId.ToString() : c.ClienteNombre.Trim().ToLowerInvariant())
                .ToList();

            var clientesDerivados = new List<Cliente>();
            long fallbackId = 1;

            foreach (var grupo in grupos)
            {
                var primeraCita = grupo.First();
                var citasCliente = grupo.OrderByDescending(c => c.FechaHoraInicio).ToList();

                var cliente = new Cliente
                {
                    Id = primeraCita.ClienteId > 0 ? primeraCita.ClienteId : fallbackId++,
                    NombreCompleto = primeraCita.ClienteNombre,
                    Telefono = !string.IsNullOrWhiteSpace(primeraCita.ClienteTelefono) ? primeraCita.ClienteTelefono : null,
                    EsWalkin = primeraCita.Notas?.Contains("Walk-in", StringComparison.OrdinalIgnoreCase) == true,
                    TotalCitas = citasCliente.Count,
                    Citas = citasCliente,
                    NivelFidelidad = citasCliente.Count >= 5 ? "Oro" : citasCliente.Count >= 3 ? "Plata" : "Bronce"
                };

                clientesDerivados.Add(cliente);
            }

            return clientesDerivados.OrderBy(c => c.NombreCompleto).ToList();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[ClienteRepository] Error al derivar clientes de citas: {ex.Message}");
            return new List<Cliente>();
        }
    }

    public async Task<Cliente?> ObtenerClientePorIdAsync(long id)
    {
        var todos = await ObtenerClientesAsync(0, 100);
        return todos.FirstOrDefault(c => c.Id == id);
    }

    public async Task<List<Reserva>> ObtenerHistorialCitasClienteAsync(long clienteId)
    {
        var todos = await ObtenerClientesAsync(0, 100);
        var cliente = todos.FirstOrDefault(c => c.Id == clienteId);
        return cliente?.Citas ?? new List<Reserva>();
    }
}
