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
using ShushineStudio.Mobile.Presentation.ViewModels.Chat;
using ShushineStudio.Mobile.Presentation.ViewModels.Notifications;
using ShushineStudio.Mobile.Presentation.ViewModels.Profile;
using ShushineStudio.Mobile.Presentation.ViewModels.Reviews;
using ShushineStudio.Mobile.Presentation.Views.Admin;
using ShushineStudio.Mobile.Presentation.Views.Appointments;
using ShushineStudio.Mobile.Presentation.Views.Auth;
using ShushineStudio.Mobile.Presentation.Views.Booking;
using ShushineStudio.Mobile.Presentation.Views.Catalog;
using ShushineStudio.Mobile.Presentation.Views.Chat;
using ShushineStudio.Mobile.Presentation.Views.Notifications;
using ShushineStudio.Mobile.Presentation.Views.Profile;
using ShushineStudio.Mobile.Presentation.Views.Reviews;

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
				fonts.AddFont("PlayfairDisplay-Regular.ttf", "PlayfairDisplay");
				fonts.AddFont("PlayfairDisplay-SemiBold.ttf", "PlayfairDisplaySemiBold");
				fonts.AddFont("PlayfairDisplay-Bold.ttf", "PlayfairDisplayBold");
				fonts.AddFont("PlusJakartaSans-Regular.ttf", "PlusJakartaSans");
				fonts.AddFont("PlusJakartaSans-Medium.ttf", "PlusJakartaSansMedium");
				fonts.AddFont("PlusJakartaSans-SemiBold.ttf", "PlusJakartaSansSemiBold");
				fonts.AddFont("PlusJakartaSans-Bold.ttf", "PlusJakartaSansBold");
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
			client.Timeout = TimeSpan.FromSeconds(120);
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
		builder.Services.AddScoped<IClienteRepository, ClienteRepository>();
		builder.Services.AddScoped<IChatRepository, ChatRepository>();
		builder.Services.AddScoped<IResenaRepository, ResenaRepository>();
		builder.Services.AddScoped<IProductoRepository, ProductoRepository>();
		builder.Services.AddScoped<ISolicitudRepository, SolicitudRepository>();

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
		builder.Services.AddTransient<ClientProductsViewModel>();
		builder.Services.AddTransient<ServiceDetailViewModel>();
		builder.Services.AddTransient<StylistSelectionViewModel>();
		builder.Services.AddTransient<SlotSelectionViewModel>();
		builder.Services.AddTransient<BookingSummaryViewModel>();
		builder.Services.AddTransient<BookingConfirmationViewModel>();
		builder.Services.AddTransient<MyAppointmentsViewModel>();
		builder.Services.AddTransient<AppointmentDetailViewModel>();
		builder.Services.AddTransient<ProfileViewModel>();
		builder.Services.AddTransient<EditProfileViewModel>();
		builder.Services.AddTransient<SecurityViewModel>();
		builder.Services.AddTransient<PreferencesViewModel>();
		builder.Services.AddTransient<AppInfoViewModel>();
		builder.Services.AddTransient<AdminDashboardViewModel>();
		builder.Services.AddTransient<TimelineAgendaViewModel>();
		builder.Services.AddTransient<AdminCatalogViewModel>();
		builder.Services.AddTransient<AdminEstilistasViewModel>();
		builder.Services.AddTransient<AdminProfileViewModel>();
		builder.Services.AddTransient<WalkInViewModel>();
		builder.Services.AddTransient<ChatViewModel>();
		builder.Services.AddTransient<CustomRequestViewModel>();
		builder.Services.AddTransient<LeaveReviewViewModel>();
		builder.Services.AddTransient<MyReviewsViewModel>();
		builder.Services.AddTransient<ReviewDetailViewModel>();
		builder.Services.AddTransient<ReviewSuccessViewModel>();
		builder.Services.AddTransient<NotificationCenterViewModel>();
		builder.Services.AddTransient<NotificationDetailViewModel>();
		builder.Services.AddTransient<AdminAppointmentsViewModel>();
		builder.Services.AddTransient<AdminAppointmentDetailViewModel>();
		builder.Services.AddTransient<AdminCreateAppointmentViewModel>();
		builder.Services.AddTransient<AdminClientsViewModel>();
		builder.Services.AddTransient<AdminClientDetailViewModel>();
		builder.Services.AddTransient<AdminCreateClientViewModel>();
		builder.Services.AddTransient<AdminProfessionalsViewModel>();
		builder.Services.AddTransient<AdminProfessionalDetailViewModel>();
		builder.Services.AddTransient<AdminCreateProfessionalViewModel>();
		builder.Services.AddTransient<AdminServicesViewModel>();
		builder.Services.AddTransient<AdminServiceDetailViewModel>();
		builder.Services.AddTransient<AdminCreateServiceViewModel>();
		builder.Services.AddTransient<AdminProductsViewModel>();
		builder.Services.AddTransient<AdminProductDetailViewModel>();
		builder.Services.AddTransient<AdminMessagesViewModel>();
		builder.Services.AddTransient<AdminConversationViewModel>();
		builder.Services.AddTransient<AdminRequestsViewModel>();
		builder.Services.AddTransient<AdminRequestDetailViewModel>();
		builder.Services.AddTransient<AdminReviewsViewModel>();
		builder.Services.AddTransient<AdminReportsViewModel>();
		builder.Services.AddTransient<AdminSettingsViewModel>();

		// ==========================================
		// 6. Páginas / Vistas (Presentation Layer)
		// ==========================================
		builder.Services.AddTransient<LoginPage>();
		builder.Services.AddTransient<RegisterPage>();
		builder.Services.AddTransient<CatalogPage>();
		builder.Services.AddTransient<ClientProductsPage>();
		builder.Services.AddTransient<ServiceDetailPage>();
		builder.Services.AddTransient<StylistSelectionPage>();
		builder.Services.AddTransient<SlotSelectionPage>();
		builder.Services.AddTransient<BookingSummaryPage>();
		builder.Services.AddTransient<BookingConfirmationPage>();
		builder.Services.AddTransient<MyAppointmentsPage>();
		builder.Services.AddTransient<AppointmentDetailPage>();
		builder.Services.AddTransient<ProfilePage>();
		builder.Services.AddTransient<EditProfilePage>();
		builder.Services.AddTransient<SecurityPage>();
		builder.Services.AddTransient<PreferencesPage>();
		builder.Services.AddTransient<AppInfoPage>();
		builder.Services.AddTransient<LeaveReviewPage>();
		builder.Services.AddTransient<MyReviewsPage>();
		builder.Services.AddTransient<ReviewDetailPage>();
		builder.Services.AddTransient<ReviewSuccessPage>();
		builder.Services.AddTransient<NotificationCenterPage>();
		builder.Services.AddTransient<NotificationDetailPage>();
		builder.Services.AddTransient<AdminDashboardPage>();
		builder.Services.AddTransient<TimelineAgendaPage>();
		builder.Services.AddTransient<AdminAppointmentsPage>();
		builder.Services.AddTransient<AdminAppointmentDetailPage>();
		builder.Services.AddTransient<AdminCreateAppointmentPage>();
		builder.Services.AddTransient<AdminClientsPage>();
		builder.Services.AddTransient<AdminClientDetailPage>();
		builder.Services.AddTransient<AdminCreateClientPage>();
		builder.Services.AddTransient<AdminProfessionalsPage>();
		builder.Services.AddTransient<AdminProfessionalDetailPage>();
		builder.Services.AddTransient<AdminCreateProfessionalPage>();
		builder.Services.AddTransient<AdminServicesPage>();
		builder.Services.AddTransient<AdminServiceDetailPage>();
		builder.Services.AddTransient<AdminCreateServicePage>();
		builder.Services.AddTransient<AdminProductsPage>();
		builder.Services.AddTransient<AdminProductDetailPage>();
		builder.Services.AddTransient<AdminMessagesPage>();
		builder.Services.AddTransient<AdminConversationPage>();
		builder.Services.AddTransient<AdminRequestsPage>();
		builder.Services.AddTransient<AdminRequestDetailPage>();
		builder.Services.AddTransient<AdminReviewsPage>();
		builder.Services.AddTransient<AdminReportsPage>();
		builder.Services.AddTransient<AdminSettingsPage>();
		builder.Services.AddTransient<AdminCatalogPage>();
		builder.Services.AddTransient<AdminEstilistasPage>();
		builder.Services.AddTransient<AdminProfilePage>();
		builder.Services.AddTransient<WalkInPage>();
		builder.Services.AddTransient<ChatPage>();
		builder.Services.AddTransient<CustomRequestPage>();

		return builder.Build();
	}
}
