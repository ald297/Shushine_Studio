import 'package:dio/dio.dart';
import '../../core/error/failures.dart';
import '../../core/network/dio_client.dart';
import '../models/appointment_dto.dart';
import '../models/timeline_item_dto.dart';

abstract class TimelineRemoteDataSource {
  Future<List<TimelineItemDto>> getTimeline({required String fecha, int? estilistaId});
  Future<AppointmentDto> updateAppointmentStatus({required int citaId, required String nuevoEstado, String? motivoCancelacion});
  Future<AppointmentDto> registerWalkin(CreateWalkinRequest request);
}

class TimelineRemoteDataSourceImpl implements TimelineRemoteDataSource {
  final DioClient _dioClient;

  TimelineRemoteDataSourceImpl({required DioClient dioClient})
      : _dioClient = dioClient;

  @override
  Future<List<TimelineItemDto>> getTimeline({required String fecha, int? estilistaId}) async {
    try {
      final response = await _dioClient.get(
        '/citas/timeline',
        queryParameters: {
          'fecha': fecha,
          if (estilistaId != null) 'estilistaId': estilistaId,
        },
      );
      final list = response.data as List<dynamic>;
      return list.map((item) => TimelineItemDto.fromJson(item as Map<String, dynamic>)).toList();
    } on DioException catch (e) {
      if (e.error is Failure) throw e.error as Failure;
      throw ServerFailure(
        message: e.message ?? 'Error al obtener la agenda timeline.',
        statusCode: e.response?.statusCode,
      );
    }
  }

  @override
  Future<AppointmentDto> updateAppointmentStatus({
    required int citaId,
    required String nuevoEstado,
    String? motivoCancelacion,
  }) async {
    try {
      final response = await _dioClient.patch(
        '/citas/$citaId/estado',
        data: {
          'id': citaId,
          'nuevoEstado': nuevoEstado,
          if (motivoCancelacion != null) 'motivoCancelacion': motivoCancelacion,
        },
      );
      return AppointmentDto.fromJson(response.data as Map<String, dynamic>);
    } on DioException catch (e) {
      if (e.error is Failure) throw e.error as Failure;
      throw ServerFailure(
        message: e.message ?? 'Error al cambiar el estado de la cita.',
        statusCode: e.response?.statusCode,
      );
    }
  }

  @override
  Future<AppointmentDto> registerWalkin(CreateWalkinRequest request) async {
    try {
      final response = await _dioClient.post(
        '/citas/walkin',
        data: request.toJson(),
      );
      return AppointmentDto.fromJson(response.data as Map<String, dynamic>);
    } on DioException catch (e) {
      if (e.error is Failure) throw e.error as Failure;
      throw ServerFailure(
        message: e.message ?? 'Error al registrar cliente walk-in.',
        statusCode: e.response?.statusCode,
      );
    }
  }
}
