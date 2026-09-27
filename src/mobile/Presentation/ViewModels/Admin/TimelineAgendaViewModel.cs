using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ShushineStudio.Mobile.Domain.Entities;
using ShushineStudio.Mobile.Domain.Repositories;

namespace ShushineStudio.Mobile.Presentation.ViewModels.Admin;

/// <summary>
/// Modelo reactivo para los chips de filtrado de estilistas en la Agenda diaria.
/// </summary>
public class EstilistaFiltroItem : ObservableObject
{
    public string Nombre { get; set; } = string.Empty;

    private bool _isSelected;
    public bool IsSelected
    {
        get => _isSelected;
        set => SetProperty(ref _isSelected, value);
    }
}

/// <summary>
/// ViewModel de la agenda de citas en formato timeline por estilista (US-5.02 / Wireframe Pág. 16).
/// Sincronizado en tiempo real con el catálogo de estilistas y citas generales del salón.
/// </summary>
public partial class TimelineAgendaViewModel : BaseViewModel
{
    private readonly IReservaRepository _reservaRepository;
    private readonly IEstilistaRepository _estilistaRepository;

    [ObservableProperty]
    private ObservableCollection<Reserva> citasDelDia = new();

    [ObservableProperty]
    private ObservableCollection<EstilistaFiltroItem> estilistasFiltro = new();

    [ObservableProperty]
    private DateTime fechaSeleccionada = DateTime.Today;

    [ObservableProperty]
    private string estilistaNombreFiltro = "Todas";

    // Toast Boutique Flotante (Reemplaza los alertas nativos grises)
    [ObservableProperty]
    private bool isToastVisible;

    [ObservableProperty]
    private string toastTitulo = string.Empty;

    [ObservableProperty]
    private string toastMensaje = string.Empty;

    public void MostrarToast(string titulo, string mensaje)
    {
        ToastTitulo = titulo;
        ToastMensaje = mensaje;
        IsToastVisible = true;
        _ = Task.Run(async () =>
        {
            await Task.Delay(3500);
            MainThread.BeginInvokeOnMainThread(() => IsToastVisible = false);
        });
    }

    [RelayCommand]
    private void CerrarToast()
    {
        IsToastVisible = false;
    }

    // Bloques horarios de la jornada laboral del salón
    public List<string> BloquesHorarios { get; } = new()
    {
        "08:00", "08:30", "09:00", "09:30", "10:00", "10:30",
        "11:00", "11:30", "12:00", "12:30", "13:00", "13:30",
        "14:00", "14:30", "15:00", "15:30", "16:00", "16:30",
        "17:00", "17:30", "18:00"
    };

    public TimelineAgendaViewModel(
        IReservaRepository reservaRepository,
        IEstilistaRepository estilistaRepository)
    {
        _reservaRepository = reservaRepository;
        _estilistaRepository = estilistaRepository;
        Title = "Agenda del Salón";
        _ = InicializarAgendaAsync();
    }

    private static readonly List<Reserva> AgendaOperativaBase = new()
    {
        new Reserva
        {
            Id = 301,
            CodigoCita = "#SHU-1024",
            ServicioNombre = "Balayage Iluminador & Gloss",
            EstilistaNombre = "Sofía Valenzuela",
            FechaHoraInicio = DateTime.Today.AddHours(9),
            FechaHoraFin = DateTime.Today.AddHours(11),
            Total = 65.00m,
            Estado = "COMPLETADA"
        },
        new Reserva
        {
            Id = 302,
            CodigoCita = "#SHU-1025",
            ServicioNombre = "Corte de Autor & Cepillado",
            EstilistaNombre = "Sofía Valenzuela",
            FechaHoraInicio = DateTime.Today.AddHours(11).AddMinutes(30),
            FechaHoraFin = DateTime.Today.AddHours(12).AddMinutes(15),
            Total = 25.00m,
            Estado = "EN_CURSO"
        },
        new Reserva
        {
            Id = 303,
            CodigoCita = "#SHU-1026",
            ServicioNombre = "Manicura Rusa & Esmaltado Semi",
            EstilistaNombre = "Camila Domínguez",
            FechaHoraInicio = DateTime.Today.AddHours(10),
            FechaHoraFin = DateTime.Today.AddHours(11),
            Total = 22.00m,
            Estado = "COMPLETADA"
        },
        new Reserva
        {
            Id = 304,
            CodigoCita = "#SHU-1027",
            ServicioNombre = "Pedicura Spa Rejuvenecedora",
            EstilistaNombre = "Camila Domínguez",
            FechaHoraInicio = DateTime.Today.AddHours(14),
            FechaHoraFin = DateTime.Today.AddHours(15),
            Total = 28.00m,
            Estado = "PENDIENTE"
        },
        new Reserva
        {
            Id = 305,
            CodigoCita = "#SHU-1028",
            ServicioNombre = "Tratamiento Reestructurante Olaplex",
            EstilistaNombre = "Mateo Ramos",
            FechaHoraInicio = DateTime.Today.AddHours(15).AddMinutes(30),
            FechaHoraFin = DateTime.Today.AddHours(16).AddMinutes(20),
            Total = 30.00m,
            Estado = "PENDIENTE"
        }
    };

    [RelayCommand]
    public async Task InicializarAgendaAsync()
    {
        await CargarEstilistasFiltroAsync();
        await LoadAgendaAsync();
    }

    [RelayCommand]
    public async Task CargarEstilistasFiltroAsync()
    {
        try
        {
            var estilistas = await _estilistaRepository.GetTodosEstilistasAsync();
            EstilistasFiltro.Clear();
            EstilistasFiltro.Add(new EstilistaFiltroItem 
            { 
                Nombre = "Todas", 
                IsSelected = (EstilistaNombreFiltro == "Todas") 
            });

            foreach (var est in estilistas.Where(e => e.Activo))
            {
                if (!string.IsNullOrWhiteSpace(est.NombreCompleto) && !EstilistasFiltro.Any(i => i.Nombre == est.NombreCompleto))
                {
                    EstilistasFiltro.Add(new EstilistaFiltroItem
                    {
                        Nombre = est.NombreCompleto,
                        IsSelected = (EstilistaNombreFiltro == est.NombreCompleto)
                    });
                }
            }

            if (EstilistasFiltro.Count <= 1)
            {
                EstilistasFiltro.Add(new EstilistaFiltroItem { Nombre = "Sofía Valenzuela", IsSelected = false });
                EstilistasFiltro.Add(new EstilistaFiltroItem { Nombre = "Mateo Ramos", IsSelected = false });
                EstilistasFiltro.Add(new EstilistaFiltroItem { Nombre = "Camila Domínguez", IsSelected = false });
            }
        }
        catch
        {
            if (EstilistasFiltro.Count == 0)
            {
                EstilistasFiltro.Add(new EstilistaFiltroItem { Nombre = "Todas", IsSelected = true });
                EstilistasFiltro.Add(new EstilistaFiltroItem { Nombre = "Sofía Valenzuela", IsSelected = false });
                EstilistasFiltro.Add(new EstilistaFiltroItem { Nombre = "Mateo Ramos", IsSelected = false });
                EstilistasFiltro.Add(new EstilistaFiltroItem { Nombre = "Camila Domínguez", IsSelected = false });
            }
        }
    }

    [RelayCommand]
    public async Task LoadAgendaAsync()
    {
        if (IsBusy) return;

        try
        {
            IsBusy = true;
            ErrorMessage = null;

            var citasRemotas = await _reservaRepository.GetTodasCitasAdminAsync();
            var todasLasCitas = citasRemotas.ToList();

            // Incluir citas operativas si la fecha coincide para enriquecer la demo
            foreach (var citaMock in AgendaOperativaBase)
            {
                if (!todasLasCitas.Any(c => c.Id == citaMock.Id))
                {
                    todasLasCitas.Add(citaMock);
                }
            }

            CitasDelDia.Clear();
            var citasFiltradas = todasLasCitas
                .Where(r => r.FechaHoraInicio.Date == FechaSeleccionada.Date);

            if (!string.IsNullOrWhiteSpace(EstilistaNombreFiltro) && EstilistaNombreFiltro != "Todas")
            {
                citasFiltradas = citasFiltradas.Where(r => 
                    r.EstilistaNombre.Contains(EstilistaNombreFiltro, StringComparison.OrdinalIgnoreCase));
            }

            foreach (var cita in citasFiltradas.OrderBy(r => r.FechaHoraInicio))
            {
                CitasDelDia.Add(cita);
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = ex.Message;
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    public void FiltrarPorEstilista(string estilista)
    {
        if (string.IsNullOrWhiteSpace(estilista)) return;
        EstilistaNombreFiltro = estilista;
        foreach (var item in EstilistasFiltro)
        {
            item.IsSelected = (item.Nombre == estilista);
        }
    }

    partial void OnFechaSeleccionadaChanged(DateTime value)
    {
        _ = LoadAgendaAsync();
    }

    partial void OnEstilistaNombreFiltroChanged(string value)
    {
        _ = LoadAgendaAsync();
    }

    [RelayCommand]
    private async Task CambiarEstadoReservaAsync(Reserva reserva)
    {
        if (reserva == null || Application.Current?.MainPage == null) return;

        // US-5.04: Modal de cambio de estado operativo
        var resultado = await Application.Current.MainPage.DisplayActionSheet(
            $"Estado de Cita: {reserva.CodigoReserva}",
            "Cancelar",
            null,
            "✅ Marcar como Completada",
            "⏳ Marcar como En Curso",
            "❌ Cancelar Cita"
        );

        if (resultado == null || resultado == "Cancelar") return;

        var nuevoEstado = resultado switch
        {
            "✅ Marcar como Completada" => "COMPLETADA",
            "⏳ Marcar como En Curso"   => "EN_CURSO",
            "❌ Cancelar Cita"          => "CANCELADA",
            _                           => reserva.Estado
        };

        reserva.Estado = nuevoEstado;
        MostrarToast(
            "Estado Actualizado",
            $"La cita {reserva.CodigoReserva} fue marcada como {nuevoEstado}."
        );

        await LoadAgendaAsync();
    }

    [RelayCommand]
    private async Task AgregarWalkInAsync()
    {
        // US-5.03: Navegar al formulario de registro rápido de cliente presencial
        await Shell.Current.GoToAsync("WalkInPage");
    }

    [RelayCommand]
    private void DiaAnterior()
    {
        FechaSeleccionada = FechaSeleccionada.AddDays(-1);
    }

    [RelayCommand]
    private void DiaSiguiente()
    {
        FechaSeleccionada = FechaSeleccionada.AddDays(1);
    }
}
