using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ShushineStudio.Mobile.Data.Services;
using ShushineStudio.Mobile.Domain.Repositories;

namespace ShushineStudio.Mobile.Presentation.ViewModels.Admin;

public partial class AdminReportsViewModel : ObservableObject
{
    private readonly IReservaRepository _reservaRepository;
    private readonly IAuthRepository _authRepository;
    private readonly ITokenStorageService _tokenStorageService;

    [ObservableProperty]
    private string title = "Ingresos & Reportes";

    [ObservableProperty]
    private bool isAuthorized = true;

    [ObservableProperty]
    private bool isUnauthorized = false;

    [ObservableProperty]
    private bool isBusy;

    [ObservableProperty]
    private string? errorMessage;

    [ObservableProperty]
    private bool hasError;

    // Métricas Reales Obtenidas del Backend (/api/admin/dashboard)
    [ObservableProperty]
    private decimal ingresosHoy;

    [ObservableProperty]
    private decimal ingresosMes;

    [ObservableProperty]
    private long totalCitasHoy;

    [ObservableProperty]
    private long totalCitasSemana;

    [ObservableProperty]
    private long citasEnProceso;

    [ObservableProperty]
    private long citasCompletadas;

    [ObservableProperty]
    private long citasCanceladas;

    [ObservableProperty]
    private long estilistasActivos;

    [ObservableProperty]
    private string periodoSeleccionado = "Hoy"; // "Hoy", "Semana", "Mes"

    [ObservableProperty]
    private string avisoExportacion = "La exportación automatizada a PDF o Excel estará disponible cuando el backend implemente el servicio de generación de reportes tributarios.";

    public AdminReportsViewModel(
        IReservaRepository reservaRepository,
        IAuthRepository authRepository,
        ITokenStorageService tokenStorageService)
    {
        _reservaRepository = reservaRepository;
        _authRepository = authRepository;
        _tokenStorageService = tokenStorageService;

        _ = InicializarAsync();
    }

    private async Task InicializarAsync()
    {
        await VerificarRolAdminAsync();
        if (IsAuthorized)
        {
            await CargarReportesAsync();
        }
    }

    private async Task VerificarRolAdminAsync()
    {
        try
        {
            var rol = await _tokenStorageService.GetRoleAsync();
            var currentUser = await _authRepository.GetCurrentUserAsync();
            if (currentUser != null && !string.IsNullOrEmpty(currentUser.Rol))
            {
                rol = currentUser.Rol;
            }

            var esAdmin = !string.IsNullOrEmpty(rol) && rol.ToUpperInvariant().Contains("ADMIN");
            IsAuthorized = esAdmin;
            IsUnauthorized = !esAdmin;
        }
        catch
        {
            IsAuthorized = false;
            IsUnauthorized = true;
        }
    }

    [RelayCommand]
    public async Task CargarReportesAsync()
    {
        if (IsBusy || !IsAuthorized) return;

        try
        {
            IsBusy = true;
            ErrorMessage = null;
            HasError = false;

            var metricas = await _reservaRepository.GetDashboardMetricasAsync();
            if (metricas != null)
            {
                IngresosHoy = metricas.IngresosHoy;
                IngresosMes = metricas.IngresosMes;
                TotalCitasHoy = metricas.TotalCitasHoy;
                TotalCitasSemana = metricas.TotalCitasSemana;
                CitasEnProceso = metricas.CitasEnProceso;
                CitasCompletadas = metricas.CitasCompletadas;
                CitasCanceladas = metricas.CitasCanceladas;
                EstilistasActivos = metricas.EstilistasActivos;
            }
        }
        catch (Exception ex)
        {
            HasError = true;
            ErrorMessage = "Error al sincronizar métricas financieras: " + ex.Message;
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private void CambiarPeriodo(string periodo)
    {
        PeriodoSeleccionado = periodo;
    }

    [RelayCommand]
    private async Task ExportarReporteAsync()
    {
        try
        {
            IsBusy = true;
            var csv = await _reservaRepository.DescargarReporteCsvAsync();
            if (string.IsNullOrEmpty(csv))
            {
                await Shell.Current.DisplayAlert("Exportación", "No se pudo obtener el reporte del servidor.", "OK");
                return;
            }

            var fileName = $"Reporte_Shushine_{DateTime.Now:yyyyMMdd_HHmm}.csv";
            var filePath = Path.Combine(FileSystem.CacheDirectory, fileName);
            await File.WriteAllTextAsync(filePath, csv);

            await Share.Default.RequestAsync(new ShareFileRequest
            {
                Title = "Reporte Oficial de Citas y Finanzas - Shushine Studio",
                File = new ShareFile(filePath)
            });
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Error", "Error al exportar reporte: " + ex.Message, "OK");
        }
        finally
        {
            IsBusy = false;
        }
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
        if (Shell.Current is AppShell appShell)
        {
            appShell.SwitchToLogin();
        }
        else
        {
            await Shell.Current.GoToAsync("//LoginPage");
        }
    }
}
