using System.Text.Json.Serialization;

namespace ShushineStudio.Mobile.Core.Models;

/// <summary>
/// Modelo conforme a la especificación oficial RFC 7807 (Problem Details for HTTP APIs)
/// Retornado por el @RestControllerAdvice de la Web API en Java Spring Boot.
/// </summary>
public class ProblemDetailsDto
{
    [JsonPropertyName("type")]
    public string? Type { get; set; }

    [JsonPropertyName("title")]
    public string? Title { get; set; }

    [JsonPropertyName("status")]
    public int Status { get; set; }

    [JsonPropertyName("detail")]
    public string? Detail { get; set; }

    [JsonPropertyName("instance")]
    public string? Instance { get; set; }

    [JsonPropertyName("errors")]
    public Dictionary<string, string[]>? Errors { get; set; }

    /// <summary>
    /// Retorna el mensaje de error más específico para mostrar al usuario en español.
    /// </summary>
    public string GetPrimaryErrorMessage()
    {
        if (Errors != null && Errors.Count > 0)
        {
            foreach (var errorList in Errors.Values)
            {
                if (errorList != null && errorList.Length > 0 && !string.IsNullOrWhiteSpace(errorList[0]))
                {
                    return errorList[0];
                }
            }
        }

        return !string.IsNullOrWhiteSpace(Detail) 
            ? Detail 
            : Title ?? "Ha ocurrido un error inesperado al procesar la solicitud.";
    }
}
