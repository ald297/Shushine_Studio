import 'package:flutter_bloc/flutter_bloc.dart';
import '../../../domain/usecases/dashboard/get_dashboard_metrics_usecase.dart';
import 'dashboard_event.dart';
import 'dashboard_state.dart';

class DashboardBloc extends Bloc<DashboardEvent, DashboardState> {
  final GetDashboardMetricsUseCase _getDashboardMetricsUseCase;

  DashboardBloc({required GetDashboardMetricsUseCase getDashboardMetricsUseCase})
      : _getDashboardMetricsUseCase = getDashboardMetricsUseCase,
        super(DashboardInitial()) {
    on<FetchDashboardMetricsRequested>(_onFetchMetrics);
  }

  Future<void> _onFetchMetrics(
    FetchDashboardMetricsRequested event,
    Emitter<DashboardState> emit,
  ) async {
    emit(DashboardLoading());
    final result = await _getDashboardMetricsUseCase();
    result.when(
      onSuccess: (metrics) => emit(DashboardLoaded(metrics)),
      onError: (failure) => emit(DashboardError(failure.message)),
    );
  }
}
