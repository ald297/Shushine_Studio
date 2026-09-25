using System.Net;
using System.Net.Http.Headers;
using System.Text.Json;
using ShushineStudio.Mobile.Core.Constants;
using ShushineStudio.Mobile.Core.Models;

namespace ShushineStudio.Mobile.Core.Handlers;

/// <summary>
/// Interceptor HTTP centralizado para:
/// 1. Adjuntar el token JWT en la cabecera Authorization: Bearer <token>.
/// 2. Capturar y deserializar errores bajo el estándar RFC 7807 (Problem Details).
/// 3. Manejar 401 (expiración de sesión) redirigiendo a la pantalla de Login.
/// 4. Manejar 403 (falta de permisos) y 409 (conflicto de reservas).
/// </summary>
public class ErrorDelegatingHandler : DelegatingHandler
{
    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request, 
        CancellationToken cancellationToken)
    {
        // 1. Inyectar token de autenticación si existe en SecureStorage
        var token = await SecureStorage.Default.GetAsync(ApiConstants.AuthTokenKey);
        if (!string.IsNullOrWhiteSpace(token) && request.Headers.Authorization == null)
        {
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }

        // 2. Ejecutar la petición HTTP
        HttpResponseMessage response;
        try
        {
            response = await base.SendAsync(request, cancellationToken);
        }
        catch (HttpRequestException)
        {
            await MainThread.InvokeOnMainThreadAsync(async () =>
            {
                if (Application.Current?.MainPage != null)
                {
                    await Application.Current.MainPage.DisplayAlert(
                        "Sin Conexión", 
                        "No se pudo establecer comunicación con el servidor del salón. Verifique su conexión a internet.", 
                        "Aceptar"
                    );
                }
            });
            throw;
        }

        // 3. Procesar códigos de error
        if (!response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync(cancellationToken);
            ProblemDetailsDto? problem = null;

            try
            {
                if (!string.IsNullOrWhiteSpace(content))
                {
                    problem = JsonSerializer.Deserialize<ProblemDetailsDto>(content, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });
                }
            }
            catch
            {
                // Si la respuesta no es JSON válido (ej. 502/504 en HTML desde un proxy)
            }

            await MainThread.InvokeOnMainThreadAsync(async () =>
            {
                var mainPage = Application.Current?.MainPage;
                if (mainPage == null) return;

                switch (response.StatusCode)
                {
                    case HttpStatusCode.Unauthorized:
                        // 401: Sesión expirada o no autorizada
                        SecureStorage.Default.Remove(ApiConstants.AuthTokenKey);
                        SecureStorage.Default.Remove(ApiConstants.UserRoleKey);
                        await mainPage.DisplayAlert(
                            "Sesión Finalizada", 
                            "Tu sesión ha expirado. Por favor, ingresa de nuevo con tus credenciales.", 
                            "Iniciar Sesión"
                        );
                        await Shell.Current.GoToAsync("//LoginPage");
                        break;

                    case HttpStatusCode.Forbidden:
                        // 403: Rol sin permisos
                        await mainPage.DisplayAlert(
                            "Acceso Denegado", 
                            "No posees los privilegios requeridos para realizar esta operación.", 
                            "Entendido"
                        );
                        break;

                    case HttpStatusCode.Conflict:
                        // 409: Conflicto de concurrencia / Doble reserva
                        var conflictMsg = problem?.GetPrimaryErrorMessage() 
                            ?? "El horario seleccionado ya no se encuentra disponible. Por favor, elija otro turno.";
                        await mainPage.DisplayAlert("Horario No Disponible", conflictMsg, "Elegir Otro");
                        break;

                    case HttpStatusCode.BadRequest:
                    case HttpStatusCode.UnprocessableEntity:
                        // 400 / 422: Validaciones de negocio fallidas
                        var validationMsg = problem?.GetPrimaryErrorMessage() 
                            ?? "Los datos ingresados no son válidos. Por favor, revíselos.";
                        await mainPage.DisplayAlert("Aviso de Validación", validationMsg, "Corregir");
                        break;

                    case HttpStatusCode.InternalServerError:
                    case HttpStatusCode.BadGateway:
                    case HttpStatusCode.ServiceUnavailable:
                    case HttpStatusCode.GatewayTimeout:
                        // 5xx: Fallas en servidor
                        await mainPage.DisplayAlert(
                            "Aviso del Salón", 
                            "El sistema del salón experimenta una interrupción momentánea. Intente nuevamente en unos minutos.", 
                            "Cerrar"
                        );
                        break;
                }
            });
        }

        return response;
    }
}
