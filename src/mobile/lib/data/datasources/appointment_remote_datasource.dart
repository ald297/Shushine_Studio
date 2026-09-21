import 'package:dio/dio.dart';
import '../../core/error/failures.dart';
import '../../core/network/dio_client.dart';
import '../models/appointment_dto.dart';

abstract class AppointmentRemoteDataSource {
  Future<AppointmentDto> createAppointment(CreateAppointmentRequest request);
  Future<List<AppointmentDto>> getMyAppointments();
  Future<AppointmentDto> getAppointmentById(int id);
}

class AppointmentRemoteDataSourceImpl implements AppointmentRemoteDataSource {
  final DioClient _dioClient;

  AppointmentRemoteDataSourceImpl({required DioClient dioClient})
      : _dioClient = dioClient;

  @override
  Future<AppointmentDto> createAppointment(CreateAppointmentRequest request) async {
    try {
      final response = await _dioClient.post(
        '/citas',
        data: request.toJson(),
      );
      return AppointmentDto.fromJson(response.data as Map<String, dynamic>);
    } on DioException catch (e) {
      if (e.error is Failure) throw e.error as Failure;
      throw ServerFailure(
        message: e.message ?? 'Error al procesar la reserva de cita.',
        statusCode: e.response?.statusCode,
      );
    }
  }

  @override
  Future<List<AppointmentDto>> getMyAppointments() async {
    try {
      final response = await _dioClient.get('/citas/mis-citas');
      final list = response.data as List<dynamic>;
      return list.map((item) => AppointmentDto.fromJson(item as Map<String, dynamic>)).toList();
    } on DioException catch (e) {
      if (e.error is Failure) throw e.error as Failure;
      throw ServerFailure(
        message: e.message ?? 'Error al obtener el historial de citas.',
        statusCode: e.response?.statusCode,
      );
    }
  }

  @override
  Future<AppointmentDto> getAppointmentById(int id) async {
    try {
      final response = await _dioClient.get('/citas/$id');
      return AppointmentDto.fromJson(response.data as Map<String, dynamic>);
    } on DioException catch (e) {
      if (e.error is Failure) throw e.error as Failure;
      throw ServerFailure(
        message: e.message ?? 'Error al obtener el detalle de la cita.',
        statusCode: e.response?.statusCode,
      );
    }
  }
}
