import 'package:bloc_test/bloc_test.dart';
import 'package:flutter_test/flutter_test.dart';
import 'package:mocktail/mocktail.dart';
import 'package:shushine_studio_mobile/core/error/failures.dart';
import 'package:shushine_studio_mobile/data/models/dashboard_metrics_dto.dart';
import 'package:shushine_studio_mobile/domain/usecases/dashboard/get_dashboard_metrics_usecase.dart';
import 'package:shushine_studio_mobile/presentation/blocs/dashboard/dashboard_bloc.dart';
import 'package:shushine_studio_mobile/presentation/blocs/dashboard/dashboard_event.dart';
import 'package:shushine_studio_mobile/presentation/blocs/dashboard/dashboard_state.dart';

class MockGetDashboardMetricsUseCase extends Mock implements GetDashboardMetricsUseCase {}

void main() {
  late MockGetDashboardMetricsUseCase mockGetDashboardMetricsUseCase;
  late DashboardBloc dashboardBloc;

  const tMetrics = DashboardMetricsDto(
    totalCitasHoy: 5,
    totalCitasSemana: 25,
    ingresosHoy: 250.0,
    ingresosMes: 2800.0,
    citasEnProceso: 2,
    citasCompletadas: 3,
    citasCanceladas: 0,
    estilistasActivos: 3,
  );

  setUp(() {
    mockGetDashboardMetricsUseCase = MockGetDashboardMetricsUseCase();
    dashboardBloc = DashboardBloc(
      getDashboardMetricsUseCase: mockGetDashboardMetricsUseCase,
    );
  });

  tearDown(() {
    dashboardBloc.close();
  });

  test('initial state should be DashboardInitial', () {
    expect(dashboardBloc.state, equals(DashboardInitial()));
  });

  group('FetchDashboardMetricsRequested', () {
    blocTest<DashboardBloc, DashboardState>(
      'emits [DashboardLoading, DashboardLoaded] when fetch succeeds',
      build: () {
        when(() => mockGetDashboardMetricsUseCase())
            .thenAnswer((_) async => const Success(tMetrics));
        return dashboardBloc;
      },
      act: (bloc) => bloc.add(FetchDashboardMetricsRequested()),
      expect: () => [
        DashboardLoading(),
        const DashboardLoaded(tMetrics),
      ],
      verify: (_) {
        verify(() => mockGetDashboardMetricsUseCase()).called(1);
      },
    );

    blocTest<DashboardBloc, DashboardState>(
      'emits [DashboardLoading, DashboardError] when fetch fails',
      build: () {
        when(() => mockGetDashboardMetricsUseCase())
            .thenAnswer((_) async => const ErrorResult(ServerFailure(message: 'Error al consultar métricas')));
        return dashboardBloc;
      },
      act: (bloc) => bloc.add(FetchDashboardMetricsRequested()),
      expect: () => [
        DashboardLoading(),
        const DashboardError('Error al consultar métricas'),
      ],
    );
  });
}
