import 'package:dio/dio.dart';
import '../../core/error/failures.dart';
import '../../core/network/dio_client.dart';
import '../models/dashboard_metrics_dto.dart';
import '../models/invoice_dto.dart';

abstract class BillingRemoteDataSource {
  Future<InvoiceDto> getInvoiceByAppointmentId(int citaId);
  Future<void> registerPayment(RegisterPaymentRequest request);
  Future<DashboardMetricsDto> getDashboardMetrics();
}

class BillingRemoteDataSourceImpl implements BillingRemoteDataSource {
  final DioClient _dioClient;

  BillingRemoteDataSourceImpl({required DioClient dioClient}) : _dioClient = dioClient;

  @override
  Future<InvoiceDto> getInvoiceByAppointmentId(int citaId) async {
    try {
      final response = await _dioClient.get('/pagos/factura/cita/$citaId');
      return InvoiceDto.fromJson(response.data as Map<String, dynamic>);
    } on DioException catch (e) {
      if (e.error is Failure) throw e.error as Failure;
      throw ServerFailure(
        message: e.message ?? 'Error al obtener la factura de la cita.',
        statusCode: e.response?.statusCode,
      );
    }
  }

  @override
  Future<void> registerPayment(RegisterPaymentRequest request) async {
    try {
      await _dioClient.post('/pagos', data: request.toJson());
    } on DioException catch (e) {
      if (e.error is Failure) throw e.error as Failure;
      throw ServerFailure(
        message: e.message ?? 'Error al registrar el pago.',
        statusCode: e.response?.statusCode,
      );
    }
  }

  @override
  Future<DashboardMetricsDto> getDashboardMetrics() async {
    try {
      final response = await _dioClient.get('/admin/dashboard');
      return DashboardMetricsDto.fromJson(response.data as Map<String, dynamic>);
    } on DioException catch (e) {
      if (e.error is Failure) throw e.error as Failure;
      throw ServerFailure(
        message: e.message ?? 'Error al obtener métricas del dashboard.',
        statusCode: e.response?.statusCode,
      );
    }
  }
}
