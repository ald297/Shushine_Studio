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

    public async Task<bool> LoginAsync(string email, string password)
    {
        var response = await _httpClient.PostAsJsonAsync("auth/login", new LoginRequestDto
        {
            Email = email,
            Password = password
        });

        if (response.IsSuccessStatusCode)
        {
            var authResult = await response.Content.ReadFromJsonAsync<AuthResponseDto>();
            if (authResult != null && !string.IsNullOrWhiteSpace(authResult.Token))
            {
                await _tokenStorage.SaveTokenAsync(
                    authResult.Token, 
                    authResult.RefreshToken, 
                    authResult.Rol, 
                    authResult.UserId
                );
                return true;
            }
        }

        return false;
    }

    public async Task<bool> RegisterAsync(string nombre, string email, string password, string telefono)
    {
        var response = await _httpClient.PostAsJsonAsync("auth/registro", new RegisterRequestDto
        {
            Nombre = nombre,
            Email = email,
            Password = password,
            Telefono = telefono
        });

        if (response.IsSuccessStatusCode)
        {
            var authResult = await response.Content.ReadFromJsonAsync<AuthResponseDto>();
            if (authResult != null && !string.IsNullOrWhiteSpace(authResult.Token))
            {
                await _tokenStorage.SaveTokenAsync(
                    authResult.Token, 
                    authResult.RefreshToken, 
                    authResult.Rol, 
                    authResult.UserId
                );
                return true;
            }
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

        var role = await _tokenStorage.GetRoleAsync() ?? "CLIENTE";
        return new Usuario
        {
            Rol = role
        };
    }
}
