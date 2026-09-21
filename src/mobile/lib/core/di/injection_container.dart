import 'package:get_it/get_it.dart';
import '../../data/datasources/appointment_remote_datasource.dart';
import '../../data/datasources/auth_remote_datasource.dart';
import '../../data/datasources/billing_remote_datasource.dart';
import '../../data/datasources/catalog_remote_datasource.dart';
import '../../data/datasources/stylist_remote_datasource.dart';
import '../../data/datasources/timeline_remote_datasource.dart';
import '../../data/repositories/appointment_repository_impl.dart';
import '../../data/repositories/auth_repository_impl.dart';
import '../../data/repositories/billing_repository_impl.dart';
import '../../data/repositories/catalog_repository_impl.dart';
import '../../data/repositories/stylist_repository_impl.dart';
import '../../data/repositories/timeline_repository_impl.dart';
import '../../domain/repositories/appointment_repository.dart';
import '../../domain/repositories/auth_repository.dart';
import '../../domain/repositories/billing_repository.dart';
import '../../domain/repositories/catalog_repository.dart';
import '../../domain/repositories/stylist_repository.dart';
import '../../domain/repositories/timeline_repository.dart';
import '../../domain/usecases/appointments/create_appointment_usecase.dart';
import '../../domain/usecases/appointments/get_my_appointments_usecase.dart';
import '../../domain/usecases/auth/get_current_user_usecase.dart';
import '../../domain/usecases/auth/login_usecase.dart';
import '../../domain/usecases/auth/logout_usecase.dart';
import '../../domain/usecases/auth/register_usecase.dart';
import '../../domain/usecases/billing/get_invoice_usecase.dart';
import '../../domain/usecases/billing/register_payment_usecase.dart';
import '../../domain/usecases/catalog/get_service_detail_usecase.dart';
import '../../domain/usecases/catalog/get_services_catalog_usecase.dart';
import '../../domain/usecases/dashboard/get_dashboard_metrics_usecase.dart';
import '../../domain/usecases/stylists/get_active_stylists_usecase.dart';
import '../../domain/usecases/stylists/get_stylist_availability_usecase.dart';
import '../../domain/usecases/timeline/get_timeline_usecase.dart';
import '../../domain/usecases/timeline/register_walkin_usecase.dart';
import '../../domain/usecases/timeline/update_appointment_status_usecase.dart';
import '../../presentation/blocs/appointments/appointment_bloc.dart';
import '../../presentation/blocs/auth/auth_bloc.dart';
import '../../presentation/blocs/billing/billing_bloc.dart';
import '../../presentation/blocs/booking/booking_bloc.dart';
import '../../presentation/blocs/catalog/catalog_bloc.dart';
import '../../presentation/blocs/dashboard/dashboard_bloc.dart';
import '../../presentation/blocs/timeline/timeline_bloc.dart';
import '../auth/token_storage.dart';
import '../network/dio_client.dart';

final sl = GetIt.instance;

Future<void> initDependencies() async {
  // 1. Core & Infrastructure
  sl.registerLazySingleton<TokenStorage>(() => TokenStorage());

  sl.registerLazySingleton<DioClient>(
    () => DioClient(tokenStorage: sl<TokenStorage>()),
  );

  // 2. Data Sources
  sl.registerLazySingleton<AuthRemoteDataSource>(
    () => AuthRemoteDataSourceImpl(dioClient: sl<DioClient>()),
  );
  sl.registerLazySingleton<CatalogRemoteDataSource>(
    () => CatalogRemoteDataSourceImpl(dioClient: sl<DioClient>()),
  );
  sl.registerLazySingleton<StylistRemoteDataSource>(
    () => StylistRemoteDataSourceImpl(dioClient: sl<DioClient>()),
  );
  sl.registerLazySingleton<AppointmentRemoteDataSource>(
    () => AppointmentRemoteDataSourceImpl(dioClient: sl<DioClient>()),
  );
  sl.registerLazySingleton<TimelineRemoteDataSource>(
    () => TimelineRemoteDataSourceImpl(dioClient: sl<DioClient>()),
  );
  sl.registerLazySingleton<BillingRemoteDataSource>(
    () => BillingRemoteDataSourceImpl(dioClient: sl<DioClient>()),
  );

  // 3. Repositories
  sl.registerLazySingleton<AuthRepository>(
    () => AuthRepositoryImpl(
      remoteDataSource: sl<AuthRemoteDataSource>(),
      tokenStorage: sl<TokenStorage>(),
    ),
  );
  sl.registerLazySingleton<CatalogRepository>(
    () => CatalogRepositoryImpl(
      remoteDataSource: sl<CatalogRemoteDataSource>(),
    ),
  );
  sl.registerLazySingleton<StylistRepository>(
    () => StylistRepositoryImpl(
      remoteDataSource: sl<StylistRemoteDataSource>(),
    ),
  );
  sl.registerLazySingleton<AppointmentRepository>(
    () => AppointmentRepositoryImpl(
      remoteDataSource: sl<AppointmentRemoteDataSource>(),
    ),
  );
  sl.registerLazySingleton<TimelineRepository>(
    () => TimelineRepositoryImpl(
      remoteDataSource: sl<TimelineRemoteDataSource>(),
    ),
  );
  sl.registerLazySingleton<BillingRepository>(
    () => BillingRepositoryImpl(
      remoteDataSource: sl<BillingRemoteDataSource>(),
    ),
  );

  // 4. Use Cases
  sl.registerLazySingleton(() => LoginUseCase(sl<AuthRepository>()));
  sl.registerLazySingleton(() => RegisterUseCase(sl<AuthRepository>()));
  sl.registerLazySingleton(() => GetCurrentUserUseCase(sl<AuthRepository>()));
  sl.registerLazySingleton(() => LogoutUseCase(sl<AuthRepository>()));

  sl.registerLazySingleton(() => GetServicesCatalogUseCase(sl<CatalogRepository>()));
  sl.registerLazySingleton(() => GetServiceDetailUseCase(sl<CatalogRepository>()));

  sl.registerLazySingleton(() => GetActiveStylistsUseCase(sl<StylistRepository>()));
  sl.registerLazySingleton(() => GetStylistAvailabilityUseCase(sl<StylistRepository>()));

  sl.registerLazySingleton(() => CreateAppointmentUseCase(sl<AppointmentRepository>()));
  sl.registerLazySingleton(() => GetMyAppointmentsUseCase(sl<AppointmentRepository>()));

  sl.registerLazySingleton(() => GetTimelineUseCase(sl<TimelineRepository>()));
  sl.registerLazySingleton(() => UpdateAppointmentStatusUseCase(sl<TimelineRepository>()));
  sl.registerLazySingleton(() => RegisterWalkinUseCase(sl<TimelineRepository>()));

  sl.registerLazySingleton(() => GetInvoiceUseCase(sl<BillingRepository>()));
  sl.registerLazySingleton(() => RegisterPaymentUseCase(sl<BillingRepository>()));
  sl.registerLazySingleton(() => GetDashboardMetricsUseCase(sl<BillingRepository>()));

  // 5. BLoCs
  sl.registerFactory(
    () => AuthBloc(
      loginUseCase: sl<LoginUseCase>(),
      registerUseCase: sl<RegisterUseCase>(),
      getCurrentUserUseCase: sl<GetCurrentUserUseCase>(),
      logoutUseCase: sl<LogoutUseCase>(),
    ),
  );
  sl.registerFactory(
    () => CatalogBloc(
      getCatalogUseCase: sl<GetServicesCatalogUseCase>(),
    ),
  );
  sl.registerFactory(
    () => BookingBloc(
      getActiveStylistsUseCase: sl<GetActiveStylistsUseCase>(),
      getAvailabilityUseCase: sl<GetStylistAvailabilityUseCase>(),
      createAppointmentUseCase: sl<CreateAppointmentUseCase>(),
    ),
  );
  sl.registerFactory(
    () => AppointmentBloc(
      getMyAppointmentsUseCase: sl<GetMyAppointmentsUseCase>(),
    ),
  );
  sl.registerFactory(
    () => TimelineBloc(
      getTimelineUseCase: sl<GetTimelineUseCase>(),
      updateStatusUseCase: sl<UpdateAppointmentStatusUseCase>(),
      registerWalkinUseCase: sl<RegisterWalkinUseCase>(),
    ),
  );
  sl.registerFactory(
    () => BillingBloc(
      getInvoiceUseCase: sl<GetInvoiceUseCase>(),
      registerPaymentUseCase: sl<RegisterPaymentUseCase>(),
    ),
  );
  sl.registerFactory(
    () => DashboardBloc(
      getDashboardMetricsUseCase: sl<GetDashboardMetricsUseCase>(),
    ),
  );
}
