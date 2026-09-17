import '../../core/error/failures.dart';
import '../../data/models/dashboard_metrics_dto.dart';
import '../../data/models/invoice_dto.dart';

abstract class BillingRepository {
  Future<Result<InvoiceDto>> getInvoiceByAppointmentId(int citaId);
  Future<Result<bool>> registerPayment(RegisterPaymentRequest request);
  Future<Result<DashboardMetricsDto>> getDashboardMetrics();
}
