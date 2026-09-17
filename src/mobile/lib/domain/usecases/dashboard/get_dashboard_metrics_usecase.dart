import '../../../../core/error/failures.dart';
import '../../../data/models/dashboard_metrics_dto.dart';
import '../../repositories/billing_repository.dart';

class GetDashboardMetricsUseCase {
  final BillingRepository _repository;

  GetDashboardMetricsUseCase(this._repository);

  Future<Result<DashboardMetricsDto>> call() async {
    return await _repository.getDashboardMetrics();
  }
}
