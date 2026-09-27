using CommunityToolkit.Maui;
using Microsoft.Extensions.Logging;
using ShushineStudio.Mobile.Core.Constants;
using ShushineStudio.Mobile.Core.Handlers;
using ShushineStudio.Mobile.Data.Repositories;
using ShushineStudio.Mobile.Data.Services;
using ShushineStudio.Mobile.Domain.Repositories;
using ShushineStudio.Mobile.Domain.UseCases;
using ShushineStudio.Mobile.Presentation.ViewModels.Appointments;
using ShushineStudio.Mobile.Presentation.ViewModels.Auth;
using ShushineStudio.Mobile.Presentation.ViewModels.Admin;
using ShushineStudio.Mobile.Presentation.ViewModels.Booking;
using ShushineStudio.Mobile.Presentation.ViewModels.Catalog;
using ShushineStudio.Mobile.Presentation.ViewModels.Profile;
using ShushineStudio.Mobile.Presentation.Views.Admin;
using ShushineStudio.Mobile.Presentation.Views.Appointments;
using ShushineStudio.Mobile.Presentation.Views.Auth;
using ShushineStudio.Mobile.Presentation.Views.Booking;
using ShushineStudio.Mobile.Presentation.Views.Catalog;
using ShushineStudio.Mobile.Presentation.Views.Profile;

namespace ShushineStudio.Mobile;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()
			.UseMauiCommunityToolkit()
			.ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
				fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
			});

#if DEBUG
		builder.Logging.AddDebug();
#endif

		// ==========================================
		// 1. Core & Red (Handlers y HttpClient)
		// ==========================================
		builder.Services.AddTransient<ErrorDelegatingHandler>();

		builder.Services.AddHttpClient("ShushineApi", client =>
		{
			client.BaseAddress = new Uri(ApiConstants.BaseUrl.TrimEnd('/') + "/");
			client.Timeout = TimeSpan.FromSeconds(75);
		})
		.AddHttpMessageHandler<ErrorDelegatingHandler>();

		builder.Services.AddScoped(sp => sp.GetRequiredService<IHttpClientFactory>().CreateClient("ShushineApi"));

		// ==========================================
		// 2. Servicios de Datos (Storage, Supabase)
		// ==========================================
		builder.Services.AddSingleton<ITokenStorageService, TokenStorageService>();

		// ==========================================
		// 3. Repositorios (Data Layer -> Domain Interfaces)
		// ==========================================
		builder.Services.AddScoped<IServicioRepository, ServicioRepository>();
		builder.Services.AddScoped<IEstilistaRepository, EstilistaRepository>();
		builder.Services.AddScoped<IReservaRepository, ReservaRepository>();
		builder.Services.AddScoped<IAuthRepository, AuthRepository>();

		// ==========================================
		// 4. Casos de Uso (Domain Layer)
		// ==========================================
		builder.Services.AddTransient<GetServiciosCatalogUseCase>();
		builder.Services.AddTransient<GetEstilistasUseCase>();
		builder.Services.AddTransient<GetDisponibilidadUseCase>();
		builder.Services.AddTransient<GetMyAppointmentsUseCase>();
		builder.Services.AddTransient<CreateAppointmentUseCase>();
		builder.Services.AddTransient<CancelAppointmentUseCase>();

		// ==========================================
		// 5. ViewModels (Presentation Layer)
		// ==========================================
		builder.Services.AddTransient<LoginViewModel>();
		builder.Services.AddTransient<RegisterViewModel>();
		builder.Services.AddTransient<CatalogViewModel>();
		builder.Services.AddTransient<ServiceDetailViewModel>();
		builder.Services.AddTransient<StylistSelectionViewModel>();
		builder.Services.AddTransient<SlotSelectionViewModel>();
		builder.Services.AddTransient<BookingSummaryViewModel>();
		builder.Services.AddTransient<BookingConfirmationViewModel>();
		builder.Services.AddTransient<MyAppointmentsViewModel>();
		builder.Services.AddTransient<ProfileViewModel>();
		builder.Services.AddTransient<AdminDashboardViewModel>();
		builder.Services.AddTransient<TimelineAgendaViewModel>();
		builder.Services.AddTransient<AdminCatalogViewModel>();
		builder.Services.AddTransient<AdminEstilistasViewModel>();
		builder.Services.AddTransient<WalkInViewModel>();

		// ==========================================
		// 6. Páginas / Vistas (Presentation Layer)
		// ==========================================
		builder.Services.AddTransient<LoginPage>();
		builder.Services.AddTransient<RegisterPage>();
		builder.Services.AddTransient<CatalogPage>();
		builder.Services.AddTransient<ServiceDetailPage>();
		builder.Services.AddTransient<StylistSelectionPage>();
		builder.Services.AddTransient<SlotSelectionPage>();
		builder.Services.AddTransient<BookingSummaryPage>();
		builder.Services.AddTransient<BookingConfirmationPage>();
		builder.Services.AddTransient<MyAppointmentsPage>();
		builder.Services.AddTransient<ProfilePage>();
		builder.Services.AddTransient<AdminDashboardPage>();
		builder.Services.AddTransient<TimelineAgendaPage>();
		builder.Services.AddTransient<AdminCatalogPage>();
		builder.Services.AddTransient<AdminEstilistasPage>();
		builder.Services.AddTransient<WalkInPage>();

		return builder.Build();
	}
}
