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
using ShushineStudio.Mobile.Presentation.ViewModels.Catalog;
using ShushineStudio.Mobile.Presentation.ViewModels.Profile;
using ShushineStudio.Mobile.Presentation.Views.Appointments;
using ShushineStudio.Mobile.Presentation.Views.Auth;
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

		builder.Services.AddHttpClient<HttpClient>(client =>
		{
			client.BaseAddress = new Uri(ApiConstants.BaseUrl.TrimEnd('/') + "/");
			client.Timeout = TimeSpan.FromSeconds(30);
		})
		.AddHttpMessageHandler<ErrorDelegatingHandler>();

		// ==========================================
		// 2. Servicios de Datos (Storage, Supabase)
		// ==========================================
		builder.Services.AddSingleton<ITokenStorageService, TokenStorageService>();

		// ==========================================
		// 3. Repositorios (Data Layer -> Domain Interfaces)
		// ==========================================
		builder.Services.AddScoped<IServicioRepository, ServicioRepository>();
		builder.Services.AddScoped<IReservaRepository, ReservaRepository>();
		builder.Services.AddScoped<IAuthRepository, AuthRepository>();

		// ==========================================
		// 4. Casos de Uso (Domain Layer)
		// ==========================================
		builder.Services.AddTransient<GetServiciosCatalogUseCase>();
		builder.Services.AddTransient<CreateAppointmentUseCase>();

		// ==========================================
		// 5. ViewModels (Presentation Layer)
		// ==========================================
		builder.Services.AddTransient<LoginViewModel>();
		builder.Services.AddTransient<RegisterViewModel>();
		builder.Services.AddTransient<CatalogViewModel>();
		builder.Services.AddTransient<MyAppointmentsViewModel>();
		builder.Services.AddTransient<ProfileViewModel>();

		// ==========================================
		// 6. Páginas / Vistas (Presentation Layer)
		// ==========================================
		builder.Services.AddTransient<LoginPage>();
		builder.Services.AddTransient<RegisterPage>();
		builder.Services.AddTransient<CatalogPage>();
		builder.Services.AddTransient<MyAppointmentsPage>();
		builder.Services.AddTransient<ProfilePage>();

		return builder.Build();
	}
}
