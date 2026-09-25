using System.Net.Http.Json;
using ShushineStudio.Mobile.Data.Dtos;
using ShushineStudio.Mobile.Data.Services;
using ShushineStudio.Mobile.Domain.Entities;
using ShushineStudio.Mobile.Domain.Repositories;

namespace ShushineStudio.Mobile.Data.Repositories;

public class AuthRepository : IAuthRepository
{
    private readonly HttpClient _httpClient;
    private readonly ITokenStorageService _tokenStorage;

    public AuthRepository(HttpClient httpClient, ITokenStorageService tokenStorage)
    {
        _httpClient = httpClient;
        _tokenStorage = tokenStorage;
    }

    public async Task<bool> LoginAsync(string emailOrLogin, string password)
    {
        try
        {
            var request = new LoginRequestDto
            {
                Login = emailOrLogin,
                Clave = password
            };

            var response = await _httpClient.PostAsJsonAsync("auth/login", request);
            if (response.IsSuccessStatusCode)
            {
                var authResult = await response.Content.ReadFromJsonAsync<AuthResponseDto>();
                if (authResult != null && !string.IsNullOrWhiteSpace(authResult.Token))
                {
                    await _tokenStorage.SaveTokenAsync(
                        authResult.Token, 
                        null, 
                        authResult.Rol, 
                        authResult.Id ?? authResult.Login ?? "user"
                    );
                    return true;
                }
            }
        }
        catch (Exception)
        {
            return false;
        }

        return false;
    }

    public Task<bool> RegisterAsync(string nombre, string email, string password, string telefono)
    {
        var partes = nombre.Split(' ', 2, StringSplitOptions.RemoveEmptyEntries);
        var nom = partes.Length > 0 ? partes[0] : nombre;
        var ape = partes.Length > 1 ? partes[1] : "Cliente";
        var login = email.Contains("@") ? email.Split('@')[0] : email;

        return RegisterAsync(login, password, nom, ape, email, telefono);
    }

    public async Task<bool> RegisterAsync(string login, string clave, string nombre, string apellido, string email, string telefono)
    {
        try
        {
            var request = new RegisterRequestDto
            {
                Login = !string.IsNullOrWhiteSpace(login) ? login : email,
                Clave = clave,
                Nombre = nombre,
                Apellido = apellido,
                Telefono = telefono,
                RolId = 2 // 2 = CLIENTE
            };

            var response = await _httpClient.PostAsJsonAsync("auth/registro", request);
            if (response.IsSuccessStatusCode)
            {
                var authResult = await response.Content.ReadFromJsonAsync<AuthResponseDto>();
                if (authResult != null && !string.IsNullOrWhiteSpace(authResult.Token))
                {
                    await _tokenStorage.SaveTokenAsync(
                        authResult.Token, 
                        null, 
                        authResult.Rol, 
                        authResult.Id ?? authResult.Login ?? "user"
                    );
                    return true;
                }
            }
        }
        catch (Exception)
        {
            return false;
        }

        return false;
    }

    public async Task LogoutAsync()
    {
        await _tokenStorage.ClearAsync();
    }

    public async Task<bool> IsAuthenticatedAsync()
    {
        return await _tokenStorage.HasValidTokenAsync();
    }

    public async Task<Usuario?> GetCurrentUserAsync()
    {
        if (!await IsAuthenticatedAsync()) return null;

        try
        {
            // Intentar obtener perfil actualizado desde el endpoint oficial GET /api/auth/me
            var response = await _httpClient.GetAsync("auth/me");
            if (response.IsSuccessStatusCode)
            {
                var perfilDto = await response.Content.ReadFromJsonAsync<UsuarioPerfilDto>();
                if (perfilDto != null)
                {
                    return perfilDto.ToEntity();
                }
            }
        }
        catch (Exception)
        {
            // Fallback a almacenamiento local si se está fuera de línea
        }

        var role = await _tokenStorage.GetRoleAsync() ?? "CLIENTE";
        var userId = await _tokenStorage.GetUserIdAsync() ?? "cliente";
        return new Usuario
        {
            Login = userId,
            NombreCompleto = userId,
            Rol = role
        };
    }
}
