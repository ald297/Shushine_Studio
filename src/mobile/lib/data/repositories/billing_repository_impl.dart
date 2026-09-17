import '../../core/error/failures.dart';
import '../../domain/repositories/billing_repository.dart';
import '../datasources/billing_remote_datasource.dart';
import '../models/dashboard_metrics_dto.dart';
import '../models/invoice_dto.dart';

class BillingRepositoryImpl implements BillingRepository {
  final BillingRemoteDataSource _remoteDataSource;

  BillingRepositoryImpl({required BillingRemoteDataSource remoteDataSource})
      : _remoteDataSource = remoteDataSource;

  @override
  Future<Result<InvoiceDto>> getInvoiceByAppointmentId(int citaId) async {
    try {
      final invoice = await _remoteDataSource.getInvoiceByAppointmentId(citaId);
      return Success(invoice);
    } on Failure catch (f) {
      return ErrorResult(f);
    } catch (e) {
      return ErrorResult(ServerFailure(message: e.toString()));
    }
  }

  @override
  Future<Result<bool>> registerPayment(RegisterPaymentRequest request) async {
    try {
      await _remoteDataSource.registerPayment(request);
      return const Success(true);
    } on Failure catch (f) {
      return ErrorResult(f);
    } catch (e) {
      return ErrorResult(ServerFailure(message: e.toString()));
    }
  }

  @override
  Future<Result<DashboardMetricsDto>> getDashboardMetrics() async {
    try {
      final metrics = await _remoteDataSource.getDashboardMetrics();
      return Success(metrics);
    } on Failure catch (f) {
      return ErrorResult(f);
    } catch (e) {
      return ErrorResult(ServerFailure(message: e.toString()));
    }
  }
}
