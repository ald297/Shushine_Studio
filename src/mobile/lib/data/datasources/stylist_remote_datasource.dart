import 'package:dio/dio.dart';
import '../../core/error/failures.dart';
import '../../core/network/dio_client.dart';
import '../models/stylist_dto.dart';

abstract class StylistRemoteDataSource {
  Future<List<StylistDto>> getActiveStylists();
  Future<AvailabilityDto> getStylistAvailability(int stylistId, String fecha, {int? duracionMinutos});
}

class StylistRemoteDataSourceImpl implements StylistRemoteDataSource {
  final DioClient _dioClient;

  StylistRemoteDataSourceImpl({required DioClient dioClient})
      : _dioClient = dioClient;

  @override
  Future<List<StylistDto>> getActiveStylists() async {
    try {
      final response = await _dioClient.get('/estilistas');
      final list = response.data as List<dynamic>;
      return list.map((item) => StylistDto.fromJson(item as Map<String, dynamic>)).toList();
    } on DioException catch (e) {
      if (e.error is Failure) throw e.error as Failure;
      throw ServerFailure(
        message: e.message ?? 'Error al obtener la lista de estilistas.',
        statusCode: e.response?.statusCode,
      );
    }
  }

  @override
  Future<AvailabilityDto> getStylistAvailability(int stylistId, String fecha, {int? duracionMinutos}) async {
    try {
      final response = await _dioClient.get(
        '/estilistas/$stylistId/disponibilidad',
        queryParameters: {
          'fecha': fecha,
          if (duracionMinutos != null) 'duracionMinutos': duracionMinutos,
        },
      );
      final data = response.data as Map<String, dynamic>;
      return AvailabilityDto.fromJson(data);
    } on DioException catch (e) {
      if (e.error is Failure) throw e.error as Failure;
      throw ServerFailure(
        message: e.message ?? 'Error al calcular la disponibilidad horaria.',
        statusCode: e.response?.statusCode,
      );
    }
  }
}
