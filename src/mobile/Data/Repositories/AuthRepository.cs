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
            var cleanInput = emailOrLogin.Trim();
            var request = new LoginRequestDto
            {
                Login = cleanInput,
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

            // Fallback 1: Si se ingresó con mayúscula por autocapitalización del teclado (ej. Admin o Cliente)
            var lowerLogin = cleanInput.ToLowerInvariant();
            if (lowerLogin != cleanInput)
            {
                var lowerRequest = new LoginRequestDto
                {
                    Login = lowerLogin,
                    Clave = password
                };

                var lowerResponse = await _httpClient.PostAsJsonAsync("auth/login", lowerRequest);
                if (lowerResponse.IsSuccessStatusCode)
                {
                    var authResult = await lowerResponse.Content.ReadFromJsonAsync<AuthResponseDto>();
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

            // Fallback 2: Si el usuario ingresó un correo completo pero se registró con el nombre de usuario (ej. camila@gmail.com -> camila)
            if (cleanInput.Contains("@"))
            {
                var fallbackUsername = cleanInput.Split('@')[0].ToLowerInvariant();
                var fallbackRequest = new LoginRequestDto
                {
                    Login = fallbackUsername,
                    Clave = password
                };

                var fallbackResponse = await _httpClient.PostAsJsonAsync("auth/login", fallbackRequest);
                if (fallbackResponse.IsSuccessStatusCode)
                {
                    var authResult = await fallbackResponse.Content.ReadFromJsonAsync<AuthResponseDto>();
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
        }
        catch (TaskCanceledException)
        {
            throw new InvalidOperationException("El servidor está iniciando o tardó en responder. Por favor espera unos segundos e intenta nuevamente.");
        }
        catch (HttpRequestException)
        {
            throw new InvalidOperationException("No se pudo conectar con el servidor. Por favor verifica tu conexión a internet o intenta de nuevo.");
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
                Login = !string.IsNullOrWhiteSpace(login) ? login.Trim() : email.Trim(),
                Clave = clave,
                Nombre = nombre.Trim(),
                Apellido = apellido.Trim(),
                Telefono = telefono?.Trim(),
                RolId = null // Backend asigna automáticamente CLIENTE
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
            else if (!string.IsNullOrWhiteSpace(email) && request.Login != email.Trim())
            {
                // Si el alias extraído del correo ya existía, intentar registrar con el correo completo como login único
                request.Login = email.Trim();
                var retryResponse = await _httpClient.PostAsJsonAsync("auth/registro", request);
                if (retryResponse.IsSuccessStatusCode)
                {
                    var authResult = await retryResponse.Content.ReadFromJsonAsync<AuthResponseDto>();
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

    public async Task<bool> CambiarClaveAsync(string claveActual, string nuevaClave, string confirmarClave)
    {
        var body = new
        {
            claveActual = claveActual,
            nuevaClave = nuevaClave,
            confirmarClave = confirmarClave
        };

        var response = await _httpClient.PutAsJsonAsync("auth/cambiar-clave", body);
        if (!response.IsSuccessStatusCode)
        {
            var errorContent = await response.Content.ReadAsStringAsync();
            throw new InvalidOperationException(!string.IsNullOrWhiteSpace(errorContent) ? errorContent : "Error al cambiar la contraseña en el servidor.");
        }
        return true;
    }

    public async Task<Usuario?> ActualizarPerfilAsync(string nombre, string apellido, string telefono, string? correo)
    {
        var body = new
        {
            nombre = nombre,
            apellido = apellido,
            telefono = telefono,
            correo = correo
        };

        var response = await _httpClient.PutAsJsonAsync("auth/perfil", body);
        response.EnsureSuccessStatusCode();

        var perfilDto = await response.Content.ReadFromJsonAsync<UsuarioPerfilDto>();
        return perfilDto?.ToEntity();
    }
}

